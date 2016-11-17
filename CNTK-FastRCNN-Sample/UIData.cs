using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CNTK_FastRCNN_Sample
{
    public class UIData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private BitmapSource _UIImage;

        public BitmapSource UIImage
        {
            get { return _UIImage; }
            set
            {
                if (_UIImage != value)
                {
                    _UIImage = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("UIImage"));
                    }
                }
            }
        }

        private ImageSource _MouseFocusImage;

        public ImageSource MouseFocusImage
        {
            get { return _MouseFocusImage; }
            set
            {
                if (_MouseFocusImage != value)
                {
                    _MouseFocusImage = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("MouseFocusImage"));
                    }
                }
            }
        }

        private Visibility _MouseFocusImageVisibility = Visibility.Visible;

        public Visibility MouseFocusImageVisibility
        {
            get { return _MouseFocusImageVisibility; }
            set
            {
                if (_MouseFocusImageVisibility != value)
                {
                    _MouseFocusImageVisibility = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("MouseFocusImageVisibility"));
                    }
                }
            }
        }

        private ImageSource _BboxImage;

        public ImageSource BboxImage
        {
            get { return _BboxImage; }
            set
            {
                if (_BboxImage != value)
                {
                    _BboxImage = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("BboxImage"));
                    }
                }
            }
        }

        private bool _progressRing_IsActive = false;

        public bool progressRing_IsActive
        {
            get { return _progressRing_IsActive; }
            set
            {
                if (_progressRing_IsActive != value)
                {
                    _progressRing_IsActive = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("progressRing_IsActive"));
                    }
                }
            }
        }

        private string _TextMessage;

        public string TextMessage
        {
            get { return _TextMessage; }
            set
            {
                if (_TextMessage != value)
                {
                    _TextMessage = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("TextMessage"));
                    }
                }
            }
        }

        private Visibility _WarpPanel_ButtonVisibility = Visibility.Hidden;

        public Visibility WarpPanel_ButtonVisibility
        {
            get { return _WarpPanel_ButtonVisibility; }
            set
            {
                if (_WarpPanel_ButtonVisibility != value)
                {
                    _WarpPanel_ButtonVisibility = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("WarpPanel_ButtonVisibility"));
                    }
                }
            }
        }

        private int _LastCount;

        public int LastCount
        {
            get { return _LastCount; }
            set
            {
                if (_LastCount != value)
                {
                    _LastCount = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("LastCount"));
                    }
                }
            }
        }

        private bool _JPGFilter = true;

        public bool JPGFilter
        {
            get { return _JPGFilter; }
            set
            {
                if (_JPGFilter != value)
                {
                    _JPGFilter = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("JPGFilter"));
                    }
                }
            }
        }

        private bool _PNGFilter = false;

        public bool PNGFilter
        {
            get { return _PNGFilter; }
            set
            {
                if (_PNGFilter != value)
                {
                    _PNGFilter = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("PNGFilter"));
                    }
                }
            }
        }

        private bool _BMPFilter = false;

        public bool BMPFilter
        {
            get { return _BMPFilter; }
            set
            {
                if (_BMPFilter != value)
                {
                    _BMPFilter = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("BMPFilter"));
                    }
                }
            }
        }
    }
}
