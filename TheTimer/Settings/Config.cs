﻿using System;
using Microsoft.Win32;
using System.Xml.Linq;
using System.ComponentModel;

namespace TheTimer.Settings
{
    public class Config : INotifyPropertyChanged
    {
        #region -> Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion


        #region -> Data
        private string _originalContent;

        private bool _trueFullScreenMode;
        private bool _darkColorScheme;
        #endregion


        #region -> Properties
        public bool TrueFullScreenMode
        {
            get { return _trueFullScreenMode; }
            set
            {
                if (_trueFullScreenMode == value)
                {
                    return;
                }

                _trueFullScreenMode = value;
                UpdateProperty(nameof(Modifyed));
                UpdateProperty(nameof(TrueFullScreenMode));
            }
        }

        public bool DarkColorScheme
        {
            get { return _darkColorScheme; }
            set
            {
                if (_darkColorScheme == value)
                {
                    return;
                }

                _darkColorScheme = value;
                UpdateProperty(nameof(Modifyed));
                UpdateProperty(nameof(DarkColorScheme));
            }
        }

        public bool Modifyed
        {
            get { return _originalContent != XContent.ToString(); }
            set
            {
                if (value == false)
                {
                    _originalContent = XContent.ToString();
                    UpdateProperty(nameof(Modifyed));
                }
            }
        }
        #endregion


        #region -> Methods
        public override string ToString() => Modifyed ? "Modifyed" : "Saved";

        public void SaveProgramConfigToRegistry()
        {
            RegistryKey rk;

            {
                rk = GetRegistryKey("Behavior");
                rk.SetValue(nameof(TrueFullScreenMode), TrueFullScreenMode);
            }

            {
                rk = GetRegistryKey("Looking");
                rk.SetValue(nameof(DarkColorScheme), DarkColorScheme);
            }

            {
                // rk = GetRegistryKey("Another");
                // rk.SetValue("DBConnection", DB.XContent.ToString());
            }

            Modifyed = false;
        }

        public void LoadProgramConfigFromRegistry()
        {
            RegistryKey rk;

            // rk= GetRegistryKey("Another");
            // if (DB != null)
            //     DB.XContent = XElement.Parse(rk.GetValue("DBConnection",
            //         @"<DB SRV=""(local)"" DBN=""ATS_SBRF_2016"" UID="""" PWD="""" WIN=""true"" />").ToString());

            {
                rk = GetRegistryKey("Behavior");
                TrueFullScreenMode = bool.Parse(rk.GetValue(nameof(TrueFullScreenMode), true).ToString());
            }

            {
                rk = GetRegistryKey("Looking");
                DarkColorScheme = bool.Parse(rk.GetValue(nameof(DarkColorScheme), false).ToString());
            }

            Modifyed = false;
        }
        #endregion



        #region -> Implementation
        public void UpdateProperty(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion



        #region -> XML deal
        public XElement XContent
        {
            get
            {
                XElement xe = new XElement("Configuration");

                XElement xeBH = new XElement("Behavior"); xe.Add(xeBH);
                xeBH.Add(new XElement(nameof(TrueFullScreenMode), TrueFullScreenMode));

                XElement xeLK = new XElement("Looking"); xe.Add(xeLK);
                xeLK.Add(new XElement(nameof(DarkColorScheme), DarkColorScheme));

                return xe;
            }
            set
            {
                if (value == null) return;
                if (value.Name != "Configuration") return;

                XElement xeBH = value.Element("Behavior"); if (xeBH != null)
                {
                    try { TrueFullScreenMode = bool.Parse(xeBH.Element(nameof(TrueFullScreenMode)).Value); }
                    catch (Exception) { TrueFullScreenMode = true; }
                }

                XElement xeLK = value.Element("Looking"); if (xeBH != null)
                {
                    try { DarkColorScheme = bool.Parse(xeLK.Element(nameof(DarkColorScheme)).Value); }
                    catch (Exception) { DarkColorScheme = false; }
                }

                Modifyed = false;
            }
        }
        #endregion



        #region -> Registry deal
        private RegistryKey GetRegistryKey(string KeyName)
        {
            RegistryKey cu = Registry.CurrentUser;
            RegistryKey sw = cu.OpenSubKey("Software", RegistryKeyPermissionCheck.ReadWriteSubTree);
            RegistryKey ez = sw.CreateSubKey("KSKSW");
            RegistryKey hl = ez.CreateSubKey("The Timer");
            RegistryKey ky = hl.CreateSubKey(KeyName);
            return ky;
        }
        #endregion

        #region -> Ctor
        public Config() { }

        public Config(XElement xe) : this() { this.XContent = xe; }
        #endregion
    }
}
