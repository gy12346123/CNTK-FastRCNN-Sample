using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CNTK_FastRCNN_Sample
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// Image file path
        /// </summary>
        private string localImagePath;

        /// <summary>
        /// Image file path list
        /// </summary>
        private List<string> localImageFile;

        /// <summary>
        /// UI bingding data
        /// </summary>
        public UIData uiData;

        /// <summary>
        /// Drawing group for Image_MouseFocus
        /// </summary>
        private DrawingGroup mouseFocusGD;

        /// <summary>
        /// Drawing group for Image_Bbox
        /// </summary>
        private DrawingGroup BboxGD;

        /// <summary>
        /// Draw mouse focus line
        /// </summary>
        private Pen mouseFocusPen = new Pen(Brushes.MediumVioletRed, 2d);

        /// <summary>
        /// Bounding box line
        /// </summary>
        private Pen BboxPen = new Pen(Brushes.Green,2d);

        /// <summary>
        /// Bouding box line for choose label
        /// </summary>
        private Pen BboxLabelPen = new Pen(Brushes.MediumVioletRed, 4d);

        /// <summary>
        /// thickness 0
        /// </summary>
        private Pen drawingBboxNoticePen = new Pen(Brushes.Red, 0d);

        /// <summary>
        /// UIImage actual rect
        /// </summary>
        private Rect UIImageActualRect;

        /// <summary>
        /// Bbox start point
        /// </summary>
        private Point startPoint;

        /// <summary>
        /// Bbox end point
        /// </summary>
        private Point EndPoint;

        /// <summary>
        /// Draw Bbox flag
        /// </summary>
        private bool flag_DrawBbox = false;

        /// <summary>
        /// Auto skip image flag
        /// </summary>
        private bool flag_AutoSkipImage = true;

        /// <summary>
        /// Bbox rect list
        /// </summary>
        private List<Rect> BboxList;

        /// <summary>
        /// File path now loaded
        /// </summary>
        private string filePathNowLoaded;

        /// <summary>
        /// Raad labels from App.config
        /// </summary>
        private List<string> labelList;

        /// <summary>
        /// Labels which user selected
        /// </summary>
        private List<string> labelSelectedList;

        /// <summary>
        /// Thread reset event
        /// </summary>
        private AutoResetEvent autoResetEvent;

        public MainWindow()
        {
            InitializeComponent();
            uiData = new UIData();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            grid_Main.DataContext = uiData;

            SetLabelButton();


            //Button[] button = new Button[2] {
            //    new Button{
            //        Content = "head",
            //        Width = 50,
            //        Height = 25},
            //    new Button{
            //        Content = "eye",
            //        Width = 50,
            //        Height = 25}
            //};
            //foreach (Button B in button)
            //{
            //    B.Click += button_Template_Click;
            //    wrapPanel_Button.Children.Add(B);
            //}

        }

        private void SetLabelButton()
        {
            if (Setting.labelSet == null || Setting.labelSet.Equals(""))
            {
                this.ShowMessageAsync("Notice","Label has not found!");
                return;
            }
            labelList = new List<string>();
            foreach (string label in Setting.labelSet.Split('|'))
            {
                labelList.Add(label);
            }
            Button[] button = new Button[labelList.Count()];
            for (int i = 0; i < labelList.Count(); i++)
            {
                button[i] = new Button {
                    Content = labelList[i],
                    Margin = new Thickness(5d)
                };
                button[i].Click += button_Template_Click;
                wrapPanel_Button.Children.Add(button[i]);
            }
        }

        private void button_Template_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            uiData.TextMessage = b.Content.ToString();
            if (labelSelectedList != null)
            {
                labelSelectedList.Add(b.Content.ToString());
            }
            autoResetEvent.Set();
        }

        private System.Windows.Input.ICommand openFirstFlyoutCommand;

        public System.Windows.Input.ICommand OpenFirstFlyoutCommand
        {
            get
            {
                return this.openFirstFlyoutCommand ?? (this.openFirstFlyoutCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => this.Flyouts.Items.Count > 0,
                    ExecuteDelegate = x => this.ToggleFlyout(0)
                });
            }
        }

        /// <summary>
        /// Show the flyout
        /// </summary>
        /// <param name="index">the number of Flyout Item</param>
        private void ToggleFlyout(int index)
        {
            var flyout = this.Flyouts.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }

            flyout.IsOpen = !flyout.IsOpen;
        }

        private void button_Setting_Click(object sender, RoutedEventArgs e)
        {
            ToggleFlyout(0);
        }

        private void radioButton_Local_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)radioButton_Local.IsChecked)
            {
                button_LoadLocalFile.IsEnabled = true;
            }
        }

        private void radioButton_Net_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)radioButton_Net.IsChecked)
            {
                button_LoadLocalFile.IsEnabled = false;
            }
        }

        private void button_LoadLocalFile_Click(object sender, RoutedEventArgs e)
        {
            string path = Tools.GetPath("Select image path",Environment.SpecialFolder.Desktop);
            if (path != null || Directory.Exists(path))
            {
                localImagePath = path;
            }else
            {
                this.ShowMessageAsync("Notice","Image path not exist!");
                return;
            }
            grid_Main.Visibility = Visibility.Visible;
            localImageFile = new List<string>();
            Task.Factory.StartNew(()=>{
                GetImageFile(localImagePath, ref localImageFile);
                StartDrawBbox(ref localImageFile);
            });
            ToggleFlyout(0);
        }

        private void GetImageFile(string dir, ref List<string> fileList)
        {
            try
            {
                uiData.progressRing_IsActive = true;
                DirectoryInfo dirInfo = new DirectoryInfo(dir);

                SearchFiles("*.jpg", ref dirInfo, ref fileList);
                SearchFiles("*.png", ref dirInfo, ref fileList);
                SearchFiles("*.bmp", ref dirInfo, ref fileList);
                fileList.Sort();
                uiData.LastCount = fileList.Count();
            }
            catch (Exception)
            {
                this.ShowMessageAsync("Notice","Find image file error.");
                return;
            }finally
            {
                uiData.progressRing_IsActive = false;
            }
        }

        private void SearchFiles(string filter, ref DirectoryInfo dirInfo, ref List<string> fileList)
        {
            foreach (FileInfo fileInfo in dirInfo.GetFiles(filter))
            {
                if (flag_AutoSkipImage)
                {
                    string path = string.Format("{0}\\{1}.bboxes.tsv", fileInfo.DirectoryName, fileInfo.Name.Split('.')[0]);
                    if (!File.Exists(path))
                    {
                        fileList.Add(fileInfo.FullName);
                    }
                }
                else
                {
                    fileList.Add(fileInfo.FullName);
                }
            }
        }

        private void StartDrawBbox(ref List<string> fileList)
        {
            if (fileList.Count() > 0)
            {
                ShowImage(fileList.First());
                fileList.RemoveAt(0);
            }else
            {
                this.Dispatcher.Invoke(() => {
                    grid_Main.Visibility = Visibility.Hidden;
                    this.ShowMessageAsync("Done", "All images has been done.");
                });
            }
        }

        private void ShowImage(string file)
        {
            uiData.progressRing_IsActive = true;
            filePathNowLoaded = file;
            FileInfo info = new FileInfo(file);
            uiData.TextMessage = string.Format("Load {0},draw bounding box..",info.Name);
            CallResetOtherImage_Delegate();
            Task.Factory.StartNew(()=>{
                System.Drawing.Bitmap image = System.Drawing.Bitmap.FromFile(file) as System.Drawing.Bitmap;
                BitmapSource bitmap = Imaging.CreateBitmapSourceFromBitmap(ref image);
                uiData.UIImage = bitmap;
                uiData.progressRing_IsActive = false;
                uiData.LastCount--;
                image.Dispose();
                bitmap = null;
            });
        }

        private void ResetOtherImage()
        {
            BboxList = new List<Rect>();
            mouseFocusGD = new DrawingGroup();
            BboxGD = new DrawingGroup();
            uiData.MouseFocusImage = new DrawingImage(mouseFocusGD);
            uiData.BboxImage = new DrawingImage(BboxGD);
        }

        private delegate void ResetOtherImage_Delegate();

        private void CallResetOtherImage_Delegate()
        {
            this.Dispatcher.Invoke(new ResetOtherImage_Delegate(ResetOtherImage));
        }

        private void DrawMouseFocus(ref Point point)
        {
            if (point.X > Image_Show.ActualWidth || point.Y > Image_Show.ActualHeight)
            {
                return;
            }
            using (DrawingContext DC = mouseFocusGD.Open())
            {
                DC.DrawLine(mouseFocusPen, new Point(0d, point.Y), new Point(Image_Show.ActualWidth, point.Y));
                DC.DrawLine(mouseFocusPen, new Point(point.X, 0d), new Point(point.X, Image_Show.ActualHeight));
            }
        }

        private void DrawBbox(ref DrawingGroup DG, ref Point start, ref Point end, ref Pen focusPen, ref Pen noticePen)
        {
            using (DrawingContext DC = DG.Open())
            {
                DC.DrawRectangle(null, noticePen, UIImageActualRect);
                DC.DrawRectangle(null, focusPen, new Rect(start,end));
            }
        }

        private void DrawBbox(ref DrawingGroup DG, ref List<Rect> list, ref Pen focusPen, ref Pen noticePen)
        {
            if (list.Count() == 0)
            {
                using (DrawingContext DC = DG.Open())
                {
                    DC.DrawRectangle(null, noticePen, UIImageActualRect);
                }
                return;
            }
            using (DrawingContext DC = DG.Open())
            {
                DC.DrawRectangle(null, noticePen, UIImageActualRect);
                foreach (Rect rect in list)
                {
                    DC.DrawRectangle(null, focusPen, rect);
                }
            }
        }

        private delegate void DrawBbox_Delegate(ref DrawingGroup DG, ref List<Rect> list, ref Pen focusPen, ref Pen noticePen);

        private void CallDrawBbox_Delegate(ref DrawingGroup DG, ref List<Rect> list, ref Pen focusPen, ref Pen noticePen)
        {
            this.Dispatcher.Invoke(new DrawBbox_Delegate(DrawBbox), DG, list, focusPen, noticePen);
        }

        private void NextImage_CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if(localImageFile.Count() > 0 || BboxList.Count() > 0)
            {
                e.CanExecute = true;
            }else
            {
                e.CanExecute = false;
                this.ShowMessageAsync("Notice", "All image has been drawed.");
            }
        }

        private void NextImage_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (BboxList == null || BboxList.Count() == 0)
            {
                return;
            }

            new Thread(new ThreadStart(Thread_NextImage)).Start();
        }

        private void Image_Show_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(Image_Show);
            if (!flag_DrawBbox)
            {
                DrawMouseFocus(ref point);
            }else
            {
                DrawBbox(ref mouseFocusGD, ref startPoint, ref point, ref mouseFocusPen, ref drawingBboxNoticePen);
            }
        }

        private void Image_MouseFocus_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(Image_Show);
            if (!flag_DrawBbox)
            {
                DrawMouseFocus(ref point);
            }else
            {
                DrawBbox(ref mouseFocusGD, ref startPoint, ref point, ref mouseFocusPen, ref drawingBboxNoticePen);
            }
        }

        private void Image_MouseFocus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            flag_DrawBbox = true;
            UIImageActualRect = new Rect(0, 0, Image_Show.ActualWidth, Image_Show.ActualHeight);
            startPoint = e.GetPosition(Image_Show);
        }

        private void Image_MouseFocus_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            flag_DrawBbox = false;
            EndPoint = e.GetPosition(Image_Show);
            BboxList.Add(new Rect(startPoint, EndPoint));

            DrawBbox(ref BboxGD, ref BboxList, ref BboxPen, ref drawingBboxNoticePen);
        }

        private void Image_MouseFocus_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!flag_DrawBbox && BboxList.Count() > 0)
            {
                BboxList.RemoveAt(BboxList.Count() - 1);
                DrawBbox(ref BboxGD, ref BboxList, ref BboxPen, ref drawingBboxNoticePen);
            }
        }

        private void SaveBboxes(string filePath, ref List<Rect> list)
        {
            using (StreamWriter SW = new StreamWriter(new FileStream(filePath, FileMode.CreateNew)))
            {
                double rate = uiData.UIImage.Width / Image_Show.ActualWidth;
                foreach (Rect rect in list)
                {
                    //SW.WriteLine(rect.TopLeft.X + @"	" + rect.TopLeft.Y + @"	" + rect.BottomRight.X + @"	" + rect.BottomRight.Y);
                    SW.WriteLine(Convert.ToInt32(rect.TopLeft.X * rate) + @"	" + Convert.ToInt32(rect.TopLeft.Y * rate) + @"	" + Convert.ToInt32(rect.BottomRight.X * rate) + @"	" + Convert.ToInt32(rect.BottomRight.Y * rate));
                }
            }
        }

        private void ChooseAndSaveLabels(string filePath, List<Rect> list)
        {
            uiData.TextMessage = "Choose label..";
            labelSelectedList = new List<string>();
            autoResetEvent = new AutoResetEvent(false);
            foreach (Rect rect in list)
            {
                List<Rect> tempList = new List<Rect>() { rect };
                //DrawBbox(ref BboxGD, ref tempList, ref mouseFocusPen, ref drawingBboxNoticePen);
                CallDrawBbox_Delegate(ref BboxGD, ref tempList, ref BboxLabelPen, ref drawingBboxNoticePen);
                autoResetEvent.WaitOne();
            }

            if (list.Count() == labelSelectedList.Count())
            {
                using (StreamWriter SW = new StreamWriter(new FileStream(filePath,FileMode.CreateNew)))
                {
                    foreach (string label in labelSelectedList)
                    {
                        SW.WriteLine(label);
                    }
                }
                uiData.TextMessage = "Save label succeed..";
            }
        }

        private void Thread_NextImage()
        {
            uiData.MouseFocusImageVisibility = Visibility.Hidden;
            uiData.WarpPanel_ButtonVisibility = Visibility.Visible;

            // Save bounding box
            FileInfo info = new FileInfo(filePathNowLoaded);
            SaveBboxes(string.Format("{0}\\{1}.bboxes.tsv", info.DirectoryName, info.Name.Split('.')[0]), ref BboxList);

            // Choose label
            ChooseAndSaveLabels(string.Format("{0}\\{1}.bboxes.labels.tsv", info.DirectoryName, info.Name.Split('.')[0]), BboxList);
            uiData.WarpPanel_ButtonVisibility = Visibility.Hidden;
            uiData.MouseFocusImageVisibility = Visibility.Visible;

            // Next image
            StartDrawBbox(ref localImageFile);
        }

        private void toggleSwitch_AutoSkipImage_IsCheckedChanged(object sender, EventArgs e)
        {
            if ((bool)toggleSwitch_AutoSkipImage.IsChecked)
            {
                flag_AutoSkipImage = true;
            }else if (!(bool)toggleSwitch_AutoSkipImage.IsChecked)
            {
                flag_AutoSkipImage = false;
            }
        }
    }
}
