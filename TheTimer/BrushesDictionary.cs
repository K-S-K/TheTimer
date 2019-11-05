using System;
using System.Windows.Media;
using System.Collections.Generic;

namespace TheTimer
{
    /// <summary>
    /// Simple Thread safe solution.
    /// </summary>
    public class BrushesDictionary
    {
        #region -> Data
        private static BrushesDictionary _instance;

        private SolidColorBrush _lastRegisteredBrush;
        private Dictionary<string, SolidColorBrush> _index;

        private static readonly object _lock = new object();
        private readonly SolidColorBrush _brushOne = Brushes.Lavender;
        private readonly SolidColorBrush _brushTwo = Brushes.LightGoldenrodYellow;
        #endregion


        #region -> Methods
        public SolidColorBrush GetBrush(string filterName)
        {
            lock (_lock)
            {
                if (!_index.TryGetValue(filterName, out SolidColorBrush brush))
                {
                    brush =
                        _lastRegisteredBrush == _brushOne ? _brushTwo : _brushOne;
                    _index.Add(filterName, brush);
                }

                _lastRegisteredBrush = brush;
                return brush;
            }
        }
        #endregion


        #region -> Ctor
        private BrushesDictionary()
        {
            _index = new Dictionary<string, SolidColorBrush>();
            _lastRegisteredBrush = _brushOne;
        }

        public static BrushesDictionary Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new BrushesDictionary();
                    }
                    return _instance;
                }
            }
        }
        #endregion
    }
}
