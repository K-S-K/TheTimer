using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.Generic;

namespace TheTimer
{
    public class ValueToBrushConverter : IValueConverter
    {
        private enum ContentType { Unknown = 0, Balancer, Sessions }

        #region -> Documentation
        /// <summary>
        /// Color names
        /// http://blog.analogmachine.org/wp-content/uploads/2012/09/DelphiColorPalette.png
        /// 
        /// Change DataGrid cell colour based on values
        /// https://stackoverflow.com/questions/5549617/change-datagrid-cell-colour-based-on-values
        /// </summary>
        #endregion


        #region -> Data
        private readonly SolidColorBrush _brushOrd = SystemColors.WindowBrush;
        private readonly SolidColorBrush _brushSel = Brushes.Red;
        #endregion


        #region -> Methods
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string filterName = string.Empty;
            bool selected = false;
            ContentType type;

            try
            {
                object dataContext = null;

                if (value is DataGridCell dgc)
                {
                    dataContext = dgc.DataContext;
                }

                if (value is DataGridRow dgr)
                {
                    dataContext = dgr.DataContext;
                    selected = dgr.IsSelected;
                }

               // if (dataContext is BalancerLineItem item)
               // {
               //     filterName = item.Filter;
               // }

                type = ContentType.Balancer;
            }
            catch (InvalidCastException)
            {
                return DependencyProperty.UnsetValue;
            }

            if (string.IsNullOrEmpty(filterName))
            {
                return DependencyProperty.UnsetValue;
            }


            SolidColorBrush brush;

            switch (type)
            {
                case ContentType.Balancer:
                    brush = GetBrush(filterName);
                    break;

                default:
                    brush = selected ? _brushSel : _brushOrd;
                    break;
            }

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion


        #region -> Implementation
        private SolidColorBrush GetBrush(string filterName)
        {
            return BrushesDictionary.Instance.GetBrush(filterName);
        }
        #endregion


        #region -> Ctor
        public ValueToBrushConverter() { }
        #endregion
    }
}
