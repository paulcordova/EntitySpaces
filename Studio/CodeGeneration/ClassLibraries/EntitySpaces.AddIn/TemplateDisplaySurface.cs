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

        static public Dictionary<Guid, Hashtable> CachedInput = new Dictionary<Guid, Hashtable>();
        public SortedList<int, UserControl> CurrentUIControls = new SortedList<int, UserControl>();
        public Root esMeta = null;
        public Template Template;


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

        internal TemplateDisplaySurface()
        {

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

                    foreach (esTemplateInfo info in templateInfoCollection.Values)
                    {
                        UserControl userControl = info.UserInterface.CreateInstance(esMeta, useCachedInput, MainWindow.ApplicationObject);
                        CurrentUIControls.Add(info.TabOrder, userControl);

                        TabPage page = new TabPage(info.TabTitle);
                        page.Controls.Add(userControl);

                        userControl.Dock = DockStyle.Fill;

                        MainWindow.tabControlTemplateUI.TabPages.Add(page);

                        MainWindow.ShowTemplateUIControl();
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
    }
}
