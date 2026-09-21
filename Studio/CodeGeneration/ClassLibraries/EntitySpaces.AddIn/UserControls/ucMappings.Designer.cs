namespace EntitySpaces.AddIn
{
    partial class ucMappings
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucMappings));
            this.MappingsDataGridView = new System.Windows.Forms.DataGridView();
            this.ProviderComboBox = new System.Windows.Forms.ComboBox();
            this.ProviderLabel = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolBarButton_Save = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.MappingsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // MappingsDataGridView
            // 
            this.MappingsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MappingsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.MappingsDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.MappingsDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.MappingsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.MappingsDataGridView.Location = new System.Drawing.Point(0, 65);
            this.MappingsDataGridView.Margin = new System.Windows.Forms.Padding(2);
            this.MappingsDataGridView.Name = "MappingsDataGridView";
            this.MappingsDataGridView.RowTemplate.Height = 24;
            this.MappingsDataGridView.Size = new System.Drawing.Size(332, 404);
            this.MappingsDataGridView.TabIndex = 0;
            // 
            // ProviderComboBox
            // 
            this.ProviderComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ProviderComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ProviderComboBox.FormattingEnabled = true;
            this.ProviderComboBox.Location = new System.Drawing.Point(196, 34);
            this.ProviderComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.ProviderComboBox.Name = "ProviderComboBox";
            this.ProviderComboBox.Size = new System.Drawing.Size(126, 21);
            this.ProviderComboBox.TabIndex = 1;
            this.ProviderComboBox.SelectedIndexChanged += new System.EventHandler(this.ProviderComboBox_SelectedIndexChanged);
            // 
            // ProviderLabel
            // 
            this.ProviderLabel.AutoSize = true;
            this.ProviderLabel.Location = new System.Drawing.Point(2, 37);
            this.ProviderLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ProviderLabel.Name = "ProviderLabel";
            this.ProviderLabel.Size = new System.Drawing.Size(190, 13);
            this.ProviderLabel.TabIndex = 2;
            this.ProviderLabel.Text = "Select a Provider to View it\'s Mappings";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Magenta;
            this.imageList1.Images.SetKeyName(0, "save.png");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            // Instanciación
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolBarButton_Save = new System.Windows.Forms.ToolStripButton();

            // 
            // ToolStrip
            // 
            this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.toolBarButton_Save});
            this.ToolStrip.Location = new System.Drawing.Point(0, 0);
            this.ToolStrip.Name = "ToolStrip";
            this.ToolStrip.Size = new System.Drawing.Size(332, 25);
            this.ToolStrip.TabIndex = 9;
            //this.ToolStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ToolStrip_ItemClicked);

            // 
            // toolBarButton_Save
            // 
            this.toolBarButton_Save.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBarButton_Save.ImageIndex = 0;
            this.toolBarButton_Save.Name = "toolBarButton_Save";
            this.toolBarButton_Save.Size = new System.Drawing.Size(23, 22);
            this.toolBarButton_Save.Tag = "save";
            this.toolBarButton_Save.ToolTipText = "Save Mappings";
            // 
            // toolBarButton_Save
            // 
            this.toolBarButton_Save.ImageIndex = 0;
            this.toolBarButton_Save.Name = "toolBarButton_Save";
            this.toolBarButton_Save.Tag = "save";
            this.toolBarButton_Save.ToolTipText = "Save Mappings";
            // 
            // ucMappings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.ToolStrip);
            this.Controls.Add(this.ProviderLabel);
            this.Controls.Add(this.ProviderComboBox);
            this.Controls.Add(this.MappingsDataGridView);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ucMappings";
            this.Size = new System.Drawing.Size(332, 469);
            this.Load += new System.EventHandler(this.ucMappings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MappingsDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView MappingsDataGridView;
        private System.Windows.Forms.ComboBox ProviderComboBox;
        private System.Windows.Forms.Label ProviderLabel;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStrip ToolStrip;
        private System.Windows.Forms.ToolStripButton toolBarButton_Save;

    }
}
