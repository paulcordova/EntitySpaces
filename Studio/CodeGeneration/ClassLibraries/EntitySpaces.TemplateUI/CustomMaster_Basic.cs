using EntitySpaces.AddIn.TemplateUI;
using EntitySpaces.MetadataEngine;
using EntitySpaces.Common; // Add this for AppTheme
using Krypton.Toolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EntitySpaces.TemplateUI
{
    // Add IThemeable to the interface list
    public partial class CustomMaster_Basic : UserControl, ITemplateUI, IThemeable
    {
        private Root esMeta;
        private bool UseCachedSettings;
        private object applicationObject;

        public CustomMaster_Basic()
        {
            InitializeComponent();

            // Subscribe to the global theme change event
            AppTheme.ThemeChanged += ApplyTheme;

            // Unsubscribe when the control is disposed to prevent memory leaks
            this.Disposed += (s, e) => AppTheme.ThemeChanged -= ApplyTheme;

        }
        
        // Add this override to apply the theme exactly when the control is loaded
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Apply theme now that the control is ready and Form1 has loaded the saved settings
            ApplyTheme();
        }

        #region ITemplateUI Members

        esTemplateInfo ITemplateUI.Init()
        {
            esTemplateInfo info = new esTemplateInfo();
            info.UserInterface = this;
            info.UserInterfaceId = new Guid("60C6D790-E5BE-4634-B168-A89EC6965F45");
            info.TabTitle = "Basic Information";
            info.TabOrder = 0;
            return info;
        }

        UserControl ITemplateUI.CreateInstance(Root esMeta, bool cachedSettings, object applicationObject)
        {
            CustomMaster_Basic window = new CustomMaster_Basic();
            window.esMeta = esMeta;
            window.applicationObject = applicationObject;
            window.UseCachedSettings = cachedSettings;
            return window;
        }

        bool ITemplateUI.OnExecute()
        {
            try
            {
                esMeta.Input["OutputPath"] = txtOutputPath.Text;
                esMeta.Input["Namespace"] = txtNamespace.Text;
                esMeta.Input["Database"] = ((IDatabase)this.cboDatabase.SelectedItem).Name;

                ArrayList list = new ArrayList();

                esMeta.Input["EntityType"] = ShowTables() == true ? "Tables" : "Views";

                foreach (ITableView tv in this.lboxTablesViews.SelectedItems)
                {
                    list.Add(tv.FullName);
                }

                esMeta.Input["Entities"] = list;
            }
            catch { }

            return true;
        }

        void ITemplateUI.OnCancel()
        {
            throw new NotImplementedException();
        }

        #endregion

        private void CustomMaster_Basic_Load(object sender, EventArgs e)
        {
            try
            {
                //-----------------------------------------------------------
                // OutputPath
                //-----------------------------------------------------------
                if (UseCachedSettings)
                {
                    this.txtOutputPath.Text = (string)esMeta.Input["OutputPath"];
                }
                else
                {
                    this.txtOutputPath.Text = (string)esMeta.Input["OutputPath"];

                    if (!this.txtOutputPath.Text.EndsWith(@"\"))
                        this.txtOutputPath.Text += @"\";

                    this.txtOutputPath.Text += @"Custom\";
                }

                //-----------------------------------------------------------
                // Namespace
                //-----------------------------------------------------------
                if (UseCachedSettings && esMeta.Input.ContainsKey("Namespace"))
                {
                    this.txtNamespace.Text = (string)esMeta.Input["Namespace"];
                }

                //-----------------------------------------------------------
                // Database
                //-----------------------------------------------------------
                this.cboDatabase.DataSource = esMeta.Databases;
                this.cboDatabase.DisplayMember = "Name";

                string database = "";
                string databaseDriver = esMeta != null ? esMeta.DriverString : string.Empty;

                if (UseCachedSettings && esMeta.Input.ContainsKey("Database"))
                {
                    database = (string)esMeta.Input["Database"];
                }
                else if (esMeta.DefaultDatabase != null)
                {
                    database = esMeta.DefaultDatabase.Name;
                }

                //NEW: show user the database provider selected in Main
                if (!string.IsNullOrWhiteSpace(databaseDriver))
                {
                    this.label4.Text = "Database" + " On: " + databaseDriver.ToUpperInvariant();
                }
                else
                {
                    this.label4.Text = "Database";
                }


                int index = this.cboDatabase.FindString(database);
                if (index != -1)
                {
                    this.cboDatabase.SelectedIndex = index;
                }

                //-----------------------------------------------------------
                // EntityType
                //-----------------------------------------------------------
                index = this.cboTablesViews.Items.Add("Tables");
                this.cboTablesViews.Items.Add("Views");

                if (UseCachedSettings && esMeta.Input.ContainsKey("EntityType"))
                {
                    index = this.cboTablesViews.FindString((string)esMeta.Input["EntityType"]);
                }

                this.cboTablesViews.SelectedIndex = index;

                //-----------------------------------------------------------
                // Entities
                //-----------------------------------------------------------
                // NOTE: Setting the "cboTablesViews.SelectedIndex" causes the lboxTablesViews
                //       to be bound
                if (UseCachedSettings && esMeta.Input.ContainsKey("Entities"))
                {
                    this.lboxTablesViews.SelectedItems.Clear();

                    ArrayList entities = (ArrayList)esMeta.Input["Entities"];

                    foreach (string entity in entities)
                    {
                        index = this.lboxTablesViews.FindString(entity);

                        if (index != -1)
                        {
                            this.lboxTablesViews.SetSelected(index, true);
                        }
                    }
                }
            }
            catch { }
        }

        private void cboDatabase_SelectionChangeCommitted(object sender, EventArgs e)
        {
            BindTablesOrViews();
        }

        private void cboTablesViews_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindTablesOrViews();
        }

        private void btnOutputPath_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
                folderBrowserDialog1.Description = "Select the Output Folder";
                folderBrowserDialog1.SelectedPath = txtOutputPath.Text;

                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    txtOutputPath.Text = folderBrowserDialog1.SelectedPath;
                }
            }
            catch { }
        }

        private bool ShowTables()
        {
            return this.cboTablesViews.SelectedIndex == 0;
        }

        private void BindTablesOrViews()
        {
            try
            {
                IDatabase database = this.cboDatabase.SelectedItem as IDatabase;

                if (database != null)
                {
                    this.lboxTablesViews.DataSource = null;

                    if (ShowTables())
                    {
                        this.lboxTablesViews.DataSource = database.Tables;
                        this.lboxTablesViews.DisplayMember = "FullName";
                    }
                    else
                    {
                        this.lboxTablesViews.DataSource = database.Views;
                        this.lboxTablesViews.DisplayMember = "FullName";
                    }
                }
            }
            catch { }
        }

        private void lboxTablesViews_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                if ((ModifierKeys & Keys.Control) == Keys.Control)
                {
                    for (int i = 0; i < this.lboxTablesViews.Items.Count; i++)
                    {
                        this.lboxTablesViews.SetSelected(i, true);
                    }
                }
            }
        }

        #region Krypton Themes

        // Implement the interface method
        public void ApplyTheme()
        {
            KryptonColors();
        }

        private void KryptonColors()
        {
            // Retrieve colors from the central AppTheme
            Color bgColor = AppTheme.GetBackgroundColor();
            Color fgColor = AppTheme.GetForegroundColor();
            bool isDark = AppTheme.CurrentMode == AppThemeMode.Dark;

            // Apply background to the UserControl itself
            this.BackColor = bgColor;
            this.ForeColor = fgColor;

            // Set labels to transparent to inherit the form's background color
            this.label1.BackColor = Color.Transparent;
            this.label1.ForeColor = fgColor;

            this.label2.BackColor = Color.Transparent;
            this.label2.ForeColor = fgColor;

            this.label4.BackColor = Color.Transparent;
            this.label4.ForeColor = fgColor;

            this.label5.BackColor = Color.Transparent;
            this.label5.ForeColor = fgColor;

            this.label6.BackColor = Color.Transparent;
            this.label6.ForeColor = fgColor;

            // --- TextBoxes: White in light mode, Dark Gray in Dark mode ---
            Color inputBg = isDark ? Color.FromArgb(60, 60, 60) : Color.White;

            this.txtOutputPath.BackColor = inputBg;
            this.txtOutputPath.ForeColor = fgColor;
            this.txtOutputPath.BorderStyle = BorderStyle.FixedSingle;

            this.txtNamespace.BackColor = inputBg;
            this.txtNamespace.ForeColor = fgColor;
            this.txtNamespace.BorderStyle = BorderStyle.FixedSingle;

            // Apply palette color to the button
            this.btnOutputPath.BackColor = bgColor;
            this.btnOutputPath.ForeColor = fgColor;

            // --- ListBox and ComboBoxes ---
            this.lboxTablesViews.BackColor = inputBg;
            this.lboxTablesViews.ForeColor = fgColor;

            this.cboDatabase.BackColor = inputBg;
            this.cboDatabase.ForeColor = fgColor;

            this.cboTablesViews.BackColor = inputBg;
            this.cboTablesViews.ForeColor = fgColor;

            // --- Theme TabControl if present ---
            // This ensures the background behind the tabs matches the dark mode
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is TabControl tabCtrl)
                {
                    // Note: Standard WinForms TabControl doesn't easily change 
                    // its tab headers color without custom drawing, but this sets the page background
                    foreach (TabPage page in tabCtrl.TabPages)
                    {
                        page.BackColor = bgColor;
                        page.ForeColor = fgColor;
                    }
                }
            }
        }

        #endregion
    }
}