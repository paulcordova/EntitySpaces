using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using EntitySpaces.AddIn.TemplateUI;
using EntitySpaces.CodeGenerator;
using EntitySpaces.Common;
using EntitySpaces.MetadataEngine;

using EntitySpaces.AddIn.ES2025;

namespace EntitySpaces.AddIn
{
    internal delegate bool OnTemplateExecute(TemplateDisplaySurface surface);
    internal delegate void OnTemplateCancel(TemplateDisplaySurface surface);

    internal class TemplateDisplaySurface
    {
        static private TemplateUICollection coll = new TemplateUICollection();
        static private MainWindow MainWindow;

        //Krypton Theme: Keep track of the current instance to handle live theme changes
        static private TemplateDisplaySurface _currentInstance;


        static public Dictionary<Guid, Hashtable> CachedInput = new Dictionary<Guid, Hashtable>();
        public SortedList<int, UserControl> CurrentUIControls = new SortedList<int, UserControl>();
        public Root esMeta = null;
        public Template Template;

        internal TemplateDisplaySurface()
        {
            // Krypton Theme: Subscribe to theme changes
            _currentInstance = this;
            AppTheme.ThemeChanged += OnThemeChanged;
        }

        // Krypton Theme: Method to handle live theme changes
        private void OnThemeChanged()
        {
            if (_currentInstance == null) return;

            Color bgColor = AppTheme.GetBackgroundColor();
            Color fgColor = AppTheme.GetForegroundColor();

            foreach (UserControl control in _currentInstance.CurrentUIControls.Values)
            {
                ApplyThemeToControlTree(control, bgColor, fgColor);
            }
        }

        // Add a method to clean up the event
        public void Cleanup()
        {
            AppTheme.ThemeChanged -= OnThemeChanged;
            _currentInstance = null;
        }

        static internal void Initialize(MainWindow mainWindow)
        {
            // Only accept the first call — which comes from Form1 via NotAConstructor()
            // with the real instance that has a valid Parent (Form1).
            // RegisterAssemblies later instantiates ALL UserControl subclasses via reflection,
            // including MainWindow itself, which would call NotAConstructor() → Initialize()
            // again and overwrite MainWindow with a parentless reflection-created instance.
            // That causes ShowTemplateUIControl() to operate on a control with Parent=NULL,
            // making all layout changes invisible.
            if (TemplateDisplaySurface.MainWindow != null) return;

            TemplateDisplaySurface.MainWindow = mainWindow;
        }


        public void DisplayTemplateUI
        (
            bool useCachedInput, 
            Hashtable input,
            esSettings settings,
            Template template, 
            OnTemplateExecute OnExecuteCallback, 
            OnTemplateCancel OnCancelCallback
        )
        {
            try
            {
                this.Template = template;

                TemplateDisplaySurface.MainWindow.OnTemplateExecuteCallback = OnExecuteCallback;
                TemplateDisplaySurface.MainWindow.OnTemplateCancelCallback = OnCancelCallback;
                TemplateDisplaySurface.MainWindow.CurrentTemplateDisplaySurface = this;

                if (template != null)
                {
                    CurrentUIControls.Clear();
                    PopulateTemplateInfoCollection();

                    SortedList<int, esTemplateInfo> templateInfoCollection = coll.GetTemplateUI(template.Header.UserInterfaceID);

                    if (templateInfoCollection == null || templateInfoCollection.Count == 0)
                    {
                        MainWindow.ShowError(new Exception("Template UI Assembly Cannot Be Located"));
                    }

                    this.esMeta = esMetaCreator.Create(settings);

                    esMeta.Input["OutputPath"] = settings.OutputPath;

                    if (useCachedInput)
                    {
                        if (CachedInput.ContainsKey(template.Header.UniqueID))
                        {
                            Hashtable cachedInput = CachedInput[template.Header.UniqueID];

                            if (cachedInput != null)
                            {
                                foreach (string key in cachedInput.Keys)
                                {
                                    esMeta.Input[key] = cachedInput[key];
                                }
                            }
                        }
                    }

                    if (input != null)
                    {
                        esMeta.Input = input;
                    }

                    MainWindow.tabControlTemplateUI.SuspendLayout();
                    MainWindow.tabControlTemplateUI.TabPages.Clear();

                    // Get the active theme colors once
                    Color themeBgColor = AppTheme.GetBackgroundColor();
                    Color themeFgColor = AppTheme.GetForegroundColor();

                    foreach (esTemplateInfo info in templateInfoCollection.Values)
                    {
                        UserControl userControl = info.UserInterface.CreateInstance(esMeta, useCachedInput, MainWindow.ApplicationObject);
                        CurrentUIControls.Add(info.TabOrder, userControl);

                        TabPage page = new TabPage(info.TabTitle);

                        page.BackColor = themeBgColor;
                        page.ForeColor = themeFgColor;
                        page.UseVisualStyleBackColor = false;

                        userControl.BackColor = themeBgColor;
                        userControl.ForeColor = themeFgColor;

                        if (userControl is IThemeable themeableControl)
                        {
                            themeableControl.ApplyTheme();
                        }

                        // Apply to children
                        ApplyThemeToControlTree(userControl, themeBgColor, themeFgColor);

                        page.Controls.Add(userControl);
                        userControl.Dock = DockStyle.Fill;

                        MainWindow.tabControlTemplateUI.TabPages.Add(page);
                    }

                    MainWindow.tabControlTemplateUI.ResumeLayout();

                    if (CurrentUIControls.Count > 0)
                    {
                        MainWindow.ShowTemplateUIControl();
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.ShowError(ex);
            }
        }

        private void PopulateTemplateInfoCollection()
        {
            try
            {
                if (!coll.IsLoaded)
                {
                    coll.RegisterAssemblies(TemplateDisplaySurface.MainWindow.Settings.UIAssemblyPath);

                    // If RegisterAssemblies found no UI assemblies in the configured path,
                    // keep IsLoaded=false so the next call can retry with a corrected path.
                    // This handles the case where UIAssemblyPath was invalid on first call
                    // (e.g. UIAddIns\ subdirectory did not exist when running from source).
                }
            }
            catch (Exception ex)
            {
                // Reset so the next attempt can retry
                coll.Clear();
                MainWindow.ShowError(ex);
            }
        }

        public bool GatherUserInput()
        {
            try
            {
                foreach (UserControl userControl in this.CurrentUIControls.Values)
                {
                    ITemplateUI templateUI = userControl as ITemplateUI;

                    if (!templateUI.OnExecute())
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.ShowError(ex);
            }

            return true;
        }

        public Hashtable CacheUserInput()
        {
            Hashtable settings = (Hashtable)esMeta.Input.Clone();
            CachedInput[Template.Header.UniqueID] = settings;
            return settings;
        }

        static public void ClearCachedSettings()
        {
            CachedInput = new Dictionary<Guid, Hashtable>();
        }

        #region Krypton Theme Support

       
        private static void ApplyThemeToControlTree(Control parent, Color bgColor, Color fgColor)
        {
            bool isDark = AppTheme.CurrentMode == AppThemeMode.Dark;

            foreach (Control ctrl in parent.Controls)
            {
                // 1. Labels, CheckBoxes, RadioButtons
                if (ctrl is Label || ctrl is CheckBox || ctrl is RadioButton)
                {
                    ctrl.BackColor = Color.Transparent;

                    // CRITICAL FIX: If the control is disabled, use a lighter gray so it's readable in dark mode
                    if (!ctrl.Enabled)
                    {
                        ctrl.ForeColor = isDark ? Color.FromArgb(150, 150, 150) : Color.Gray;
                    }
                    else
                    {
                        ctrl.ForeColor = fgColor;
                    }
                }
                // 2. GroupBoxes (Force text color)
                else if (ctrl is GroupBox gb)
                {
                    gb.BackColor = Color.Transparent;
                    gb.ForeColor = fgColor;
                }
                // 3. TextBoxes and ComboBoxes
                else if (ctrl is TextBox || ctrl is ComboBox || ctrl is ListBox)
                {
                    ctrl.BackColor = isDark ? Color.FromArgb(60, 60, 60) : Color.White;
                    ctrl.ForeColor = fgColor;
                }
                // 4. Containers
                else if (ctrl is Panel || ctrl is TableLayoutPanel || ctrl is TabControl || ctrl is TabPage)
                {
                    ctrl.BackColor = bgColor;
                    ctrl.ForeColor = fgColor;
                }
                // 5. Buttons
                else if (ctrl is Button btn)
                {
                    btn.BackColor = bgColor;
                    btn.ForeColor = fgColor;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = fgColor;
                }

                // Go deeper into nested controls
                if (ctrl.HasChildren)
                {
                    ApplyThemeToControlTree(ctrl, bgColor, fgColor);
                }
            }
        }

        #endregion

    } // end class
}
