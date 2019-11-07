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

namespace TheTimer.Settings
{
    /// <summary>
    /// Interaction logic for ConfigDlg.xaml
    /// </summary>
    public partial class ConfigDlg : Window
    {
        #region -> Properties
        public readonly Config Config;
        #endregion


        #region -> Methods
        private void OnBtnApp(object sender, RoutedEventArgs e)
        {
            //  Config.XContent = (DataContext as Config).XContent;
            DialogResult = true;
            Close();
        }

        private void OnBtnEsc(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Config.Modifyed)
                {
                    OnBtnApp(sender, e);
                }
            }
        }
        #endregion


        #region -> Ctor
        public ConfigDlg()
        {
            InitializeComponent();
            this.PreviewKeyDown += HandleKeyDown;
        }

        public ConfigDlg(Config cfg) : this()
        {
            Config = new Config(cfg.XContent);
            DataContext = Config;
        }
        #endregion   
    }
}
