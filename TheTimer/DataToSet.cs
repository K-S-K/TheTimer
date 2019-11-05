using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTimer
{
    public class DataToSet : INotifyPropertyChanged
    {
        #region -> Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region -> Data
        private TimeSpan _duration;

        public readonly string tsFMT = @"hh\:mm\:ss";
        public readonly CultureInfo SettingsCultureInfo = CultureInfo.InvariantCulture;
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
                HasChanges = true;
                UpdateProperty("HasChanges");
                UpdateProperty("Duration");
            }
        }

        public string DurationString
        {
            get { return Duration.ToString(tsFMT, SettingsCultureInfo); }
            set
            {
                Duration = TimeSpan.ParseExact(value, tsFMT, SettingsCultureInfo);
                UpdateProperty("DurationString");
            }
        }

        public bool HasChanges { get; private set; }
        #endregion


        #region -> Methods
        private void UpdateProperty(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion


        #region -> Ctor
        public DataToSet()
        {
            _duration = TimeSpan.FromMinutes(2);
        }
        public DataToSet(DataToSet other) : this()
        {
            this._duration = other._duration;
        }
        #endregion
    }
}
