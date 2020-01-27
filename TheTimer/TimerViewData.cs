using System;
using System.Windows.Media;
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

        private Brush _stringColor;
        private Brush _borderColor;
        private Brush _wingowBackground;
        private Brush _wingowForeground;
        private Color _interactiveGradientBeg;
        private Color _interactiveGradientEnd;

        private Color _mainWindowGradientEnd;
        private Color _mainWindowGradientBeg;

        private bool IsDarkPalette;
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
                UpdateProperty(nameof(Duration));
                UpdateProperty(nameof(DurationString));

                UpdateStringColor();
            }
        }

        public string DurationString => Duration.ToString(Duration.Hours > 0 ? @"hh\:mm\:ss" : @"mm\:ss");

        public Brush StringColor
        {
            get { return _stringColor; }
            set
            {
                if (_stringColor == value)
                {
                    return;
                }

                _stringColor = value;
                UpdateProperty(nameof(StringColor));
            }
        }

        public Brush BorderColor
        {
            get { return _borderColor; }
            set
            {
                if (_borderColor == value)
                {
                    return;
                }

                _borderColor = value;
                UpdateProperty(nameof(BorderColor));
            }
        }

        public Brush WingowBackground
        {
            get { return _wingowBackground; }
            set
            {
                if (_wingowBackground == value)
                {
                    return;
                }

                _wingowBackground = value;
                UpdateProperty(nameof(WingowBackground));
            }
        }


        public Brush WingowForeground
        {
            get { return _wingowForeground; }
            set
            {
                if (_wingowForeground == value)
                {
                    return;
                }

                _wingowForeground = value;
                UpdateProperty(nameof(WingowForeground));
            }
        }

        public Color MainWindowGradientEnd
        {
            get { return _mainWindowGradientEnd; }
            set
            {
                if (_mainWindowGradientEnd == value)
                {
                    return;
                }

                _mainWindowGradientEnd = value;
                UpdateProperty(nameof(MainWindowGradientEnd));
            }
        }

        public Color MainWindowGradientBeg
        {
            get { return _mainWindowGradientBeg; }
            set
            {
                if (_mainWindowGradientBeg == value)
                {
                    return;
                }

                _mainWindowGradientBeg = value;
                UpdateProperty(nameof(MainWindowGradientBeg));
            }
        }


        public Color InteractiveGradientBeg
        {
            get { return _interactiveGradientBeg; }
            set
            {
                if (_interactiveGradientBeg == value)
                {
                    return;
                }

                _interactiveGradientBeg = value;
                UpdateProperty(nameof(InteractiveGradientBeg));
            }
        }


        public Color InteractiveGradientEnd
        {
            get { return _interactiveGradientEnd; }
            set
            {
                if (_interactiveGradientEnd == value)
                {
                    return;
                }

                _interactiveGradientEnd = value;
                UpdateProperty(nameof(InteractiveGradientEnd));
            }
        }

        #endregion


        #region -> Methods
        public void UpdatePaletteProperties()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BorderColor)));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WingowBackground)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WingowForeground)));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InteractiveGradientBeg)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InteractiveGradientEnd)));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MainWindowGradientBeg)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MainWindowGradientEnd)));
        }

        public void SetPalette(bool dark)
        {
            IsDarkPalette = dark;

            // TODO: https://stackoverflow.com/questions/3173364/white-border-around-groupbox
            // TODO: https://stackoverflow.com/questions/1283006/changing-wpf-title-bar-background-color

            if (dark)
            {
                _borderColor = Brushes.Gray;

                _interactiveGradientBeg = Colors.Black;
                // _interactiveGradientEnd = Colors.DarkSlateGray;
                _interactiveGradientEnd = Color.FromArgb(128, 0x66, 0x66, 0x66);

                _mainWindowGradientEnd = Colors.Black;
                _mainWindowGradientBeg = Colors.Black;

                _wingowBackground = Brushes.Black;
                _wingowForeground = Brushes.Yellow;
            }
            else
            {
                _borderColor = Brushes.Black;

                _interactiveGradientBeg = Colors.LightGray;
                _interactiveGradientEnd = Colors.White;

                _mainWindowGradientEnd = Colors.White;
                _mainWindowGradientBeg = Colors.White;

                _wingowBackground = Brushes.White;
                _wingowForeground = Brushes.Black;
            }

            UpdateStringColor();
            UpdatePaletteProperties();
        }
        #endregion


        #region -> Implementation
        private void UpdateProperty(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void UpdateStringColor()
        {
            if (_duration < TimeSpan.FromSeconds(1))
            {
                StringColor = Brushes.Red;
            }
            else if (_duration < TimeSpan.FromMinutes(1))
            {
                StringColor = Brushes.OrangeRed;
            }
            else
            {
                StringColor = IsDarkPalette ?
                    Brushes.YellowGreen :
                    Brushes.Black;
            }
        }
        #endregion


        #region -> Ctor
        public TimerViewData()
        {
            _interactiveGradientBeg = Colors.LightGray;
            _interactiveGradientEnd = Colors.White;

            _mainWindowGradientEnd = Colors.LightGray;
            _mainWindowGradientBeg = Colors.White;

            _borderColor = Brushes.LightGray;
            _stringColor = Brushes.Black;

            _wingowBackground = Brushes.Black;
            _wingowForeground = Brushes.Yellow;
        }
        #endregion    
    }
}
