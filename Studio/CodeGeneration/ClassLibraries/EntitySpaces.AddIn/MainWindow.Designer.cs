namespace EntitySpaces.AddIn.ES2025
{
    partial class MainWindow
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            splitContainer = new System.Windows.Forms.SplitContainer();
            splitContainerTabControl = new System.Windows.Forms.SplitContainer();
            tabControl = new System.Windows.Forms.TabControl();
            tabProjects = new System.Windows.Forms.TabPage();
            ucProjects = new ucProjects();
            tabTemplates = new System.Windows.Forms.TabPage();
            ucTemplates = new ucTemplates();
            tabSettings = new System.Windows.Forms.TabPage();
            ucSettings = new ucSettings();
            tabMetadata = new System.Windows.Forms.TabPage();
            ucMetadata = new ucMetadata();
            tabLanguageMappings = new System.Windows.Forms.TabPage();
            ucMappings = new ucMappings();
            tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            tabControlTemplateUI = new System.Windows.Forms.TabControl();
            panel1 = new System.Windows.Forms.Panel();
            buttonExecuteTemplateCancel = new System.Windows.Forms.Button();
            buttonExecuteTemplateOk = new System.Windows.Forms.Button();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            textBoxError = new System.Windows.Forms.TextBox();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            pictureBoxError = new System.Windows.Forms.PictureBox();
            imageList1 = new System.Windows.Forms.ImageList(components);
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerTabControl).BeginInit();
            splitContainerTabControl.Panel1.SuspendLayout();
            splitContainerTabControl.Panel2.SuspendLayout();
            splitContainerTabControl.SuspendLayout();
            tabControl.SuspendLayout();
            tabProjects.SuspendLayout();
            tabTemplates.SuspendLayout();
            tabSettings.SuspendLayout();
            tabMetadata.SuspendLayout();
            tabLanguageMappings.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            panel1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxError).BeginInit();
            SuspendLayout();
            // 
            // splitContainer
            // 
            splitContainer.BackColor = System.Drawing.SystemColors.Window;
            splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer.Location = new System.Drawing.Point(0, 0);
            splitContainer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            splitContainer.Name = "splitContainer";
            splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(splitContainerTabControl);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(tableLayoutPanel1);
            splitContainer.Panel2Collapsed = true;
            splitContainer.Panel2MinSize = 40;
            splitContainer.Size = new System.Drawing.Size(594, 734);
            splitContainer.SplitterDistance = 25;
            splitContainer.SplitterWidth = 5;
            splitContainer.TabIndex = 1;
            // 
            // splitContainerTabControl
            // 
            splitContainerTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerTabControl.Location = new System.Drawing.Point(0, 0);
            splitContainerTabControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            splitContainerTabControl.Name = "splitContainerTabControl";
            // 
            // splitContainerTabControl.Panel1
            // 
            splitContainerTabControl.Panel1.Controls.Add(tabControl);
            // 
            // splitContainerTabControl.Panel2
            // 
            splitContainerTabControl.Panel2.Controls.Add(tableLayoutPanel3);
            splitContainerTabControl.Panel2Collapsed = true;
            splitContainerTabControl.Size = new System.Drawing.Size(594, 734);
            splitContainerTabControl.SplitterDistance = 288;
            splitContainerTabControl.SplitterWidth = 5;
            splitContainerTabControl.TabIndex = 1;
            // 
            // tabControl
            // 
            tabControl.AllowDrop = true;
            tabControl.Controls.Add(tabProjects);
            tabControl.Controls.Add(tabTemplates);
            tabControl.Controls.Add(tabSettings);
            tabControl.Controls.Add(tabMetadata);
            tabControl.Controls.Add(tabLanguageMappings);
            tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl.Location = new System.Drawing.Point(0, 0);
            tabControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new System.Drawing.Size(594, 734);
            tabControl.TabIndex = 0;
            // 
            // tabProjects
            // 
            tabProjects.Controls.Add(ucProjects);
            tabProjects.Location = new System.Drawing.Point(4, 24);
            tabProjects.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabProjects.Name = "tabProjects";
            tabProjects.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabProjects.Size = new System.Drawing.Size(586, 706);
            tabProjects.TabIndex = 6;
            tabProjects.Text = "Projects";
            tabProjects.UseVisualStyleBackColor = true;
            tabProjects.Enter += TabPage_Enter;
            // 
            // ucProjects
            // 
            ucProjects.AllowDrop = true;
            ucProjects.BackColor = System.Drawing.SystemColors.Window;
            ucProjects.Dock = System.Windows.Forms.DockStyle.Fill;
            ucProjects.Location = new System.Drawing.Point(4, 3);
            ucProjects.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            ucProjects.Name = "ucProjects";
            ucProjects.Size = new System.Drawing.Size(578, 700);
            ucProjects.TabIndex = 0;
            // 
            // tabTemplates
            // 
            tabTemplates.Controls.Add(ucTemplates);
            tabTemplates.Location = new System.Drawing.Point(4, 24);
            tabTemplates.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabTemplates.Name = "tabTemplates";
            tabTemplates.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabTemplates.Size = new System.Drawing.Size(192, 72);
            tabTemplates.TabIndex = 0;
            tabTemplates.Text = "Templates";
            tabTemplates.UseVisualStyleBackColor = true;
            tabTemplates.Enter += TabPage_Enter;
            // 
            // ucTemplates
            // 
            ucTemplates.BackColor = System.Drawing.SystemColors.Window;
            ucTemplates.Dock = System.Windows.Forms.DockStyle.Fill;
            ucTemplates.Location = new System.Drawing.Point(4, 3);
            ucTemplates.Margin = new System.Windows.Forms.Padding(2);
            ucTemplates.Name = "ucTemplates";
            ucTemplates.Size = new System.Drawing.Size(184, 66);
            ucTemplates.TabIndex = 0;
            // 
            // tabSettings
            // 
            tabSettings.Controls.Add(ucSettings);
            tabSettings.Location = new System.Drawing.Point(4, 24);
            tabSettings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabSettings.Name = "tabSettings";
            tabSettings.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabSettings.Size = new System.Drawing.Size(192, 72);
            tabSettings.TabIndex = 3;
            tabSettings.Text = "Settings";
            tabSettings.UseVisualStyleBackColor = true;
            tabSettings.Enter += TabPage_Enter;
            // 
            // ucSettings
            // 
            ucSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            ucSettings.Location = new System.Drawing.Point(4, 3);
            ucSettings.Margin = new System.Windows.Forms.Padding(5);
            ucSettings.Name = "ucSettings";
            ucSettings.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ucSettings.Size = new System.Drawing.Size(184, 66);
            ucSettings.TabIndex = 0;
            // 
            // tabMetadata
            // 
            tabMetadata.BackColor = System.Drawing.Color.Transparent;
            tabMetadata.Controls.Add(ucMetadata);
            tabMetadata.Location = new System.Drawing.Point(4, 24);
            tabMetadata.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabMetadata.Name = "tabMetadata";
            tabMetadata.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabMetadata.Size = new System.Drawing.Size(192, 72);
            tabMetadata.TabIndex = 1;
            tabMetadata.Text = "Metadata";
            tabMetadata.UseVisualStyleBackColor = true;
            tabMetadata.Enter += TabPage_Enter;
            // 
            // ucMetadata
            // 
            ucMetadata.Dock = System.Windows.Forms.DockStyle.Fill;
            ucMetadata.Location = new System.Drawing.Point(4, 3);
            ucMetadata.Margin = new System.Windows.Forms.Padding(5);
            ucMetadata.Name = "ucMetadata";
            ucMetadata.Size = new System.Drawing.Size(184, 66);
            ucMetadata.TabIndex = 0;
            // 
            // tabLanguageMappings
            // 
            tabLanguageMappings.Controls.Add(ucMappings);
            tabLanguageMappings.Location = new System.Drawing.Point(4, 24);
            tabLanguageMappings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabLanguageMappings.Name = "tabLanguageMappings";
            tabLanguageMappings.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabLanguageMappings.Size = new System.Drawing.Size(192, 72);
            tabLanguageMappings.TabIndex = 4;
            tabLanguageMappings.Text = "Mappings";
            tabLanguageMappings.UseVisualStyleBackColor = true;
            tabLanguageMappings.Enter += TabPage_Enter;
            // 
            // ucMappings
            // 
            ucMappings.BackColor = System.Drawing.SystemColors.Window;
            ucMappings.Dock = System.Windows.Forms.DockStyle.Fill;
            ucMappings.Location = new System.Drawing.Point(4, 3);
            ucMappings.Margin = new System.Windows.Forms.Padding(2);
            ucMappings.Name = "ucMappings";
            ucMappings.Size = new System.Drawing.Size(184, 66);
            ucMappings.TabIndex = 1;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel3.Controls.Add(tabControlTemplateUI, 0, 0);
            tableLayoutPanel3.Controls.Add(panel1, 0, 1);
            tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            tableLayoutPanel3.Size = new System.Drawing.Size(96, 100);
            tableLayoutPanel3.TabIndex = 2;
            // 
            // tabControlTemplateUI
            // 
            tabControlTemplateUI.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControlTemplateUI.Location = new System.Drawing.Point(4, 3);
            tabControlTemplateUI.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabControlTemplateUI.Name = "tabControlTemplateUI";
            tabControlTemplateUI.SelectedIndex = 0;
            tabControlTemplateUI.Size = new System.Drawing.Size(88, 48);
            tabControlTemplateUI.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Controls.Add(buttonExecuteTemplateCancel);
            panel1.Controls.Add(buttonExecuteTemplateOk);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(4, 57);
            panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(88, 40);
            panel1.TabIndex = 1;
            // 
            // buttonExecuteTemplateCancel
            // 
            buttonExecuteTemplateCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonExecuteTemplateCancel.Location = new System.Drawing.Point(-97, 10);
            buttonExecuteTemplateCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonExecuteTemplateCancel.Name = "buttonExecuteTemplateCancel";
            buttonExecuteTemplateCancel.Size = new System.Drawing.Size(88, 27);
            buttonExecuteTemplateCancel.TabIndex = 3;
            buttonExecuteTemplateCancel.Text = "&Cancel";
            buttonExecuteTemplateCancel.UseVisualStyleBackColor = true;
            buttonExecuteTemplateCancel.Click += buttonExecuteTemplateCancel_Click;
            // 
            // buttonExecuteTemplateOk
            // 
            buttonExecuteTemplateOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonExecuteTemplateOk.Location = new System.Drawing.Point(-3, 10);
            buttonExecuteTemplateOk.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonExecuteTemplateOk.Name = "buttonExecuteTemplateOk";
            buttonExecuteTemplateOk.Size = new System.Drawing.Size(88, 27);
            buttonExecuteTemplateOk.TabIndex = 2;
            buttonExecuteTemplateOk.Text = "&Ok";
            buttonExecuteTemplateOk.UseVisualStyleBackColor = true;
            buttonExecuteTemplateOk.Click += buttonExecuteTemplateOk_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = System.Drawing.Color.Black;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(textBoxError, 1, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new System.Drawing.Size(150, 46);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // textBoxError
            // 
            textBoxError.BackColor = System.Drawing.Color.Black;
            textBoxError.BorderStyle = System.Windows.Forms.BorderStyle.None;
            textBoxError.Dock = System.Windows.Forms.DockStyle.Fill;
            textBoxError.ForeColor = System.Drawing.Color.Yellow;
            textBoxError.Location = new System.Drawing.Point(86, 3);
            textBoxError.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxError.Multiline = true;
            textBoxError.Name = "textBoxError";
            textBoxError.ReadOnly = true;
            textBoxError.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            textBoxError.Size = new System.Drawing.Size(60, 40);
            textBoxError.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.BackColor = System.Drawing.Color.Black;
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            tableLayoutPanel2.Controls.Add(pictureBoxError, 0, 1);
            tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point(4, 3);
            tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 3;
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.00001F));
            tableLayoutPanel2.Size = new System.Drawing.Size(74, 40);
            tableLayoutPanel2.TabIndex = 2;
            // 
            // pictureBoxError
            // 
            pictureBoxError.ErrorImage = null;
            pictureBoxError.Image = Resource.error;
            pictureBoxError.InitialImage = null;
            pictureBoxError.Location = new System.Drawing.Point(4, -7);
            pictureBoxError.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pictureBoxError.Name = "pictureBoxError";
            pictureBoxError.Size = new System.Drawing.Size(59, 55);
            pictureBoxError.TabIndex = 0;
            pictureBoxError.TabStop = false;
            pictureBoxError.Click += pictureBoxError_Click;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            imageList1.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = System.Drawing.Color.Transparent;
            imageList1.Images.SetKeyName(0, "error.png");
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(splitContainer);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "MainWindow";
            Size = new System.Drawing.Size(594, 734);
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            splitContainerTabControl.Panel1.ResumeLayout(false);
            splitContainerTabControl.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerTabControl).EndInit();
            splitContainerTabControl.ResumeLayout(false);
            tabControl.ResumeLayout(false);
            tabProjects.ResumeLayout(false);
            tabTemplates.ResumeLayout(false);
            tabSettings.ResumeLayout(false);
            tabMetadata.ResumeLayout(false);
            tabLanguageMappings.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            panel1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxError).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabTemplates;
        private System.Windows.Forms.TabPage tabMetadata;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.TabPage tabLanguageMappings;
        private ucMetadata ucMetadata;
        private ucMappings ucMappings;
        private ucTemplates ucTemplates;
        private ucSettings ucSettings;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox textBoxError;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.SplitContainer splitContainerTabControl;
        public System.Windows.Forms.TabControl tabControlTemplateUI;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonExecuteTemplateCancel;
        private System.Windows.Forms.Button buttonExecuteTemplateOk;
        //private System.Windows.Forms.TabPage tabWhatsNew;
        //private EntitySpaces.AddIn.ucWhatsNew ucWhatsNew;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TabPage tabProjects;
        private ucProjects ucProjects;
        private System.Windows.Forms.PictureBox pictureBoxError;
    }
}
