using EntitySpaces.AddIn.TemplateUI;
using EntitySpaces.CodeGenerator;
using EntitySpaces.Common;
using EntitySpaces.MetadataEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace EntitySpaces.AddIn
{
    internal partial class ucTemplates : esUserControl
    {
        TemplateDisplaySurface templateDisplaySurface = new TemplateDisplaySurface();

        public ucTemplates()
        {
            try
            {
                InitializeComponent();
            }
            catch { }
        }

        private void ucTemplates_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;

            Cursor origCursor = this.Cursor;

            try
            {
                this.Cursor = Cursors.WaitCursor;

                tree.LoadTemplates(templateMenu, subTemplateMenu, Settings);
            }
            finally
            {
                this.Cursor = origCursor;
            }
        }

        private void DisplayTemplateForEdit()
        {
            try
            {
                if (this.tree.SelectedNode != null)
                {
                    Template t = this.tree.SelectedNode.Tag as Template;

                    if (t != null)
                    {
                        System.Diagnostics.Process.Start(t.Header.FullFileName);
                    }
                }
            }
            catch
            {
                // If they don't have the ".est" extension mapped we'll just launch 
                // it in Notepad.exe (boring)
                try
                {
                    if (this.tree.SelectedNode != null)
                    {
                        Template t = this.tree.SelectedNode.Tag as Template;
                        System.Diagnostics.Process.Start("notepad.exe", t.Header.FullFileName);
                    }
                }
                catch { }
            }
        }

        private void DisplayTemplateUI(bool useCachedInput)
        {
            Cursor origCursor = this.Cursor;
            try
            {
                this.Cursor = Cursors.WaitCursor;

                this.MainWindow.HideErrorOrStatusMessage();

                Template template = this.tree.SelectedNode.Tag as Template;
                this.templateDisplaySurface.DisplayTemplateUI(useCachedInput, null, this.Settings, template, OnExecute, OnCancel);
            }
            catch (Exception ex)
            {
                this.MainWindow.ShowError(ex);
            }
            finally
            {
                this.Cursor = origCursor;
            }
        }

        private bool OnExecute(TemplateDisplaySurface surface)
        {
            try
            {
                if (surface.GatherUserInput())
                {
                    Template temp = new Template();
                    temp.Execute(surface.esMeta, surface.Template.Header.FullFileName);

                    surface.CacheUserInput();

                    this.MainWindow.ShowStatusMessage("Template '" + surface.Template.Header.Title + "' generated successfully.");
                }
                else return false;
            }
            catch (Exception ex)
            {
                this.MainWindow.ShowError(ex);
                return false;
            }

            return true;
        }

        private void OnCancel(TemplateDisplaySurface surface)
        {

        }

        public override void OnSettingsChanged()
        {
            try
            {
                TemplateDisplaySurface.ClearCachedSettings();

                Template.SetTemplateCachePath(esSettings.TemplateCachePath);
                Template.SetCompilerAssemblyPath(Settings.CompilerAssemblyPath);

                tree.LoadTemplates(templateMenu, subTemplateMenu, Settings);
            }
            catch { }
        }

        private void ButtonRefresh_Click(object sender, EventArgs e)
        {
            tree.LoadTemplates(templateMenu, subTemplateMenu, Settings);
        }

        private void ButtonExecute_Click(object sender, EventArgs e)
        {
            DisplayTemplateUI(false);
        }

        private void ButtonExecuteCache_Click(object sender, EventArgs e)
        {
            DisplayTemplateUI(true);
        }

        private void tree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                if (e.Node != null)
                {
                    Template template = e.Node.Tag as Template;

                    if (template != null)
                    {
                        this.ButtonExecute.Enabled = !template.Header.IsSubTemplate;
                        this.ButtonExecuteCache.Enabled = !template.Header.IsSubTemplate;
                        this.ButtonRecordProject.Enabled = !template.Header.IsSubTemplate;
                        this.ButtonViewTemplate.Enabled = true;
                        return;
                    }
                }

                this.ButtonExecute.Enabled = false;
                this.ButtonExecuteCache.Enabled = false;
                this.ButtonRecordProject.Enabled = false;
                this.ButtonViewTemplate.Enabled = false;
            }
            catch { }
        }

        private void tree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                if (e.Node != null)
                {
                    Template template = e.Node.Tag as Template;

                    if (template != null)
                    {
                        if (template.Header.IsSubTemplate)
                        {
                            DisplayTemplateForEdit();
                            return;
                        }

                        switch (this.Settings.DefaultTemplateDoubleClickAction)
                        {
                            case "Edit":
                                DisplayTemplateForEdit();
                                break;

                            case "Execute":
                                DisplayTemplateUI(false);
                                break;

                            case "ExecuteWithLastSettings":
                                DisplayTemplateUI(true);
                                break;

                            default:
                                DisplayTemplateUI(false);
                                break;
                        }
                    }
                }
            }
            catch { }
        }

        private void ButtonViewTemplate_Click(object sender, EventArgs e)
        {
            DisplayTemplateForEdit();
        }

        private void ButtonOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "explorer";
                p.StartInfo.Arguments = "/e," + Settings.OutputPath;
                p.StartInfo.UseShellExecute = true;
                p.Start();
            }
            catch { }
        }

        #region Krypton Themes

        // Override the base method to apply specific styles for this control
        public override void ApplyTheme()
        {
            base.ApplyTheme(); // Applies general UserControl background from AppTheme
            KryptonColors();   // Applies specific control styles
        }

        private void KryptonColors()
        {
            Color bgColor = AppTheme.GetBackgroundColor();
            Color fgColor = AppTheme.GetForegroundColor();
            bool isDark = AppTheme.CurrentMode == AppThemeMode.Dark;

            // Define selection color for menus based on theme
            Color selectionColor = isDark ? Color.FromArgb(75, 75, 75) : Color.FromArgb(200, 220, 240);
            var customRenderer = new KryptonMenuRenderer(bgColor, selectionColor, fgColor);

            // 1. Style the ToolStrip
            if (this.toolStrip1 != null)
            {
                this.toolStrip1.BackColor = bgColor;
                this.toolStrip1.ForeColor = fgColor;
                this.toolStrip1.Renderer = customRenderer;

                foreach (ToolStripItem item in this.toolStrip1.Items)
                {
                    item.BackColor = bgColor;
                    item.ForeColor = fgColor;
                }
            }

            // 2. Style the Context Menus
            if (this.templateMenu != null)
            {
                this.templateMenu.BackColor = bgColor;
                this.templateMenu.ForeColor = fgColor;
                this.templateMenu.Renderer = customRenderer;
            }

            if (this.subTemplateMenu != null)
            {
                this.subTemplateMenu.BackColor = bgColor;
                this.subTemplateMenu.ForeColor = fgColor;
                this.subTemplateMenu.Renderer = customRenderer;
            }

            // 3. Style the custom Tree control (ucTemplateControl)
            // Note: This handles the background of the UserControl. 
            // If it has an internal TreeView that stays white, we may need to style that inside ucTemplateControl itself.
            if (this.tree != null)
            {
                Color treeBg;
                if (AppTheme.CurrentMode == AppThemeMode.Dark)
                {
                    treeBg = Color.FromArgb(45, 45, 48);
                }
                else if (AppTheme.CurrentMode == AppThemeMode.EntitySpaces)
                {
                    treeBg = bgColor; // Use the blue background
                }
                else
                {
                    treeBg = Color.White;
                }

                this.tree.BackColor = treeBg;
                this.tree.ForeColor = fgColor;

                // If 'tree' implements IThemeable, it will handle its own inner controls.
                // If not, the recursive method in MainWindow will try to paint its children.
            }
        }

        #endregion

    } // end class
}
