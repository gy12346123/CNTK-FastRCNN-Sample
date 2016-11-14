using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        private string localImagePath;

        private List<string> localImageFile;
        public MainWindow()
        {
            InitializeComponent();
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
            localImageFile = new List<string>();
            Task.Factory.StartNew(()=>{
                getImageFile(localImagePath, ref localImageFile);
            });

        }

        private void getImageFile(string dir, ref List<string> fileList)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                foreach (FileInfo fileInfo in dirInfo.GetFiles("*.jpg"))
                {
                    fileList.Add(fileInfo.FullName);
                }
                foreach (FileInfo fileInfo in dirInfo.GetFiles("*.png"))
                {
                    fileList.Add(fileInfo.FullName);
                }
                foreach (FileInfo fileInfo in dirInfo.GetFiles("*.bmp"))
                {
                    fileList.Add(fileInfo.FullName);
                }
                fileList.Sort();
            }
            catch (Exception)
            {
                this.ShowMessageAsync("Notice","Find image file error.");
                return;
            }
        }
    }
}
