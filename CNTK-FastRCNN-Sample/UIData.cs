using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CNTK_FastRCNN_Sample
{
    class UIData : INotifyPropertyChanged
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
    }
}
