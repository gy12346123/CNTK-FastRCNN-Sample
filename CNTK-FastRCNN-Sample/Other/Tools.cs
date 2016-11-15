using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CNTK_FastRCNN_Sample
{
    /// <summary>
    /// Tools often used
    /// </summary>
    class Tools
    {

        /// <summary>
        /// return TimeStamp as string.
        /// </summary>
        /// <returns>TimeStamp</returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// Get file name which user selectd.
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="filter">Example:Output.log|*.log|Output.txt|*.txt</param>
        /// <param name="initDir">Default path</param>
        /// <param name="multiSelect">Multi file</param>
        /// <param name="checkPath">Check path if not exist will show the error message</param>
        /// <param name="checkFile">Check file if not exist will show the error message</param>
        /// <returns>File name with full path,if select one file,return string[0]</returns>
        public static string[] GetFile(string title,string filter,string initDir,bool multiSelect = false,bool checkPath = false,bool checkFile = false)
        {
            System.Windows.Forms.OpenFileDialog OFD = new System.Windows.Forms.OpenFileDialog();
            OFD.Title = title;
            OFD.Filter = filter;
            OFD.InitialDirectory = initDir;
            OFD.Multiselect = multiSelect;
            OFD.CheckPathExists = checkPath;
            OFD.CheckFileExists = checkFile;
            if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (multiSelect)
                {
                    return OFD.FileNames;
                }else
                {
                    return new string[1] { OFD.FileName };
                }

            }
            else
            {
                return new string[1] { String.Empty };
            }
        }

        public static string GetPath(string title,System.Environment.SpecialFolder initDir = Environment.SpecialFolder.MyPictures)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = title;
            dialog.RootFolder = initDir;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return dialog.SelectedPath;
            }else
            {
                return null;
            }
        }

        /// <summary>
        /// Set a action run as async and run callback action if has
        /// </summary>
        /// <param name="action">Async fuction</param>
        /// <param name="callback">callback fuction</param>
        public static async void RunAsync(Action action,Action callback = null)
        {
            //Func<System.Threading.Tasks.Task> funcTask = () => {
            //    return System.Threading.Tasks.Task.Run(()=> {
            //        action();
            //    });
            //};
            //await funcTask();
            await new Func<System.Threading.Tasks.Task>(() => Task.Run(action)).Invoke();
            callback?.Invoke();
        }
    }

    /// <summary>
    /// Image converter
    /// </summary>
    public static class Imaging
    {
        /// <summary>
        /// Bitmap to BitmapSource
        /// </summary>
        /// <param name="bitmap">Bitmap</param>
        /// <returns>BitmapSource</returns>
        public static BitmapSource CreateBitmapSourceFromBitmap(ref Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            if (Application.Current.Dispatcher == null)
                return null; // Is it possible?

            using (MemoryStream memoryStream = new MemoryStream())
            {
                // You need to specify the image format to fill the stream. 
                // I'm assuming it is PNG
                bitmap.Save(memoryStream, ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Make sure to create the bitmap in the UI thread
                if (InvokeRequired)
                    return (BitmapSource)Application.Current.Dispatcher.Invoke(
                        new Func<Stream, BitmapSource>(CreateBitmapSourceFromBitmap),
                        DispatcherPriority.Normal,
                        memoryStream);

                return CreateBitmapSourceFromBitmap(memoryStream);
            }
        }

        /// <summary>
        /// BitmapSource to Bitmap
        /// </summary>
        /// <param name="s">BitmapSource</param>
        /// <returns>Bitmap</returns>
        public static System.Drawing.Bitmap WpfBitmapSourceToBitmap(BitmapSource s)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(s.PixelWidth, s.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            s.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }

        /// <summary>
        /// Cut bitmap and return new size bitmap resource
        /// </summary>
        /// <param name="b">raw bitmap</param>
        /// <param name="StartX">start X point</param>
        /// <param name="StartY">start Y point</param>
        /// <param name="iWidth">cut width</param>
        /// <param name="iHeight">cut height</param>
        /// <returns></returns>
        public static Bitmap CutBitmap(Bitmap b, int StartX, int StartY, int iWidth, int iHeight)
        {
            if (b == null)
            {
                return null;
            }

            int w = b.Width;
            int h = b.Height;

            if (StartX >= w || StartY >= h)
            {
                return null;
            }

            if (StartX + iWidth > w)
            {
                iWidth = w - StartX;
            }

            if (StartY + iHeight > h)
            {
                iHeight = h - StartY;
            }

            Bitmap bmpOut = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);

            Graphics g = Graphics.FromImage(bmpOut);
            g.DrawImage(b, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(StartX, StartY, iWidth, iHeight), GraphicsUnit.Pixel);
            g.Dispose();

            return bmpOut;
        }

        private static bool InvokeRequired
        {
            get { return Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
        }

        private static BitmapSource CreateBitmapSourceFromBitmap(Stream stream)
        {
            BitmapDecoder bitmapDecoder = BitmapDecoder.Create(
                stream,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.OnLoad);

            // This will disconnect the stream from the image completely...
            WriteableBitmap writable = new WriteableBitmap(bitmapDecoder.Frames.Single());
            writable.Freeze();

            return writable;
        }

        /// <summary>
        /// Save image as file from WriteableBitmap
        /// </summary>
        /// <param name="wtbBmp">WriteableBitmap</param>
        /// <param name="path">full path with file name</param>
        public static void SaveWriteableBitmap(WriteableBitmap wtbBmp, string path)
        {
            if (wtbBmp == null)
            {
                return;
            }
            RenderTargetBitmap rtbitmap = new RenderTargetBitmap(wtbBmp.PixelWidth, wtbBmp.PixelHeight, wtbBmp.DpiX, wtbBmp.DpiY, System.Windows.Media.PixelFormats.Default);
            System.Windows.Media.DrawingVisual drawingVisual = new System.Windows.Media.DrawingVisual();
            using (var dc = drawingVisual.RenderOpen())
            {
                dc.DrawImage(wtbBmp, new Rect(0, 0, wtbBmp.Width, wtbBmp.Height));
            }
            rtbitmap.Render(drawingVisual);
            JpegBitmapEncoder bitmapEncoder = new JpegBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(rtbitmap));
            //string strDir = @"Images\";
            //string strpath = strDir + DateTime.Now.ToString("yyyyMMddfff") + ".jpg";
            if (!File.Exists(path))
            {
                MemoryStream ms = new MemoryStream();
                //bitmapEncoder.Save(File.OpenWrite(path));
                bitmapEncoder.Save(ms);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(ms);
                bitmap.Save(path);
            }
        }
    }
}
