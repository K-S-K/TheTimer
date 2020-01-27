using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace TheTimer
{
    /// <summary>
    /// Interaction logic for DlgSet.xaml
    /// </summary>
    public partial class DlgSet : Window
    {
        public DlgSet()
        {
            InitializeComponent();

            Loaded += OnDlgLoaded;
        }

        private void OnDlgLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is DataToSet data)
            {
                TimeTextBox.Text = data.Duration.ToString(@"hh\:mm\:ss");
            }
        }

        private void OnBtnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OnBtnApply(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
