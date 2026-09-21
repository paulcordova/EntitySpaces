namespace EntitySpaces.TemplateUI
{
    partial class AdminTemplateSuite
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdminTemplateSuite));
            cboxDatabases = new System.Windows.Forms.ComboBox();
            tabs = new System.Windows.Forms.TabControl();
            tabTables = new System.Windows.Forms.TabPage();
            lboxTables = new System.Windows.Forms.ListBox();
            tabSettings = new System.Windows.Forms.TabPage();
            chkIsForDnn = new System.Windows.Forms.CheckBox();
            txtPageSize = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            chkRawNames = new System.Windows.Forms.CheckBox();
            txtNamespace = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            l1 = new System.Windows.Forms.Label();
            btnPath = new System.Windows.Forms.Button();
            txtOutputPath = new System.Windows.Forms.TextBox();
            tabBrowse = new System.Windows.Forms.TabPage();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            cboxBrowseViews = new System.Windows.Forms.ComboBox();
            btnBrowseDN = new System.Windows.Forms.Button();
            btnBrowseUP = new System.Windows.Forms.Button();
            cboxBrowseSortDir = new System.Windows.Forms.ComboBox();
            lboxBrowseColumns = new System.Windows.Forms.ListBox();
            cboxBrowseSortCol = new System.Windows.Forms.ComboBox();
            label2 = new System.Windows.Forms.Label();
            tabDetail = new System.Windows.Forms.TabPage();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            btnDetailTitleAdd = new System.Windows.Forms.Button();
            btnDetailTitleRemove = new System.Windows.Forms.Button();
            btnDetailDN = new System.Windows.Forms.Button();
            btnDetailUP = new System.Windows.Forms.Button();
            lboxDetailColumns = new System.Windows.Forms.ListBox();
            txtDetailEditTitle = new System.Windows.Forms.TextBox();
            tabDetailLookups = new System.Windows.Forms.TabPage();
            tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            lboxDetailLookups = new System.Windows.Forms.ListBox();
            lboxDetailLookupColumns = new System.Windows.Forms.ListBox();
            txtDetailLookupColumns = new System.Windows.Forms.TextBox();
            tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            btnAddLookupColumn = new System.Windows.Forms.Button();
            btnRemoveLookupColumn = new System.Windows.Forms.Button();
            tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            btnLookupColumnClear = new System.Windows.Forms.Button();
            tabDetailGrids = new System.Windows.Forms.TabPage();
            tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            cboxViewKey = new System.Windows.Forms.ComboBox();
            cboxDetailGridSortCol = new System.Windows.Forms.ComboBox();
            tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            btnDetailGridDN = new System.Windows.Forms.Button();
            btnDetailGridUP = new System.Windows.Forms.Button();
            label3 = new System.Windows.Forms.Label();
            btnDetailGridColumnClear = new System.Windows.Forms.Button();
            tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            btnAddDetailGridColumn = new System.Windows.Forms.Button();
            btnRemoveDetailGridColumn = new System.Windows.Forms.Button();
            cboxDetailGridSortDir = new System.Windows.Forms.ComboBox();
            lboxDetailGridColumns = new System.Windows.Forms.ListBox();
            txtDetailGridColumns = new System.Windows.Forms.TextBox();
            chklistDetailGrids = new System.Windows.Forms.CheckedListBox();
            cboxViews = new System.Windows.Forms.ComboBox();
            tabSearch = new System.Windows.Forms.TabPage();
            btnSearchUP = new System.Windows.Forms.Button();
            btnSearchDN = new System.Windows.Forms.Button();
            lboxSearchColumns = new System.Windows.Forms.ListBox();
            tabEdit = new System.Windows.Forms.TabPage();
            lboxEditColumns = new System.Windows.Forms.ListBox();
            btnEditUP = new System.Windows.Forms.Button();
            btnEditDN = new System.Windows.Forms.Button();
            pathFinder = new System.Windows.Forms.FolderBrowserDialog();
            toolTip = new System.Windows.Forms.ToolTip(components);
            tabs.SuspendLayout();
            tabTables.SuspendLayout();
            tabSettings.SuspendLayout();
            tabBrowse.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tabDetail.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tabDetailLookups.SuspendLayout();
            tableLayoutPanel7.SuspendLayout();
            tableLayoutPanel8.SuspendLayout();
            tableLayoutPanel9.SuspendLayout();
            tabDetailGrids.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            tableLayoutPanel6.SuspendLayout();
            tabSearch.SuspendLayout();
            tabEdit.SuspendLayout();
            SuspendLayout();
            // 
            // cboxDatabases
            // 
            cboxDatabases.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            cboxDatabases.FormattingEnabled = true;
            cboxDatabases.Location = new System.Drawing.Point(18, 18);
            cboxDatabases.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cboxDatabases.Name = "cboxDatabases";
            cboxDatabases.Size = new System.Drawing.Size(859, 23);
            cboxDatabases.TabIndex = 0;
            toolTip.SetToolTip(cboxDatabases, "You can switch databases here.");
            cboxDatabases.SelectionChangeCommitted += cboxDatabases_SelectionChangeCommitted;
            // 
            // tabs
            // 
            tabs.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tabs.Controls.Add(tabTables);
            tabs.Controls.Add(tabSettings);
            tabs.Controls.Add(tabBrowse);
            tabs.Controls.Add(tabDetail);
            tabs.Controls.Add(tabDetailLookups);
            tabs.Controls.Add(tabDetailGrids);
            tabs.Controls.Add(tabSearch);
            tabs.Controls.Add(tabEdit);
            tabs.Location = new System.Drawing.Point(18, 50);
            tabs.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabs.Name = "tabs";
            tabs.SelectedIndex = 0;
            tabs.Size = new System.Drawing.Size(860, 595);
            tabs.TabIndex = 1;
            toolTip.SetToolTip(tabs, "Select the columns in the list that you wish to allow users\r\nto Edit on. These columns will be made available to the \r\nusers on the Edit page. ");
            // 
            // tabTables
            // 
            tabTables.Controls.Add(lboxTables);
            tabTables.Location = new System.Drawing.Point(4, 24);
            tabTables.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabTables.Name = "tabTables";
            tabTables.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabTables.Size = new System.Drawing.Size(852, 567);
            tabTables.TabIndex = 0;
            tabTables.Text = "Tables";
            tabTables.UseVisualStyleBackColor = true;
            // 
            // lboxTables
            // 
            lboxTables.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lboxTables.ItemHeight = 15;
            lboxTables.Location = new System.Drawing.Point(14, 17);
            lboxTables.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lboxTables.Name = "lboxTables";
            lboxTables.Size = new System.Drawing.Size(821, 529);
            lboxTables.TabIndex = 3;
            toolTip.SetToolTip(lboxTables, "Please select the main table for this Web Admin page.");
            lboxTables.SelectedIndexChanged += lboxTables_SelectedIndexChanged;
            // 
            // tabSettings
            // 
            tabSettings.Controls.Add(chkIsForDnn);
            tabSettings.Controls.Add(txtPageSize);
            tabSettings.Controls.Add(label4);
            tabSettings.Controls.Add(chkRawNames);
            tabSettings.Controls.Add(txtNamespace);
            tabSettings.Controls.Add(label1);
            tabSettings.Controls.Add(l1);
            tabSettings.Controls.Add(btnPath);
            tabSettings.Controls.Add(txtOutputPath);
            tabSettings.Location = new System.Drawing.Point(4, 24);
            tabSettings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabSettings.Name = "tabSettings";
            tabSettings.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabSettings.Size = new System.Drawing.Size(852, 567);
            tabSettings.TabIndex = 1;
            tabSettings.Text = "Settings";
            tabSettings.UseVisualStyleBackColor = true;
            // 
            // chkIsForDnn
            // 
            chkIsForDnn.AutoSize = true;
            chkIsForDnn.Location = new System.Drawing.Point(115, 137);
            chkIsForDnn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkIsForDnn.Name = "chkIsForDnn";
            chkIsForDnn.Size = new System.Drawing.Size(155, 19);
            chkIsForDnn.TabIndex = 24;
            chkIsForDnn.Text = "Support for DotNetNuke";
            chkIsForDnn.UseVisualStyleBackColor = true;
            // 
            // txtPageSize
            // 
            txtPageSize.Location = new System.Drawing.Point(115, 97);
            txtPageSize.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtPageSize.Name = "txtPageSize";
            txtPageSize.Size = new System.Drawing.Size(130, 23);
            txtPageSize.TabIndex = 23;
            txtPageSize.Text = "20";
            // 
            // label4
            // 
            label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label4.Location = new System.Drawing.Point(22, 97);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(84, 27);
            label4.TabIndex = 22;
            label4.Text = "Page Size:";
            label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkRawNames
            // 
            chkRawNames.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            chkRawNames.Location = new System.Drawing.Point(115, 164);
            chkRawNames.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkRawNames.Name = "chkRawNames";
            chkRawNames.Size = new System.Drawing.Size(205, 28);
            chkRawNames.TabIndex = 20;
            chkRawNames.Text = "Use Raw Database Names";
            // 
            // txtNamespace
            // 
            txtNamespace.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtNamespace.Location = new System.Drawing.Point(115, 59);
            txtNamespace.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtNamespace.Name = "txtNamespace";
            txtNamespace.Size = new System.Drawing.Size(653, 23);
            txtNamespace.TabIndex = 18;
            txtNamespace.Text = "BusinessObjects";
            // 
            // label1
            // 
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label1.Location = new System.Drawing.Point(13, 57);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(93, 27);
            label1.TabIndex = 19;
            label1.Text = "NameSpace:";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // l1
            // 
            l1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            l1.Location = new System.Drawing.Point(13, 20);
            l1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            l1.Name = "l1";
            l1.Size = new System.Drawing.Size(93, 27);
            l1.TabIndex = 17;
            l1.Text = "Output Path:";
            l1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnPath
            // 
            btnPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnPath.Location = new System.Drawing.Point(788, 20);
            btnPath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnPath.Name = "btnPath";
            btnPath.Size = new System.Drawing.Size(33, 28);
            btnPath.TabIndex = 16;
            btnPath.Text = "...";
            btnPath.Click += btnPath_Click;
            // 
            // txtOutputPath
            // 
            txtOutputPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtOutputPath.Location = new System.Drawing.Point(115, 20);
            txtOutputPath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtOutputPath.Name = "txtOutputPath";
            txtOutputPath.Size = new System.Drawing.Size(653, 23);
            txtOutputPath.TabIndex = 15;
            // 
            // tabBrowse
            // 
            tabBrowse.Controls.Add(tableLayoutPanel1);
            tabBrowse.Location = new System.Drawing.Point(4, 24);
            tabBrowse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabBrowse.Name = "tabBrowse";
            tabBrowse.Size = new System.Drawing.Size(852, 567);
            tabBrowse.TabIndex = 2;
            tabBrowse.Text = "Browse";
            tabBrowse.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 5;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.Controls.Add(cboxBrowseViews, 0, 1);
            tableLayoutPanel1.Controls.Add(btnBrowseDN, 4, 3);
            tableLayoutPanel1.Controls.Add(btnBrowseUP, 3, 3);
            tableLayoutPanel1.Controls.Add(cboxBrowseSortDir, 2, 3);
            tableLayoutPanel1.Controls.Add(lboxBrowseColumns, 0, 2);
            tableLayoutPanel1.Controls.Add(cboxBrowseSortCol, 1, 3);
            tableLayoutPanel1.Controls.Add(label2, 0, 3);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.Size = new System.Drawing.Size(852, 567);
            tableLayoutPanel1.TabIndex = 33;
            toolTip.SetToolTip(tableLayoutPanel1, "Choose the direction, Ascending or Descending");
            // 
            // cboxBrowseViews
            // 
            tableLayoutPanel1.SetColumnSpan(cboxBrowseViews, 5);
            cboxBrowseViews.Dock = System.Windows.Forms.DockStyle.Fill;
            cboxBrowseViews.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboxBrowseViews.Location = new System.Drawing.Point(4, 26);
            cboxBrowseViews.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cboxBrowseViews.Name = "cboxBrowseViews";
            cboxBrowseViews.Size = new System.Drawing.Size(844, 23);
            cboxBrowseViews.TabIndex = 32;
            toolTip.SetToolTip(cboxBrowseViews, resources.GetString("cboxBrowseViews.ToolTip"));
            cboxBrowseViews.SelectionChangeCommitted += cboxBrowseViews_SelectionChangeCommitted;
            // 
            // btnBrowseDN
            // 
            btnBrowseDN.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnBrowseDN.Location = new System.Drawing.Point(783, 537);
            btnBrowseDN.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnBrowseDN.Name = "btnBrowseDN";
            btnBrowseDN.Size = new System.Drawing.Size(65, 27);
            btnBrowseDN.TabIndex = 27;
            btnBrowseDN.Text = "Down";
            toolTip.SetToolTip(btnBrowseDN, "Move a column up or down in the ListBox above to control\r\nthe order the columns are diplayed in the main grid");
            btnBrowseDN.Click += btnBrowseDN_Click;
            // 
            // btnBrowseUP
            // 
            btnBrowseUP.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnBrowseUP.Location = new System.Drawing.Point(709, 537);
            btnBrowseUP.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnBrowseUP.Name = "btnBrowseUP";
            btnBrowseUP.Size = new System.Drawing.Size(65, 27);
            btnBrowseUP.TabIndex = 28;
            btnBrowseUP.Text = "Up";
            toolTip.SetToolTip(btnBrowseUP, "Move a column up or down in the ListBox above to control\r\nthe order the columns are diplayed in the main grid");
            btnBrowseUP.Click += btnBrowseUP_Click;
            // 
            // cboxBrowseSortDir
            // 
            cboxBrowseSortDir.Dock = System.Windows.Forms.DockStyle.Fill;
            cboxBrowseSortDir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboxBrowseSortDir.Items.AddRange(new object[] { "Ascending", "Descending" });
            cboxBrowseSortDir.Location = new System.Drawing.Point(514, 537);
            cboxBrowseSortDir.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cboxBrowseSortDir.Name = "cboxBrowseSortDir";
            cboxBrowseSortDir.Size = new System.Drawing.Size(187, 23);
            cboxBrowseSortDir.TabIndex = 31;
            toolTip.SetToolTip(cboxBrowseSortDir, "Choose the Sort Direction");
            // 
            // lboxBrowseColumns
            // 
            tableLayoutPanel1.SetColumnSpan(lboxBrowseColumns, 5);
            lboxBrowseColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            lboxBrowseColumns.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lboxBrowseColumns.ItemHeight = 15;
            lboxBrowseColumns.Location = new System.Drawing.Point(4, 55);
            lboxBrowseColumns.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lboxBrowseColumns.Name = "lboxBrowseColumns";
            lboxBrowseColumns.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            lboxBrowseColumns.Size = new System.Drawing.Size(844, 476);
            lboxBrowseColumns.TabIndex = 26;
            toolTip.SetToolTip(lboxBrowseColumns, resources.GetString("lboxBrowseColumns.ToolTip"));
            lboxBrowseColumns.SelectedIndexChanged += lboxTables_SelectedIndexChanged;
            // 
            // cboxBrowseSortCol
            // 
            cboxBrowseSortCol.Dock = System.Windows.Forms.DockStyle.Fill;
            cboxBrowseSortCol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboxBrowseSortCol.Location = new System.Drawing.Point(59, 537);
            cboxBrowseSortCol.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cboxBrowseSortCol.MinimumSize = new System.Drawing.Size(46, 0);
            cboxBrowseSortCol.Name = "cboxBrowseSortCol";
            cboxBrowseSortCol.Size = new System.Drawing.Size(447, 23);
            cboxBrowseSortCol.TabIndex = 30;
            toolTip.SetToolTip(cboxBrowseSortCol, "This column will determine the sort order of the main grid");
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label2.Location = new System.Drawing.Point(4, 540);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(47, 27);
            label2.TabIndex = 29;
            label2.Text = "Sort:";
            label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tabDetail
            // 
            tabDetail.Controls.Add(tableLayoutPanel2);
            tabDetail.Location = new System.Drawing.Point(4, 24);
            tabDetail.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabDetail.Name = "tabDetail";
            tabDetail.Size = new System.Drawing.Size(852, 567);
            tabDetail.TabIndex = 3;
            tabDetail.Text = "Detail";
            tabDetail.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 5;
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 142F));
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 88F));
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 72F));
            tableLayoutPanel2.Controls.Add(tableLayoutPanel3, 1, 2);
            tableLayoutPanel2.Controls.Add(btnDetailDN, 4, 2);
            tableLayoutPanel2.Controls.Add(btnDetailUP, 3, 2);
            tableLayoutPanel2.Controls.Add(lboxDetailColumns, 0, 1);
            tableLayoutPanel2.Controls.Add(txtDetailEditTitle, 0, 2);
            tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 3;
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 115F));
            tableLayoutPanel2.Size = new System.Drawing.Size(852, 567);
            tableLayoutPanel2.TabIndex = 35;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel3.Controls.Add(btnDetailTitleAdd, 0, 0);
            tableLayoutPanel3.Controls.Add(btnDetailTitleRemove, 0, 1);
            tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel3.Location = new System.Drawing.Point(542, 455);
            tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel3.Size = new System.Drawing.Size(134, 109);
            tableLayoutPanel3.TabIndex = 36;
            // 
            // btnDetailTitleAdd
            // 
            btnDetailTitleAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnDetailTitleAdd.Location = new System.Drawing.Point(4, 46);
            btnDetailTitleAdd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnDetailTitleAdd.Name = "btnDetailTitleAdd";
            btnDetailTitleAdd.Size = new System.Drawing.Size(131, 27);
            btnDetailTitleAdd.TabIndex = 33;
            btnDetailTitleAdd.Text = "Add to Title";
            toolTip.SetToolTip(btnDetailTitleAdd, "Add the selected column above in the ListBox to the Header.");
            btnDetailTitleAdd.Click += btnDetailTitleAdd_Click;
            // 
            // btnDetailTitleRemove
            // 
            btnDetailTitleRemove.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnDetailTitleRemove.Location = new System.Drawing.Point(4, 79);
            btnDetailTitleRemove.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnDetailTitleRemove.Name = "btnDetailTitleRemove";
            btnDetailTitleRemove.Size = new System.Drawing.Size(131, 27);
            btnDetailTitleRemove.TabIndex = 32;
            btnDetailTitleRemove.Text = "Remove from Title";
            toolTip.SetToolTip(btnDetailTitleRemove, "Remove the selected column above in the ListBox from the Header.");
            btnDetailTitleRemove.Click += btnDetailTitleRemove_Click;
            // 
            // btnDetailDN
            // 
            btnDetailDN.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnDetailDN.Location = new System.Drawing.Point(784, 537);
            btnDetailDN.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnDetailDN.Name = "btnDetailDN";
            btnDetailDN.Size = new System.Drawing.Size(64, 27);
            btnDetailDN.TabIndex = 30;
            btnDetailDN.Text = "Down";
            toolTip.SetToolTip(btnDetailDN, "Move a column up or down in the ListBox above to control\r\n in what order the selected column appears on the Detail page.");
            btnDetailDN.Click += btnDetailDN_Click;
            // 
            // btnDetailUP
            // 
            btnDetailUP.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnDetailUP.Location = new System.Drawing.Point(711, 537);
            btnDetailUP.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnDetailUP.Name = "btnDetailUP";
            btnDetailUP.Size = new System.Drawing.Size(65, 27);
            btnDetailUP.TabIndex = 31;
            btnDetailUP.Text = "Up";
            toolTip.SetToolTip(btnDetailUP, "Move a column up or down in the ListBox above to control\r\nin what order the selected column appears on the Detail page.");
            btnDetailUP.Click += btnDetailUP_Click;
            // 
            // lboxDetailColumns
            // 
            tableLayoutPanel2.SetColumnSpan(lboxDetailColumns, 5);
            lboxDetailColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            lboxDetailColumns.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lboxDetailColumns.ItemHeight = 15;
            lboxDetailColumns.Location = new System.Drawing.Point(4, 26);
            lboxDetailColumns.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lboxDetailColumns.Name = "lboxDetailColumns";
            lboxDetailColumns.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            lboxDetailColumns.Size = new System.Drawing.Size(844, 423);
            lboxDetailColumns.TabIndex = 29;
            toolTip.SetToolTip(lboxDetailColumns, resources.GetString("lboxDetailColumns.ToolTip"));
            // 
            // txtDetailEditTitle
            // 
            txtDetailEditTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            txtDetailEditTitle.Location = new System.Drawing.Point(4, 455);
            txtDetailEditTitle.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtDetailEditTitle.Multiline = true;
            txtDetailEditTitle.Name = "txtDetailEditTitle";
            txtDetailEditTitle.ReadOnly = true;
            txtDetailEditTitle.Size = new System.Drawing.Size(530, 109);
            txtDetailEditTitle.TabIndex = 34;
            toolTip.SetToolTip(txtDetailEditTitle, resources.GetString("txtDetailEditTitle.ToolTip"));
            // 
            // tabDetailLookups
            // 
            tabDetailLookups.Controls.Add(tableLayoutPanel7);
            tabDetailLookups.Location = new System.Drawing.Point(4, 24);
            tabDetailLookups.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabDetailLookups.Name = "tabDetailLookups";
            tabDetailLookups.Size = new System.Drawing.Size(852, 567);
            tabDetailLookups.TabIndex = 5;
            tabDetailLookups.Text = "Detail Lookups";
            tabDetailLookups.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel7
            // 
            tableLayoutPanel7.ColumnCount = 2;
            tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            tableLayoutPanel7.Controls.Add(lboxDetailLookups, 0, 1);
            tableLayoutPanel7.Controls.Add(lboxDetailLookupColumns, 0, 2);
            tableLayoutPanel7.Controls.Add(txtDetailLookupColumns, 0, 3);
            tableLayoutPanel7.Controls.Add(tableLayoutPanel8, 1, 2);
            tableLayoutPanel7.Controls.Add(tableLayoutPanel9, 1, 3);
            tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel7.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel7.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel7.Name = "tableLayoutPanel7";
            tableLayoutPanel7.RowCount = 4;
            tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            tableLayoutPanel7.Size = new System.Drawing.Size(852, 567);
            tableLayoutPanel7.TabIndex = 33;
            // 
            // lboxDetailLookups
            // 
            lboxDetailLookups.Dock = System.Windows.Forms.DockStyle.Fill;
            lboxDetailLookups.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lboxDetailLookups.ItemHeight = 15;
            lboxDetailLookups.Location = new System.Drawing.Point(4, 26);
            lboxDetailLookups.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lboxDetailLookups.Name = "lboxDetailLookups";
            lboxDetailLookups.Size = new System.Drawing.Size(762, 228);
            lboxDetailLookups.TabIndex = 27;
            toolTip.SetToolTip(lboxDetailLookups, resources.GetString("lboxDetailLookups.ToolTip"));
            lboxDetailLookups.SelectedIndexChanged += lboxDetailLookups_SelectedIndexChanged;
            // 
            // lboxDetailLookupColumns
            // 
            lboxDetailLookupColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            lboxDetailLookupColumns.ItemHeight = 15;
            lboxDetailLookupColumns.Location = new System.Drawing.Point(4, 260);
            lboxDetailLookupColumns.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lboxDetailLookupColumns.Name = "lboxDetailLookupColumns";
            lboxDetailLookupColumns.Size = new System.Drawing.Size(762, 228);
            lboxDetailLookupColumns.TabIndex = 28;
            toolTip.SetToolTip(lboxDetailLookupColumns, "Replace the column you have selected above with one or more\r\ncolumns from this List.");
            // 
            // txtDetailLookupColumns
            // 
            txtDetailLookupColumns.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtDetailLookupColumns.Location = new System.Drawing.Point(4, 494);
            txtDetailLookupColumns.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtDetailLookupColumns.Multiline = true;
            txtDetailLookupColumns.Name = "txtDetailLookupColumns";
            txtDetailLookupColumns.ReadOnly = true;
            txtDetailLookupColumns.Size = new System.Drawing.Size(762, 70);
            txtDetailLookupColumns.TabIndex = 29;
            // 
            // tableLayoutPanel8
            // 
            tableLayoutPanel8.ColumnCount = 1;
            tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel8.Controls.Add(btnAddLookupColumn, 0, 0);
            tableLayoutPanel8.Controls.Add(btnRemoveLookupColumn, 0, 1);
            tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel8.Location = new System.Drawing.Point(774, 260);
            tableLayoutPanel8.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel8.Name = "tableLayoutPanel8";
            tableLayoutPanel8.RowCount = 2;
            tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel8.Size = new System.Drawing.Size(74, 228);
            tableLayoutPanel8.TabIndex = 30;
            // 
            // btnAddLookupColumn
            // 
            btnAddLookupColumn.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnAddLookupColumn.Location = new System.Drawing.Point(5, 3);
            btnAddLookupColumn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnAddLookupColumn.Name = "btnAddLookupColumn";
            btnAddLookupColumn.Size = new System.Drawing.Size(65, 27);
            btnAddLookupColumn.TabIndex = 30;
            btnAddLookupColumn.Text = "Add";
            toolTip.SetToolTip(btnAddLookupColumn, "Use this column as a replacement");
            btnAddLookupColumn.Click += btnAddLookupColumn_Click;
            // 
            // btnRemoveLookupColumn
            // 
            btnRemoveLookupColumn.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnRemoveLookupColumn.Location = new System.Drawing.Point(5, 36);
            btnRemoveLookupColumn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnRemoveLookupColumn.Name = "btnRemoveLookupColumn";
            btnRemoveLookupColumn.Size = new System.Drawing.Size(65, 27);
            btnRemoveLookupColumn.TabIndex = 31;
            btnRemoveLookupColumn.Text = "Remove";
            toolTip.SetToolTip(btnRemoveLookupColumn, "Remove this column from the replacement list");
            btnRemoveLookupColumn.Click += btnRemoveLookupColumn_Click;
            // 
            // tableLayoutPanel9
            // 
            tableLayoutPanel9.ColumnCount = 1;
            tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel9.Controls.Add(btnLookupColumnClear, 0, 0);
            tableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel9.Location = new System.Drawing.Point(774, 494);
            tableLayoutPanel9.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel9.Name = "tableLayoutPanel9";
            tableLayoutPanel9.RowCount = 1;
            tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel9.Size = new System.Drawing.Size(74, 70);
            tableLayoutPanel9.TabIndex = 31;
            // 
            // btnLookupColumnClear
            // 
            btnLookupColumnClear.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnLookupColumnClear.Location = new System.Drawing.Point(5, 3);
            btnLookupColumnClear.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnLookupColumnClear.Name = "btnLookupColumnClear";
            btnLookupColumnClear.Size = new System.Drawing.Size(65, 27);
            btnLookupColumnClear.TabIndex = 32;
            btnLookupColumnClear.Text = "Clear";
            toolTip.SetToolTip(btnLookupColumnClear, "Clear all columns from the list");
            btnLookupColumnClear.Click += btnLookupColumnClear_Click;
            // 
            // tabDetailGrids
            // 
            tabDetailGrids.Controls.Add(tableLayoutPanel4);
            tabDetailGrids.Location = new System.Drawing.Point(4, 24);
            tabDetailGrids.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabDetailGrids.Name = "tabDetailGrids";
            tabDetailGrids.Size = new System.Drawing.Size(852, 567);
            tabDetailGrids.TabIndex = 4;
            tabDetailGrids.Text = "Relationships";
            tabDetailGrids.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 4;
            tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 57F));
            tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 212F));
            tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            tableLayoutPanel4.Controls.Add(cboxViewKey, 2, 2);
            tableLayoutPanel4.Controls.Add(cboxDetailGridSortCol, 1, 5);
            tableLayoutPanel4.Controls.Add(tableLayoutPanel5, 3, 1);
            tableLayoutPanel4.Controls.Add(label3, 0, 5);
            tableLayoutPanel4.Controls.Add(btnDetailGridColumnClear, 3, 4);
            tableLayoutPanel4.Controls.Add(tableLayoutPanel6, 3, 3);
            tableLayoutPanel4.Controls.Add(cboxDetailGridSortDir, 2, 5);
            tableLayoutPanel4.Controls.Add(lboxDetailGridColumns, 0, 3);
            tableLayoutPanel4.Controls.Add(txtDetailGridColumns, 0, 4);
            tableLayoutPanel4.Controls.Add(chklistDetailGrids, 0, 1);
            tableLayoutPanel4.Controls.Add(cboxViews, 0, 2);
            tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 6;
            tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            tableLayoutPanel4.Size = new System.Drawing.Size(852, 567);
            tableLayoutPanel4.TabIndex = 51;
            // 
            // cboxViewKey
            // 
            cboxViewKey.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            cboxViewKey.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboxViewKey.Enabled = false;
            cboxViewKey.Location = new System.Drawing.Point(562, 185);
            cboxViewKey.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cboxViewKey.Name = "cboxViewKey";
            cboxViewKey.Size = new System.Drawing.Size(204, 23);
            cboxViewKey.TabIndex = 50;
            toolTip.SetToolTip(cboxViewKey, resources.GetString("cboxViewKey.ToolTip"));
            cboxViewKey.SelectionChangeCommitted += cboxViewKey_SelectionChangeCommitted;
            // 
            // cboxDetailGridSortCol
            // 
            cboxDetailGridSortCol.Dock = System.Windows.Forms.DockStyle.Fill;
            cboxDetailGridSortCol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboxDetailGridSortCol.Location = new System.Drawing.Point(61, 534);
            cboxDetailGridSortCol.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cboxDetailGridSortCol.Name = "cboxDetailGridSortCol";
            cboxDetailGridSortCol.Size = new System.Drawing.Size(493, 23);
            cboxDetailGridSortCol.TabIndex = 46;
            toolTip.SetToolTip(cboxDetailGridSortCol, "Choose the column you wish to sort the relationship grid on.");
            cboxDetailGridSortCol.SelectionChangeCommitted += cboxDetailGridSortCol_SelectionChangeCommitted;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 1;
            tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel5.Controls.Add(btnDetailGridDN, 0, 1);
            tableLayoutPanel5.Controls.Add(btnDetailGridUP, 0, 0);
            tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel5.Location = new System.Drawing.Point(774, 26);
            tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 2;
            tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel5.Size = new System.Drawing.Size(74, 153);
            tableLayoutPanel5.TabIndex = 49;
            // 
            // btnDetailGridDN
            // 
            btnDetailGridDN.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnDetailGridDN.Location = new System.Drawing.Point(5, 36);
            btnDetailGridDN.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnDetailGridDN.Name = "btnDetailGridDN";
            btnDetailGridDN.Size = new System.Drawing.Size(65, 27);
            btnDetailGridDN.TabIndex = 38;
            btnDetailGridDN.Text = "Down";
            toolTip.SetToolTip(btnDetailGridDN, "Control the order that the Relationship grids are displayed");
            btnDetailGridDN.Click += btnDetailGridDN_Click;
            // 
            // btnDetailGridUP
            // 
            btnDetailGridUP.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnDetailGridUP.Location = new System.Drawing.Point(5, 3);
            btnDetailGridUP.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnDetailGridUP.Name = "btnDetailGridUP";
            btnDetailGridUP.Size = new System.Drawing.Size(65, 27);
            btnDetailGridUP.TabIndex = 39;
            btnDetailGridUP.Text = "Up";
            toolTip.SetToolTip(btnDetailGridUP, "Control the order that the Relationship grids are displayed");
            btnDetailGridUP.Click += btnDetailGridUP_Click;
            // 
            // label3
            // 
            label3.Dock = System.Windows.Forms.DockStyle.Fill;
            label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label3.Location = new System.Drawing.Point(4, 531);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(49, 36);
            label3.TabIndex = 45;
            label3.Text = "Sort:";
            label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnDetailGridColumnClear
            // 
            btnDetailGridColumnClear.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnDetailGridColumnClear.Location = new System.Drawing.Point(783, 374);
            btnDetailGridColumnClear.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnDetailGridColumnClear.Name = "btnDetailGridColumnClear";
            btnDetailGridColumnClear.Size = new System.Drawing.Size(65, 27);
            btnDetailGridColumnClear.TabIndex = 44;
            btnDetailGridColumnClear.Text = "Clear";
            toolTip.SetToolTip(btnDetailGridColumnClear, "Remove all of the columns from the Relationship grid list.");
            btnDetailGridColumnClear.Click += btnDetailGridColumnClear_Click;
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.ColumnCount = 1;
            tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel6.Controls.Add(btnAddDetailGridColumn, 0, 0);
            tableLayoutPanel6.Controls.Add(btnRemoveDetailGridColumn, 0, 1);
            tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel6.Location = new System.Drawing.Point(774, 214);
            tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 2;
            tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel6.Size = new System.Drawing.Size(74, 154);
            tableLayoutPanel6.TabIndex = 51;
            // 
            // btnAddDetailGridColumn
            // 
            btnAddDetailGridColumn.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnAddDetailGridColumn.Location = new System.Drawing.Point(5, 3);
            btnAddDetailGridColumn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnAddDetailGridColumn.Name = "btnAddDetailGridColumn";
            btnAddDetailGridColumn.Size = new System.Drawing.Size(65, 27);
            btnAddDetailGridColumn.TabIndex = 43;
            btnAddDetailGridColumn.Text = "Add";
            toolTip.SetToolTip(btnAddDetailGridColumn, "Display the selected column in the Relationship grid");
            btnAddDetailGridColumn.Click += btnAddDetailGridColumn_Click;
            // 
            // btnRemoveDetailGridColumn
            // 
            btnRemoveDetailGridColumn.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnRemoveDetailGridColumn.Location = new System.Drawing.Point(5, 36);
            btnRemoveDetailGridColumn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnRemoveDetailGridColumn.Name = "btnRemoveDetailGridColumn";
            btnRemoveDetailGridColumn.Size = new System.Drawing.Size(65, 27);
            btnRemoveDetailGridColumn.TabIndex = 42;
            btnRemoveDetailGridColumn.Text = "Remove";
            toolTip.SetToolTip(btnRemoveDetailGridColumn, "Remove the selected column from the Relationship grid");
            btnRemoveDetailGridColumn.Click += btnRemoveDetailGridColumn_Click;
            // 
            // cboxDetailGridSortDir
            // 
            cboxDetailGridSortDir.Dock = System.Windows.Forms.DockStyle.Fill;
            cboxDetailGridSortDir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboxDetailGridSortDir.Items.AddRange(new object[] { "Ascending", "Descending" });
            cboxDetailGridSortDir.Location = new System.Drawing.Point(562, 534);
            cboxDetailGridSortDir.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cboxDetailGridSortDir.Name = "cboxDetailGridSortDir";
            cboxDetailGridSortDir.Size = new System.Drawing.Size(204, 23);
            cboxDetailGridSortDir.TabIndex = 47;
            toolTip.SetToolTip(cboxDetailGridSortDir, "Choose the sort direction.");
            cboxDetailGridSortDir.SelectionChangeCommitted += cboxDetailGridSortDir_SelectionChangeCommitted;
            // 
            // lboxDetailGridColumns
            // 
            tableLayoutPanel4.SetColumnSpan(lboxDetailGridColumns, 3);
            lboxDetailGridColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            lboxDetailGridColumns.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lboxDetailGridColumns.ItemHeight = 15;
            lboxDetailGridColumns.Location = new System.Drawing.Point(4, 214);
            lboxDetailGridColumns.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lboxDetailGridColumns.Name = "lboxDetailGridColumns";
            lboxDetailGridColumns.Size = new System.Drawing.Size(762, 154);
            lboxDetailGridColumns.TabIndex = 40;
            // 
            // txtDetailGridColumns
            // 
            tableLayoutPanel4.SetColumnSpan(txtDetailGridColumns, 3);
            txtDetailGridColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            txtDetailGridColumns.Location = new System.Drawing.Point(4, 374);
            txtDetailGridColumns.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtDetailGridColumns.Multiline = true;
            txtDetailGridColumns.Name = "txtDetailGridColumns";
            txtDetailGridColumns.ReadOnly = true;
            txtDetailGridColumns.Size = new System.Drawing.Size(762, 154);
            txtDetailGridColumns.TabIndex = 41;
            toolTip.SetToolTip(txtDetailGridColumns, "The columns you have added to this TextBox will be displayed \r\nin the relationship grid which is shown on the Detail page.");
            // 
            // chklistDetailGrids
            // 
            tableLayoutPanel4.SetColumnSpan(chklistDetailGrids, 3);
            chklistDetailGrids.Dock = System.Windows.Forms.DockStyle.Fill;
            chklistDetailGrids.Location = new System.Drawing.Point(4, 26);
            chklistDetailGrids.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chklistDetailGrids.Name = "chklistDetailGrids";
            chklistDetailGrids.Size = new System.Drawing.Size(762, 153);
            chklistDetailGrids.TabIndex = 48;
            toolTip.SetToolTip(chklistDetailGrids, resources.GetString("chklistDetailGrids.ToolTip"));
            chklistDetailGrids.SelectedIndexChanged += chklistDetailGrids_SelectedIndexChanged;
            // 
            // cboxViews
            // 
            tableLayoutPanel4.SetColumnSpan(cboxViews, 2);
            cboxViews.Dock = System.Windows.Forms.DockStyle.Fill;
            cboxViews.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboxViews.Enabled = false;
            cboxViews.Location = new System.Drawing.Point(4, 185);
            cboxViews.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cboxViews.Name = "cboxViews";
            cboxViews.Size = new System.Drawing.Size(550, 23);
            cboxViews.TabIndex = 49;
            toolTip.SetToolTip(cboxViews, resources.GetString("cboxViews.ToolTip"));
            cboxViews.SelectionChangeCommitted += cboxViews_SelectionChangeCommitted;
            // 
            // tabSearch
            // 
            tabSearch.Controls.Add(btnSearchUP);
            tabSearch.Controls.Add(btnSearchDN);
            tabSearch.Controls.Add(lboxSearchColumns);
            tabSearch.Location = new System.Drawing.Point(4, 24);
            tabSearch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabSearch.Name = "tabSearch";
            tabSearch.Size = new System.Drawing.Size(852, 567);
            tabSearch.TabIndex = 6;
            tabSearch.Text = "Search";
            tabSearch.UseVisualStyleBackColor = true;
            // 
            // btnSearchUP
            // 
            btnSearchUP.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnSearchUP.Location = new System.Drawing.Point(695, 515);
            btnSearchUP.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSearchUP.Name = "btnSearchUP";
            btnSearchUP.Size = new System.Drawing.Size(65, 27);
            btnSearchUP.TabIndex = 26;
            btnSearchUP.Text = "Up";
            toolTip.SetToolTip(btnSearchUP, "Control the order of the columns as they appear on the Search page.");
            btnSearchUP.Click += btnSearchUP_Click;
            // 
            // btnSearchDN
            // 
            btnSearchDN.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnSearchDN.Location = new System.Drawing.Point(770, 515);
            btnSearchDN.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSearchDN.Name = "btnSearchDN";
            btnSearchDN.Size = new System.Drawing.Size(65, 27);
            btnSearchDN.TabIndex = 25;
            btnSearchDN.Text = "Down";
            toolTip.SetToolTip(btnSearchDN, "Control the order of the columns as they appear on the Search page.");
            btnSearchDN.Click += btnSearchDN_Click;
            // 
            // lboxSearchColumns
            // 
            lboxSearchColumns.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lboxSearchColumns.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lboxSearchColumns.ItemHeight = 15;
            lboxSearchColumns.Location = new System.Drawing.Point(14, 25);
            lboxSearchColumns.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lboxSearchColumns.Name = "lboxSearchColumns";
            lboxSearchColumns.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            lboxSearchColumns.Size = new System.Drawing.Size(821, 469);
            lboxSearchColumns.TabIndex = 24;
            toolTip.SetToolTip(lboxSearchColumns, resources.GetString("lboxSearchColumns.ToolTip"));
            // 
            // tabEdit
            // 
            tabEdit.Controls.Add(lboxEditColumns);
            tabEdit.Controls.Add(btnEditUP);
            tabEdit.Controls.Add(btnEditDN);
            tabEdit.Location = new System.Drawing.Point(4, 24);
            tabEdit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabEdit.Name = "tabEdit";
            tabEdit.Size = new System.Drawing.Size(852, 567);
            tabEdit.TabIndex = 7;
            tabEdit.Text = "Edit";
            tabEdit.UseVisualStyleBackColor = true;
            // 
            // lboxEditColumns
            // 
            lboxEditColumns.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lboxEditColumns.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lboxEditColumns.ItemHeight = 15;
            lboxEditColumns.Location = new System.Drawing.Point(14, 25);
            lboxEditColumns.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lboxEditColumns.Name = "lboxEditColumns";
            lboxEditColumns.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            lboxEditColumns.Size = new System.Drawing.Size(821, 469);
            lboxEditColumns.TabIndex = 27;
            // 
            // btnEditUP
            // 
            btnEditUP.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnEditUP.Location = new System.Drawing.Point(695, 515);
            btnEditUP.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnEditUP.Name = "btnEditUP";
            btnEditUP.Size = new System.Drawing.Size(65, 27);
            btnEditUP.TabIndex = 29;
            btnEditUP.Text = "Up";
            toolTip.SetToolTip(btnEditUP, "Control the order of the columns as they appear on the Search page.");
            btnEditUP.Click += btnEditUP_Click;
            // 
            // btnEditDN
            // 
            btnEditDN.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnEditDN.Location = new System.Drawing.Point(770, 515);
            btnEditDN.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnEditDN.Name = "btnEditDN";
            btnEditDN.Size = new System.Drawing.Size(65, 27);
            btnEditDN.TabIndex = 28;
            btnEditDN.Text = "Down";
            toolTip.SetToolTip(btnEditDN, "Control the order of the columns as they appear on the Search page.");
            btnEditDN.Click += btnEditDN_Click;
            // 
            // toolTip
            // 
            toolTip.AutoPopDelay = 30000;
            toolTip.InitialDelay = 500;
            toolTip.ReshowDelay = 100;
            toolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            toolTip.ToolTipTitle = "More Information";
            // 
            // AdminTemplateSuite
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.Window;
            Controls.Add(tabs);
            Controls.Add(cboxDatabases);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "AdminTemplateSuite";
            Size = new System.Drawing.Size(894, 668);
            Load += AdminTemplateSuite_Load;
            tabs.ResumeLayout(false);
            tabTables.ResumeLayout(false);
            tabSettings.ResumeLayout(false);
            tabSettings.PerformLayout();
            tabBrowse.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tabDetail.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            tabDetailLookups.ResumeLayout(false);
            tableLayoutPanel7.ResumeLayout(false);
            tableLayoutPanel7.PerformLayout();
            tableLayoutPanel8.ResumeLayout(false);
            tableLayoutPanel9.ResumeLayout(false);
            tabDetailGrids.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel6.ResumeLayout(false);
            tabSearch.ResumeLayout(false);
            tabEdit.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboxDatabases;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabTables;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.TabPage tabBrowse;
        private System.Windows.Forms.TabPage tabDetail;
        private System.Windows.Forms.TabPage tabDetailGrids;
        private System.Windows.Forms.TabPage tabDetailLookups;
        private System.Windows.Forms.TabPage tabSearch;
        private System.Windows.Forms.TabPage tabEdit;
        private System.Windows.Forms.ListBox lboxTables;
        private System.Windows.Forms.TextBox txtPageSize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkRawNames;
        private System.Windows.Forms.TextBox txtNamespace;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label l1;
        private System.Windows.Forms.Button btnPath;
        private System.Windows.Forms.TextBox txtOutputPath;
        private System.Windows.Forms.ComboBox cboxBrowseViews;
        private System.Windows.Forms.ComboBox cboxBrowseSortDir;
        private System.Windows.Forms.ComboBox cboxBrowseSortCol;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBrowseUP;
        private System.Windows.Forms.Button btnBrowseDN;
        private System.Windows.Forms.ListBox lboxBrowseColumns;
        private System.Windows.Forms.TextBox txtDetailEditTitle;
        private System.Windows.Forms.Button btnDetailTitleAdd;
        private System.Windows.Forms.Button btnDetailTitleRemove;
        private System.Windows.Forms.Button btnDetailUP;
        private System.Windows.Forms.Button btnDetailDN;
        private System.Windows.Forms.ListBox lboxDetailColumns;
        private System.Windows.Forms.ComboBox cboxViewKey;
        private System.Windows.Forms.ComboBox cboxViews;
        private System.Windows.Forms.CheckedListBox chklistDetailGrids;
        private System.Windows.Forms.ComboBox cboxDetailGridSortDir;
        private System.Windows.Forms.ComboBox cboxDetailGridSortCol;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnDetailGridColumnClear;
        private System.Windows.Forms.Button btnAddDetailGridColumn;
        private System.Windows.Forms.TextBox txtDetailGridColumns;
        private System.Windows.Forms.ListBox lboxDetailGridColumns;
        private System.Windows.Forms.Button btnRemoveDetailGridColumn;
        private System.Windows.Forms.Button btnDetailGridUP;
        private System.Windows.Forms.Button btnDetailGridDN;
        private System.Windows.Forms.Button btnLookupColumnClear;
        private System.Windows.Forms.Button btnAddLookupColumn;
        private System.Windows.Forms.TextBox txtDetailLookupColumns;
        private System.Windows.Forms.ListBox lboxDetailLookupColumns;
        private System.Windows.Forms.ListBox lboxDetailLookups;
        private System.Windows.Forms.Button btnRemoveLookupColumn;
        private System.Windows.Forms.Button btnSearchUP;
        private System.Windows.Forms.Button btnSearchDN;
        private System.Windows.Forms.ListBox lboxSearchColumns;
        private System.Windows.Forms.ListBox lboxEditColumns;
        private System.Windows.Forms.Button btnEditUP;
        private System.Windows.Forms.Button btnEditDN;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel9;
        private System.Windows.Forms.FolderBrowserDialog pathFinder;
        private System.Windows.Forms.CheckBox chkIsForDnn;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
