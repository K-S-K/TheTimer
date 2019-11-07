using System;
using System.Windows;
using System.Threading;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using TheTimer.Timer;
using TheTimer.Settings;

namespace TheTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region -> Data
        private bool IsFullScreenNow;

        private object sync_sw;
        private Style styleHover;
        private Style styleLeave;
        private GroupBox gbSelected;
        private GroupBox gbFocused;
        private Stopwatch swBack;

        private object syncSel;

        private DataToSet _timerSet;
        private TimerItself _timerItem;
        private TimerViewData _timerViewData;

        private readonly Config _config;
        #endregion


        #region -> MainView Content Selector

        #region -> Events handlers
        private void OnBtnClick(object sender, RoutedEventArgs e)
        {
            lock (sync_sw)
            {
                if (e.Source is GroupBox gb)
                {
                    OnBtnClick(gb);
                }

                if (e.Source is Image img)
                {
                    OnBtnClick(img);
                }

                if (e.Source == txTimer)
                {
                    ToggleSurroundVisability();
                }
            }
        }

        private void OnBtnMouseEnter(object sender, RoutedEventArgs e)
        {
            lock (sync_sw)
            {
                if (e.Source is GroupBox gb)
                {
                    if (gb != wndMainView)
                    {
                        gb.Style = styleHover;
                    }

                    ShowSelected(gb);

                    BackWaitingCancel();
                }
            }
        }

        private void OnBtnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            lock (sync_sw)
            {
                if (e.Source is GroupBox gb)
                {
                    gb.Style = styleLeave;

                    if (gbSelected != null
                        &&
                        gbFocused != null)
                    {
                        if (gbFocused == gbSelected)
                        {
                            BackWaitingCancel();
                        }
                        else
                        {
                            BackWaitingBegin();
                        }
                    }
                    sbiStateName.Text = string.Empty;
                }
            }
        }

        private void OnBtnSelect(object sender, RoutedEventArgs e)
        {
            lock (sync_sw)
            {
                if (e.Source is RadioButton rb)
                {
                    if (rb.Parent is GroupBox gb)
                    {
                        gbSelected = gb;
                        if (gb == btnRun)
                        {
                            OnBtnPlay();
                            imgRun.Source = new BitmapImage(new Uri("Images/Btn_Run_Red.png", UriKind.Relative));
                            ImgPau.Source = new BitmapImage(new Uri("Images/Btn_Pau_Gray.png", UriKind.Relative));
                            ImgStp.Source = new BitmapImage(new Uri("Images/Btn_Stp_Gray.png", UriKind.Relative));
                        }
                        if (gb == btnPau) { OnBtnPause(); }
                        if (gb == btnStp) { OnBtnStop(); }
                    }
                }
            }
        }
        #endregion


        #region -> Visibility control
        private void BackWaitingBegin()
        {
            swBack.Reset();
            swBack.Start();
        }

        private void BackWaitingCancel()
        {
            swBack.Reset();
        }

        private void BackWaitingComplete()
        {
            swBack.Reset();

            Application.Current.Dispatcher.Invoke(() =>
            {
                ShowSelected(gbSelected);
            });
        }

        private void BackWaitingWatchDog()
        {
            TimeSpan backTime = TimeSpan.FromMilliseconds(500);
            while (true)
            {
                Thread.Sleep(100);
                if (!swBack.IsRunning) continue;
                if (swBack.Elapsed < backTime) continue;

                BackWaitingComplete();

                Dispatcher.Invoke((Action)delegate
                {
                    sbiStateName.Text = string.Empty;
                });
            }
        }


        private void OnBtnClick(Image img)
        {
            // if (img == null) 
            return;

            if (img == imgRun) { OnBtnClick(btnRun); }
            if (img == ImgPau) { OnBtnClick(btnPau); }
            if (img == ImgStp) { OnBtnClick(btnStp); }
            // if (img == ImgRst) { OnBtnClick(btnRst); }
        }


        private void OnBtnClick(GroupBox gb)
        {
            if (gb == null) return;

            if (gb == wndMainView)
            {
                if (gbSelected != null)
                {
                    gb = gbSelected;
                }
            }

            if (gb == btnRun) { OnBtnPlay(); }
            if (gb == btnPau) { OnBtnPause(); }
            if (gb == btnStp) { OnBtnStop(); }
            if (gb == btnRst) { OnBtnRst(); }
            if (gb == btnSet) { OnBtnSet(); }
            if (gb == btnCfg) { OnBtnCfg(); }
            if (gb == btnEsc) { OnBtnEsc(); }

            gbFocused = gb;
        }

        private void ToggleSurroundVisability()
        {
            bool bShow = sbMain.Visibility != Visibility.Visible;

            ToggleSurroundVisability(bShow);
        }

        private void ToggleSurroundVisability(bool bShow)
        {
            sbMain.Visibility = bShow ? Visibility.Visible : Visibility.Collapsed;

            firstColumn.Width = new GridLength(bShow ? 80 : 0);
            this.WindowStyle = bShow ? WindowStyle.SingleBorderWindow : WindowStyle.None;

            if (_config.TrueFullScreenMode)
            {
                this.WindowState = bShow ? WindowState.Normal : WindowState.Maximized;

                if (bShow)
                {
                    Topmost = false;
                }
                else
                {
                    Topmost = true;
                    IsFullScreenNow = true;
                }
            }

            if (bShow)
            {
                IsFullScreenNow = false;
            }
        }

        private void OnBtnPause()
        {
            _timerItem.Pause();
        }

        private void OnBtnPlay()
        {
            _timerItem.Start();
        }

        private void OnBtnStop()
        {
            _timerItem.Stop();
        }

        private void OnBtnRst()
        {
            if (_timerItem.State == TimerState.Running)
            {
                MessageBoxResult userDecision = MessageBox.Show(this,
                  $"Do you wish to{Environment.NewLine}restart The Timer?", "Think about it",
                  MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (userDecision == MessageBoxResult.No) { return; }
            }

            _timerItem.Restart();
        }

        private void OnBtnSet()
        {
            Window wnd = new DlgSet()
            {
                DataContext = new DataToSet(_timerSet)
            };

            if (wnd.ShowDialog() == false) { return; }
            else
            {
                _timerSet.Duration = (wnd.DataContext as DataToSet).Duration;
                ConfigureTimerItem();
            }
        }

        private void ConfigureTimerItem()
        {
            _timerItem.TimeToWait = _timerSet.Duration;
        }

        private void OnBtnCfg()
        {
            ConfigDlg dlg = new ConfigDlg(_config) { Owner = this };

            bool? DR = dlg.ShowDialog();

            if (DR == null || !DR.Value) { return; }

            _config.XContent = dlg.Config.XContent;
            _config.SaveProgramConfigToRegistry();
        }

        private void OnBtnEsc()
        {
            Application.Current.Shutdown();
        }

        private void ShowSelected(GroupBox gb)
        {
            if (gb == null) return;

            if (gb == wndMainView)
            {
                if (gbSelected != null)
                {
                    gb = gbSelected;
                }
            }

            if (gbFocused != gb)
            {
                if (gb == btnRun) sbiStateName.Text = TimerState.Running.Description();
                if (gb == btnPau) sbiStateName.Text = TimerState.Paused.Description();
                if (gb == btnStp) sbiStateName.Text = TimerState.Stopped.Description();
                if (gb == btnRst) sbiStateName.Text = "Restart The Timer awaiting again";
                if (gb == btnSet) sbiStateName.Text = "Set The Timer duration time";
                if (gb == btnCfg) sbiStateName.Text = "Configure The Timer";

                gbFocused = gb;
            }
        }
        #endregion

        #endregion


        #region -> Preparations
        private Style LoadStaticWindowResourceStyle(string styleName)
        {
            if (FindResource(styleName) is Style style)
            {
                if (style != null)
                {
                    return style;
                }
            }

            string msg = string.Format(
                "Cannot find style resource {0}\"{1}\".{0}{0}" +
                "Application will be closed right now",
                Environment.NewLine, styleName);

            MessageBox.Show(this, msg, "Something goes wrong",
                MessageBoxButton.OK, MessageBoxImage.Exclamation);

            Close();

            return null;
        }
        #endregion


        #region -> Table content selection ovrd
        private void OnDataGridCellSelected(object sender, RoutedEventArgs e)
        {
            DataGridRow dgr = null;
            DataGrid grd = null;

            if (sender is DataGridCell dgc)
            {
                var parent = VisualTreeHelper.GetParent(dgc);

                #region Find row
                while (parent != null)
                {
                    if (parent.GetType() == typeof(DataGridRow))
                    {
                        dgr = parent as DataGridRow;
                        break;
                    }
                    parent = VisualTreeHelper.GetParent(parent);
                }
                #endregion

                #region Find grid
                while (parent != null)
                {
                    if (parent.GetType() == typeof(DataGrid))
                    {
                        grd = parent as DataGrid;
                        break;
                    }
                    parent = VisualTreeHelper.GetParent(parent);
                }
                #endregion

                var item = grd.SelectedItem;
            }
        }

        private void OnDataGridRowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGrid grd = null;
            DataGridRow dgr = sender as DataGridRow;


            #region Find grid
            var parent = VisualTreeHelper.GetParent(dgr);
            while (parent != null && parent.GetType() != typeof(DataGridRow))
            {
                if (parent.GetType() == typeof(DataGrid))
                {
                    grd = parent as DataGrid;
                    break;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            #endregion

            dgr.IsSelected = true;
            dgr.Focus();
            // grd.Focus();
            FocusManager.SetFocusedElement(dgr, null);
        }
        #endregion


        #region -> Ctor
        public MainWindow()
        {
            _config = new Config();
            _config.LoadProgramConfigFromRegistry();


            _timerSet = new DataToSet();
            _timerItem = new TimerItself();
            _timerViewData = new TimerViewData();

            _timerItem.TimeChanged += (duration) => { _timerViewData.Duration = duration; };
            _timerItem.StateChanged += (stOld, stNew) =>
            {
                Dispatcher.Invoke((Action)delegate
                {
                    // wndMainView.Header = stNew.ToString();
                    txtHdr.Text = stNew.ToString();

                    switch (stNew)
                    {
                        case TimerState.Complete:
                            imgRun.Source = new BitmapImage(new Uri("Images/Btn_Run_Gray.png", UriKind.Relative));
                            ImgPau.Source = new BitmapImage(new Uri("Images/Btn_Pau_Gray.png", UriKind.Relative));
                            ImgStp.Source = new BitmapImage(new Uri("Images/Btn_Stp_Gray.png", UriKind.Relative));
                            imgHdr.Source = null;
                            // chkRun.IsChecked = false;
                            break;

                        case TimerState.Running:
                            imgRun.Source = new BitmapImage(new Uri("Images/Btn_Run_Blue.png", UriKind.Relative));
                            ImgPau.Source = new BitmapImage(new Uri("Images/Btn_Pau_Gray.png", UriKind.Relative));
                            ImgStp.Source = new BitmapImage(new Uri("Images/Btn_Stp_Gray.png", UriKind.Relative));
                            imgHdr.Source = new BitmapImage(new Uri("Images/Btn_Run_Green.png", UriKind.Relative));
                            // chkRun.IsChecked = true;
                            break;

                        case TimerState.Paused:
                            imgRun.Source = new BitmapImage(new Uri("Images/Btn_Run_Gray.png", UriKind.Relative));
                            ImgPau.Source = new BitmapImage(new Uri("Images/Btn_Pau_Blue.png", UriKind.Relative));
                            ImgStp.Source = new BitmapImage(new Uri("Images/Btn_Stp_Gray.png", UriKind.Relative));
                            imgHdr.Source = new BitmapImage(new Uri("Images/Btn_Pau_Green.png", UriKind.Relative));
                            // chkPau.IsChecked = true;
                            break;

                        case TimerState.Stopped:
                            imgRun.Source = new BitmapImage(new Uri("Images/Btn_Run_Gray.png", UriKind.Relative));
                            ImgPau.Source = new BitmapImage(new Uri("Images/Btn_Pau_Gray.png", UriKind.Relative));
                            ImgStp.Source = new BitmapImage(new Uri("Images/Btn_Stp_Blue.png", UriKind.Relative));
                            imgHdr.Source = new BitmapImage(new Uri("Images/Btn_Stp_Green.png", UriKind.Relative));
                            // chkStp.IsChecked = true;
                            break;
                    }

                });
            };

            sync_sw = new object();
            syncSel = new object();
            swBack = new Stopwatch();

            InitializeComponent();

            DataContext = _timerViewData;

            styleHover = LoadStaticWindowResourceStyle("MainViewContentSelector_Hover");
            styleLeave = LoadStaticWindowResourceStyle("MainViewContentSelector_Leave");


            if (styleHover == null)
            {
                MessageBox.Show(this, "text", "caption", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
            }

            btnRun.MouseEnter += OnBtnMouseEnter;
            btnPau.MouseEnter += OnBtnMouseEnter;
            btnStp.MouseEnter += OnBtnMouseEnter;
            btnRst.MouseEnter += OnBtnMouseEnter;
            btnSet.MouseEnter += OnBtnMouseEnter;
            btnCfg.MouseEnter += OnBtnMouseEnter;
            btnEsc.MouseEnter += OnBtnMouseEnter;
            wndMainView.MouseEnter += OnBtnMouseEnter; // Enter only

            btnRun.MouseLeave += OnBtnMouseLeave;
            btnPau.MouseLeave += OnBtnMouseLeave;
            btnStp.MouseLeave += OnBtnMouseLeave;
            btnRst.MouseLeave += OnBtnMouseLeave;
            btnSet.MouseLeave += OnBtnMouseLeave;
            btnCfg.MouseLeave += OnBtnMouseLeave;
            btnEsc.MouseLeave += OnBtnMouseLeave;

            // chkPau.Checked += OnBtnSelect;
            // chkRun.Checked += OnBtnSelect;
            // chkStp.Checked += OnBtnSelect;

            btnRun.MouseLeftButtonDown += OnBtnClick;
            btnPau.MouseLeftButtonDown += OnBtnClick;
            btnStp.MouseLeftButtonDown += OnBtnClick;
            btnRst.MouseLeftButtonDown += OnBtnClick;
            btnSet.MouseLeftButtonDown += OnBtnClick;
            btnCfg.MouseLeftButtonDown += OnBtnClick;
            btnEsc.MouseLeftButtonDown += OnBtnClick;
            imgRun.MouseLeftButtonDown += (s, e) => { OnBtnClick(btnRun); };//OnBtnClick;
            ImgPau.MouseLeftButtonDown += (s, e) => { OnBtnClick(btnPau); };//OnBtnClick;
            ImgStp.MouseLeftButtonDown += (s, e) => { OnBtnClick(btnStp); };//OnBtnClick;
            ImgRst.MouseLeftButtonDown += (s, e) => { OnBtnClick(btnRst); };//OnBtnClick;
            ImgSet.MouseLeftButtonDown += (s, e) => { OnBtnClick(btnSet); };//OnBtnClick;
            ImgCfg.MouseLeftButtonDown += (s, e) => { OnBtnClick(btnCfg); };//OnBtnClick;
            ImgEsc.MouseLeftButtonDown += (s, e) => { OnBtnClick(btnEsc); };//OnBtnClick;
            wndMainView.MouseLeftButtonDown += WndMainView_MouseLeftButtonDown;

            new Thread(delegate () { BackWaitingWatchDog(); }) { IsBackground = true }.Start();

            Loaded += MainWindow_Loaded;
        }

        private void WndMainView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnBtnToggpePause();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ConfigureTimerItem();

            // wndMainView.Header = TimerState.Stopped.ToString();
            txtHdr.Text = TimerState.Stopped.ToString();
            sbiStateName.Text = string.Empty;

            this.KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ToggleSurroundVisability();
                return;
            }

            if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
            {
                switch (e.Key)
                {
                    case Key.M:
                        this.WindowState = WindowState.Maximized;
                        break;

                    case Key.N:
                        this.WindowState = WindowState.Normal;
                        break;

                    case Key.G:
                        OnBtnPlay();
                        break;

                    case Key.R:
                        OnBtnRst();
                        break;

                    case Key.P:
                    case Key.Space:
                        OnBtnToggpePause();
                        break;

                    case Key.S:
                        OnBtnStop();
                        break;

                    default:
                        break;
                }
            }
        }

        private void OnBtnToggpePause()
        {
            if (_timerItem.State == TimerState.Running)
            {
                OnBtnPause();
            }
            else
            {
                OnBtnPlay();
            }
        }
        #endregion
    }
}
