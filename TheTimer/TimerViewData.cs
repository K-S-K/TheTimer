using System;
using System.ComponentModel;

namespace TheTimer
{
    public class TimerViewData : INotifyPropertyChanged
    {
        #region -> Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region -> Data
        private TimeSpan _duration;
        private System.Windows.Media.Brush _stringColor;
        #endregion


        #region -> Properties
        public TimeSpan Duration
        {
            get { return _duration; }
            set
            {
                if (_duration == value)
                {
                    return;
                }

                _duration = value;
                UpdateProperty("Duration");
                UpdateProperty("DurationString");

                if (_duration < TimeSpan.FromMinutes(1))
                {
                    StringColor = System.Windows.Media.Brushes.OrangeRed;
                }
                else if (_duration == TimeSpan.Zero)
                {
                    StringColor = System.Windows.Media.Brushes.Red;
                }
                else
                {
                    StringColor = System.Windows.Media.Brushes.Black;
                }
            }
        }

        public string DurationString => Duration.ToString(Duration.Hours > 0 ? @"hh\:mm\:ss" : @"mm\:ss");

        public System.Windows.Media.Brush StringColor
        {
            get { return _stringColor; }
            set
            {
                if (_stringColor == value)
                {
                    return;
                }

                _stringColor = value;
                UpdateProperty("StringColor");
            }
        }
        #endregion


        #region -> Methods
        private void UpdateProperty(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion


        #region -> Ctor
        public TimerViewData()
        {
            _stringColor = System.Windows.Media.Brushes.Black;
        }
        #endregion    
    }
}
