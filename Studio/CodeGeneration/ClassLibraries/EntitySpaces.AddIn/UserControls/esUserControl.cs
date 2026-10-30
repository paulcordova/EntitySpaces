using EntitySpaces.AddIn.ES2025;
using EntitySpaces.Common;
using EntitySpaces.MetadataEngine;
using Krypton.Toolkit;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace EntitySpaces.AddIn
{
    // Implement the IThemeable interface
    public class esUserControl : UserControl, IThemeable
    {
        public esUserControl()
        {
        }

        // Override OnLoad to apply theme automatically when the control is ready
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
            {
                ApplyTheme();
            }
        }

        public virtual void ApplyTheme()
        {
            if (!this.DesignMode)
            {
                try
                {
                    this.BackColor = AppTheme.GetBackgroundColor();
                    this.ForeColor = AppTheme.GetForegroundColor();
                }
                catch { }
            }
        }


        private MainWindow mainWindow;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MainWindow MainWindow
        {
            get { return mainWindow; }
            set { mainWindow = value; }
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