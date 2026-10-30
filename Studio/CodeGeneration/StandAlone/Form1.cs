using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

using EntitySpaces.AddIn;
using EntitySpaces.Common;
using Krypton.Toolkit;

namespace EntitySpaces
{
    public partial class Form1 : KryptonForm, IThemeable
    {
        public Form1()
        {
            InitializeComponent();

            this.Text = $"{VersionInfo.ProductName} - Persistence Layer and Business Objects for Microsoft .NET";

            // Initialize theme logic
            LoadSavedTheme();
            BuildThemeMenu();
            ApplyTheme();

            // Force all controls to refresh their theme on startup
            RefreshControls(this);

            // Fires once per app launch. Silent, async, never blocks startup.
            TelemetryHelper.PingLaunch();
        }

        #region Original Application Logic

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutBox aboutBox = new AboutBox())
            {
                aboutBox.ShowDialog();
            }
        }

        private void feedBackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FeedbackBox feedbackBox = new FeedbackBox())
            {
                feedbackBox.ShowDialog();
            }
        }

        #endregion

        #region Theme Management (IThemeable Implementation)

        // Implement the interface method
        public void ApplyTheme()
        {
            KryptonColors();
        }

        private void LoadSavedTheme()
        {
            // Retrieve the saved theme string from settings
            string savedTheme = Properties.Settings.Default.Theme;

            // Try to parse the string back into our AppThemeMode enum
            if (!string.IsNullOrEmpty(savedTheme) && Enum.TryParse(savedTheme, out AppThemeMode mode))
            {
                AppTheme.CurrentMode = mode;
            }
            else
            {
                // Fallback to default if something goes wrong
                AppTheme.CurrentMode = AppThemeMode.EntitySpaces;
            }

            // Apply the Krypton palette globally before building the UI
            AppTheme.ApplyKryptonGlobalTheme();
        }

        private void ChangeTheme(AppThemeMode mode)
        {
            AppTheme.CurrentMode = mode;
            AppTheme.ApplyKryptonGlobalTheme();

            // Save the selected theme to user settings
            Properties.Settings.Default.Theme = mode.ToString();
            Properties.Settings.Default.Save();

            // Notify all open forms/controls to refresh their colors
            RefreshAllForms();
        }

        private void KryptonColors()
        {
            // Get base colors from AppTheme
            Color bgColor = AppTheme.GetBackgroundColor();
            Color fgColor = AppTheme.GetForegroundColor();

            // Define the highlight color based on the current theme
            Color selectionColor;
            if (AppTheme.CurrentMode == AppThemeMode.Dark)
            {
                selectionColor = Color.FromArgb(75, 75, 75); // Dark gray
            }
            else
            {
                selectionColor = Color.FromArgb(200, 220, 240); // Light blue
            }

            // Instantiate the custom renderer
            var customRenderer = new KryptonMenuRenderer(bgColor, selectionColor, fgColor);

            // Apply to main MenuStrip
            this.menuStrip1.Renderer = customRenderer;
            this.menuStrip1.BackColor = bgColor;
            this.menuStrip1.ForeColor = fgColor;

            // Apply recursively to items, passing the renderer
            ApplyMenuColors(this.menuStrip1.Items, bgColor, fgColor, customRenderer);
        }

        #endregion

        #region Theme Menu Construction & Rendering

        private void BuildThemeMenu()
        {
            // 1. Create the "Config" main menu
            ToolStripMenuItem configMenu = new ToolStripMenuItem("Config");
            configMenu.Name = "configToolStripMenuItem";

            // 2. Create the "Theme" submenu
            ToolStripMenuItem themeMenu = new ToolStripMenuItem("Theme");
            themeMenu.Name = "themeToolStripMenuItem";

            // 3. Create the Theme Options and wire up existing events
            ToolStripMenuItem standardTheme = new ToolStripMenuItem("Standard (Light)");
            standardTheme.Name = "standardThemeToolStripMenuItem";
            standardTheme.Click += this.standardThemeToolStripMenuItem_Click;

            ToolStripMenuItem entitySpacesTheme = new ToolStripMenuItem("EntitySpaces (Blue)");
            entitySpacesTheme.Name = "entitySpacesThemeToolStripMenuItem";
            entitySpacesTheme.Click += this.entitySpacesThemeToolStripMenuItem_Click;

            ToolStripMenuItem darkTheme = new ToolStripMenuItem("Dark (Graphite)");
            darkTheme.Name = "darkThemeToolStripMenuItem";
            darkTheme.Click += this.darkThemeToolStripMenuItem_Click;

            // 4. Assemble the hierarchy
            themeMenu.DropDownItems.Add(standardTheme);
            themeMenu.DropDownItems.Add(entitySpacesTheme);
            themeMenu.DropDownItems.Add(darkTheme);

            configMenu.DropDownItems.Add(themeMenu);

            // 5. Add the Config menu to the MenuStrip
            this.menuStrip1.Items.Add(configMenu);
        }

        private void ApplyMenuColors(ToolStripItemCollection items, Color bgColor, Color fgColor, ToolStripRenderer renderer)
        {
            foreach (ToolStripItem item in items)
            {
                item.BackColor = bgColor;
                item.ForeColor = fgColor;

                if (item is ToolStripMenuItem menuItem)
                {
                    // CRITICAL: Apply the custom renderer to the dropdown itself
                    menuItem.DropDown.BackColor = bgColor;
                    menuItem.DropDown.ForeColor = fgColor;
                    menuItem.DropDown.Renderer = renderer;

                    if (menuItem.HasDropDownItems)
                    {
                        ApplyMenuColors(menuItem.DropDownItems, bgColor, fgColor, renderer);
                    }
                }
            }
        }

        private void standardThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeTheme(AppThemeMode.Standard);
        }

        private void entitySpacesThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeTheme(AppThemeMode.EntitySpaces);
        }

        private void darkThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeTheme(AppThemeMode.Dark);
        }

        #endregion

        #region Helpers (Form Refresh)

        private void RefreshAllForms()
        {
            // 1. Iterate through all open Forms
            foreach (Form form in Application.OpenForms)
            {
                // If the form implements the interface, apply theme
                if (form is IThemeable themeableForm)
                {
                    themeableForm.ApplyTheme();
                }

                // 2. Recursively search for UserControls inside the form
                RefreshControls(form);
            }
        }

        // Helper method to traverse the control hierarchy
        private void RefreshControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                // If the control implements the interface, apply theme
                if (control is IThemeable themeableControl)
                {
                    themeableControl.ApplyTheme();
                }

                // If the control has children, go deeper
                if (control.HasChildren)
                {
                    RefreshControls(control);
                }
            }
        }

        #endregion

    } // end class
}