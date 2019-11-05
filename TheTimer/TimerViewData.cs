using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTimer
{
    public class TimerViewData : INotifyPropertyChanged
    {
        #region -> Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region -> Data
        private TimeSpan _elapsed;
        #endregion


        #region -> Properties
        public TimeSpan Duration
        {
            get { return _elapsed; }
            set
            {
                if (_elapsed == value)
                {
                    return;
                }

                _elapsed = value;
                UpdateProperty("Duration");
                UpdateProperty("DurationString");
            }
        }

        public string DurationString => Duration.ToString(Duration.Hours > 0 ? @"hh\:mm\:ss" : @"mm\:ss");
        #endregion


        #region -> Methods
        private void UpdateProperty(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion



    }
}
