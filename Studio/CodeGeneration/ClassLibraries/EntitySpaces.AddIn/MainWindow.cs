using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using System.Net;
using System.IO;


using Microsoft.Win32;

using EntitySpaces;
using EntitySpaces.Common; // Required for AppTheme and IThemeable
using EntitySpaces.CodeGenerator;
using EntitySpaces.MetadataEngine;

using EntitySpaces.AddIn;

namespace EntitySpaces.AddIn.ES2025
{
    // Add IThemeable to the class definition
    public partial class MainWindow : UserControl, IThemeable
    {
        private Licensing licensing = new Licensing();
        private object applicationObject;
        private List<esUserControl> userControlCollection = new List<esUserControl>();
        private esSettings settings;
        internal string esVersion = VersionInfo.Version;

        internal OnTemplateExecute OnTemplateExecuteCallback;
        internal OnTemplateCancel OnTemplateCancelCallback;
        internal TemplateDisplaySurface CurrentTemplateDisplaySurface;

        public MainWindow()
        {
            InitializeComponent();

            // Subscribe to the global theme change event
            AppTheme.ThemeChanged += ApplyTheme;

            // Unsubscribe when disposed to prevent memory leaks
            this.Disposed += (s, e) => AppTheme.ThemeChanged -= ApplyTheme;

            NotAConstructor();
        }

        internal void NotAConstructor()
        {
            try
            {
                if (!this.DesignMode)
                {
                    TemplateDisplaySurface.Initialize(this);

                    if (this.DesignMode)
                        return;

                    if (this.Settings == null)
                        this.Settings = esSettings.Load();

                    this.Settings = settings;
                    this.ucSettings.Settings = this.Settings;

                    esPlugIn plugin = new esPlugIn(settings);

                    // ... (license code remains unchanged)
//                    Licensing licensing = new Licensing();
//                    string id = licensing.getUniqueID("C");

//                    bool canRunOffline = false;

//                    int result = licensing.ValidateLicense("trial", "b69e3783-9f56-47a7-82e0-6eee6d0779bf", System.Environment.MachineName, id, esVersion, GetProxySettings(Settings));

//                    if (1 != result)
//                    {
//                        result = licensing.RegisterLicense("trial", "b69e3783-9f56-47a7-82e0-6eee6d0779bf", System.Environment.MachineName, id, esVersion, GetProxySettings(Settings));
//                    }
//#else
//                    Crypto crypto = new Crypto();

//                    string id = licensing.getUniqueID("C");
//                    string serialNumber = "";
//                    bool canRunOffline = false;

//                    RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\EntitySpaces 2019", true);
//                    if (key != null)
//                    {
//                        try
//                        {
//                            serialNumber = (string)key.GetValue("Serial_Number");
//                        }
//                        catch { }
//                    }


//                    string offlinePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
//                    offlinePath += @"\EntitySpaces\ES2019\Interop.ADODBX.dll";

//                    // See if we have registered our license
//                    int result = licensing.ValidateLicense("developer", serialNumber, System.Environment.MachineName, id, plugin.esVersion, GetProxySettings(Settings));

//                    switch (result)
//                    {
//                        case 0:

//                            // Try Registering it ...
//                            int newResult = licensing.RegisterLicense("developer", serialNumber, System.Environment.MachineName, id, plugin.esVersion, GetProxySettings(Settings));

//                            if (newResult == 1)
//                            {
//                                licensing.CreateSerialNumber2Key(key, "Serial_Number2", id, false);
//                                result = 1;
//                            }
//                            else 
//                            {
//                                result = 0;
//                            }
//                            break;

//                        case 1:

//                            licensing.CreateSerialNumber2Key(key, "Serial_Number2", id, false);
//                            try
//                            {
//                                File.Delete(offlinePath);
//                            }
//                            catch { }

//                            break;

//                        case -1:

//                            bool isOffLine = false;
//                            DateTime offLineDate = DateTime.MinValue;
//                            if (licensing.ReadSerialNumber2Key(key, "Serial_Number2", id, out isOffLine, out offLineDate))
//                            {
//                                if (isOffLine)
//                                {
//                                    if (File.Exists(offlinePath))
//                                    {
//                                        DateTime fileDate = licensing.OpenOfflineFile(offlinePath);

//                                        if (DateTime.Now > offLineDate)
//                                        {
//                                            TimeSpan ts = DateTime.Now.Subtract(offLineDate);
//                                            if (ts.Days < licensing.DaysTheyCanRunOffline)
//                                            {
//                                                if (fileDate < DateTime.Now)
//                                                {
//                                                    canRunOffline = true;
//                                                }
//                                            }
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    licensing.CreateSerialNumber2Key(key, "Serial_Number2", id, true);
//                                    licensing.CreateOfflineFile(offlinePath);
//                                    canRunOffline = true;
//                                }
//                            }
//                            break;
//                    }
//#endif 

                    this.ucSettings.Settings = this.Settings;

                    //if (result == 1 || (canRunOffline && result == -1))
                    //{
                    userControlCollection.Add(ucProjects);
                    userControlCollection.Add(ucTemplates);
                    userControlCollection.Add(ucMetadata);
                    userControlCollection.Add(ucMappings);

                    this.ucProjects.MainWindow = this;
                    this.ucTemplates.MainWindow = this;
                    this.ucMetadata.MainWindow = this;
                    this.ucMappings.MainWindow = this;
                    //}
                    //else
                    //{
                    //    this.tabControl.TabPages.Remove(this.tabProjects);
                    //    this.tabControl.TabPages.Remove(this.tabMetadata);
                    //    this.tabControl.TabPages.Remove(this.tabProjects);
                    //    this.tabControl.TabPages.Remove(this.tabTemplates);
                    //    this.tabControl.TabPages.Remove(this.tabLanguageMappings);
                    //}

                }
            }
            catch (Exception ex)
            {
                this.ShowError(ex);
            }
            finally
            {
                //userControlCollection.Add(ucWhatsNew);
                //this.ucWhatsNew.MainWindow = this;

                userControlCollection.Add(ucSettings);
                this.ucSettings.MainWindow = this;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object ApplicationObject
        {
            get { return applicationObject; }
            set { applicationObject = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public esSettings Settings
        {
            get { return settings; }
            set
            {
                settings = value;
            }
        }

        public void NofityControlsThatSettingsChanged()
        {
            try
            {
                Root.UnLoadPlugins();

                foreach (esUserControl control in this.userControlCollection)
                {
                    control.OnSettingsChanged();
                }
            }
            catch (Exception ex)
            {
                this.ShowError(ex);
            }
        }

        public void ShowError(Exception ex)
        {
            try
            {
                string errorText = string.Empty;
                string callStack = string.Empty;

                CompilerException cex = ex as CompilerException;

                if (cex != null)
                {
                    foreach (CompilerError error in cex.Results.Errors)
                    {
                        errorText += "Error Found in " + cex.Template.Header.FullFileName + " on line " +
                            cex.Template.TemplateLineFromErrorLine(error.Line) +
                            Environment.NewLine + error.ErrorText + Environment.NewLine + Environment.NewLine;
                    }
                }
                else
                {
                    Exception rootCause = ex;

                    while (rootCause.InnerException != null)
                    {
                        if (rootCause.Equals(ex.InnerException)) break;

                        rootCause = ex.InnerException;
                    }

                    errorText = rootCause.Message;
                }

                this.splitContainer.Panel2Collapsed = false;
                this.pictureBoxError.Image = Resource.error;
                this.textBoxError.Text = errorText;
                this.textBoxError.Text += Environment.NewLine + Environment.NewLine;
                this.textBoxError.Text += ex.StackTrace;
                this.textBoxError.ScrollToCaret();
            }
            catch (Exception exx)
            {
                this.ShowError(exx);
            }
        }

        public void HideErrorOrStatusMessage()
        {
            try
            {
                this.splitContainer.Panel2Collapsed = true;
                this.textBoxError.Text = string.Empty;
            }
            catch (Exception ex)
            {
                this.ShowError(ex);
            }
        }

        public void ShowStatusMessage(string message)
        {
            this.splitContainer.Panel2Collapsed = false;
            this.pictureBoxError.Image = Resource.info;

            this.textBoxError.Text = message;
            this.textBoxError.Text += Environment.NewLine + Environment.NewLine;
            this.textBoxError.ScrollToCaret();
        }

        public void ShowTemplateUIControl()
        {
            try
            {
                this.splitContainerTabControl.Panel1Collapsed = true;
                this.splitContainerTabControl.Panel2Collapsed = false;
            }
            catch (Exception ex)
            {
                this.ShowError(ex);
            }
        }

        private void buttonExecuteTemplateOk_Click(object sender, EventArgs e)
        {
            Cursor origCursor = this.Cursor;

            try
            {
                if (this.OnTemplateExecuteCallback == null) return;

                this.HideErrorOrStatusMessage();

//#if TRIAL
//                Licensing license = new Licensing();
//                string id = license.getUniqueID("C");

//                if (1 != licensing.ValidateLicense("trial", "b69e3783-9f56-47a7-82e0-6eee6d0779bf", System.Environment.MachineName, id, "2019.1.0725.0", GetProxySettings(Settings)))
//                {
//                    return;
//                }
//#else
//                Licensing licensing = new Licensing();
//                licensing.ReplaceMeLater("developer", esVersion, "Serial_Number", "Serial_Number2", "Interop.ADODBX.dll", GetProxySettings(Settings));
//#endif

                this.Cursor = Cursors.WaitCursor;

                if (this.OnTemplateExecuteCallback(this.CurrentTemplateDisplaySurface))
                {
                    this.tabControlTemplateUI.TabPages.Clear();

                    this.splitContainerTabControl.Panel1Collapsed = false;
                    this.splitContainerTabControl.Panel2Collapsed = true;

                    OnTemplateExecuteCallback = null;
                    OnTemplateCancelCallback = null;
                    CurrentTemplateDisplaySurface = null;
                }
            }
            catch (Exception ex)
            {
                this.ShowError(ex);
            }
            finally
            {
                this.Cursor = origCursor;
            }
        }

        private void buttonExecuteTemplateCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.OnTemplateExecuteCallback == null) return;

                this.OnTemplateCancelCallback(this.CurrentTemplateDisplaySurface);

                this.HideErrorOrStatusMessage();

                this.tabControlTemplateUI.TabPages.Clear();

                this.splitContainerTabControl.Panel1Collapsed = false;
                this.splitContainerTabControl.Panel2Collapsed = true;
            }
            catch (Exception ex)
            {
                this.ShowError(ex);
            }
            finally
            {
                OnTemplateExecuteCallback = null;
                OnTemplateCancelCallback = null;
                CurrentTemplateDisplaySurface = null;
            }
        }

        private void pictureBoxError_Click(object sender, EventArgs e)
        {
            try
            {
                this.splitContainer.Panel2Collapsed = true;
            }
            catch (Exception ex)
            {
                this.ShowError(ex);
            }
        }

        private void TabPage_Enter(object sender, EventArgs e)
        {
            try
            {
                this.HideErrorOrStatusMessage();

                TabPage tabPage = sender as TabPage;

                if (tabPage.Name != "Projects")
                {
                    this.ucProjects.PromptForSave();
                }
            }
            catch { }
        }

        #region Krypton Themes

        // Implement the IThemeable interface method
        public void ApplyTheme()
        {
            if (DesignMode) return;

            Color bgColor = AppTheme.GetBackgroundColor();
            Color fgColor = AppTheme.GetForegroundColor();
            bool isDark = AppTheme.CurrentMode == AppThemeMode.Dark;

            // Apply base colors to main containers
            this.BackColor = bgColor;
            this.splitContainer.BackColor = bgColor;
            this.splitContainer.Panel1.BackColor = bgColor;
            this.splitContainer.Panel2.BackColor = Color.Black; // Keep error panel dark

            this.splitContainerTabControl.BackColor = bgColor;
            this.splitContainerTabControl.Panel1.BackColor = bgColor;

            this.tableLayoutPanel3.BackColor = bgColor;
            this.panel1.BackColor = bgColor;


            // Configure the Template UI TabControl for custom drawing
            if (this.tabControlTemplateUI != null)
            {
                this.tabControlTemplateUI.DrawMode = TabDrawMode.OwnerDrawFixed;
                this.tabControlTemplateUI.DrawItem -= TabControlTemplateUI_DrawItem; // Prevent duplicate subscriptions
                this.tabControlTemplateUI.DrawItem += TabControlTemplateUI_DrawItem;
                this.tabControlTemplateUI.Refresh(); // Force redraw
            }

            // Configure main TabControl for custom drawing
            this.tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.tabControl.DrawItem -= TabControl_DrawItem; // Avoid duplicate subscriptions
            this.tabControl.DrawItem += TabControl_DrawItem;

            // Recursively apply theme to all children
            ApplyThemeRecursively(this, bgColor, fgColor, isDark);
        }

        private void TabControlTemplateUI_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl tabCtrl = sender as TabControl;
            if (tabCtrl == null || e.Index < 0) return;

            Color bgColor = AppTheme.GetBackgroundColor();
            Color fgColor = AppTheme.GetForegroundColor();
            bool isDark = AppTheme.CurrentMode == AppThemeMode.Dark;

            TabPage page = tabCtrl.TabPages[e.Index];
            Rectangle rect = tabCtrl.GetTabRect(e.Index);

            // Determine if this tab is currently selected
            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            // Define the background color for the tab header
            Color tabBg;
            if (isSelected)
            {
                tabBg = bgColor; // Selected tab blends with the form background
            }
            else
            {
                // Unselected tabs get a slightly darker shade to stand out
                if (isDark)
                    tabBg = Color.FromArgb(60, 60, 60);
                else if (AppTheme.CurrentMode == AppThemeMode.EntitySpaces)
                    tabBg = Color.FromArgb(180, 200, 230); // Slightly darker blue
                else
                    tabBg = Color.FromArgb(230, 230, 230); // Light gray for Standard
            }

            // 1. Draw the tab background
            using (SolidBrush b = new SolidBrush(tabBg))
            {
                e.Graphics.FillRectangle(b, rect);
            }

            // 2. Draw the tab text
            TextRenderer.DrawText(e.Graphics, page.Text, e.Font, rect, fgColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }


        private void ApplyThemeRecursively(Control parent, Color bgColor, Color fgColor, bool isDark)
        {
            foreach (Control control in parent.Controls)
            {
                // 1. If the control handles its own theme (like ucProjects)
                if (control is IThemeable themeable)
                {
                    themeable.ApplyTheme();

                    // Force background color just in case the ApplyTheme implementation missed the base background
                    if (control is UserControl)
                    {
                        control.BackColor = bgColor;
                    }
                }
                else
                {
                    // 2. Apply generic rules for standard controls
                    if (control is Label || control is LinkLabel)
                    {
                        control.BackColor = Color.Transparent;
                        control.ForeColor = fgColor;
                    }
                    else if (control is TextBox || control is ComboBox || control is ListBox)
                    {
                        control.BackColor = isDark ? Color.FromArgb(60, 60, 60) : Color.White;
                        control.ForeColor = fgColor;

                        if (control is TextBox txt)
                            txt.BorderStyle = BorderStyle.FixedSingle;
                    }
                    else if (control is Button btn)
                    {
                        btn.BackColor = bgColor;
                        btn.ForeColor = fgColor;
                        btn.FlatStyle = FlatStyle.Flat;
                        btn.FlatAppearance.BorderColor = fgColor;
                    }
                    // ADDED: || control is UserControl to catch standard UserControls like ucTemplates and ucMetadata
                    else if (control is TabPage || control is Panel || control is TableLayoutPanel || control is SplitContainer || control is SplitterPanel || control is UserControl)
                    {
                        control.BackColor = bgColor;
                    }
                    else if (control is TabControl tabCtrl)
                    {
                        tabCtrl.BackColor = bgColor;
                        foreach (TabPage page in tabCtrl.TabPages)
                        {
                            page.BackColor = bgColor;
                        }
                    }
                    else if (control is TreeView tree)
                    {
                        tree.BackColor = isDark ? Color.FromArgb(45, 45, 48) : Color.White;
                        tree.ForeColor = fgColor;
                    }
                    else if (control is DataGridView dgv)
                    {
                        dgv.BackgroundColor = bgColor;
                        dgv.DefaultCellStyle.BackColor = isDark ? Color.FromArgb(45, 45, 48) : Color.White;
                        dgv.DefaultCellStyle.ForeColor = fgColor;
                    }
                }

                // 3. ALWAYS recurse into children
                if (control.HasChildren)
                {
                    ApplyThemeRecursively(control, bgColor, fgColor, isDark);
                }
            }
        }

        private void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl tabCtrl = sender as TabControl;
            if (tabCtrl == null || e.Index < 0) return;

            Color bgColor = AppTheme.GetBackgroundColor();
            Color fgColor = AppTheme.GetForegroundColor();
            bool isDark = AppTheme.CurrentMode == AppThemeMode.Dark;

            TabPage page = tabCtrl.TabPages[e.Index];
            Rectangle rect = tabCtrl.GetTabRect(e.Index);

            // Determine selected state
            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            // Define the background color for the tab header
            Color tabBg;
            if (isSelected)
            {
                // Selected tab blends with the form background
                tabBg = bgColor;
            }
            else
            {
                if (isDark)
                {
                    // Dark gray for dark mode
                    tabBg = Color.FromArgb(60, 60, 60);
                }
                else if (AppTheme.CurrentMode == AppThemeMode.EntitySpaces)
                {
                    // Very light blue for EntitySpaces to avoid the dirty gray look
                    tabBg = Color.FromArgb(220, 235, 250);
                }
                else
                {
                    // Light gray for Standard mode
                    tabBg = Color.FromArgb(240, 240, 240);
                }
            }

            // Draw background
            using (SolidBrush b = new SolidBrush(tabBg))
            {
                e.Graphics.FillRectangle(b, rect);
            }

            // Draw text
            TextRenderer.DrawText(e.Graphics, page.Text, e.Font, rect, fgColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        #endregion
    }
}