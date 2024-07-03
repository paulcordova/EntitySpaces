﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using EntitySpaces.Common;
using EntitySpaces.MetadataEngine;

using EntitySpaces.AddIn.ES2024;

namespace EntitySpaces.AddIn
{
    public class esUserControl : UserControl
    {
        public esUserControl() { }

        private MainWindow mainWindow;

        public MainWindow MainWindow
        {
            get { return mainWindow;  }
            set 
            { 
                mainWindow = value; 
            }
        }

        public esSettings Settings
        {
            get 
            {
                if (mainWindow != null)
                {
                    return mainWindow.Settings;
                }
                else
                {
                    return null;
                }
            }
        }

        virtual public void OnSettingsChanged()
        {

        }
    }
}
