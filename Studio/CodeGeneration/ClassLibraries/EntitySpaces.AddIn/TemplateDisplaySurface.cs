﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using EntitySpaces.AddIn.TemplateUI;
using EntitySpaces.CodeGenerator;
using EntitySpaces.Common;
using EntitySpaces.MetadataEngine;

using EntitySpaces.AddIn.ES2024;

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
                }
            }
            catch (Exception ex)
            {
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
