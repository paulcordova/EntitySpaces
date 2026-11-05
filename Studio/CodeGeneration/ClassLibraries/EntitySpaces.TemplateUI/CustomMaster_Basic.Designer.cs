namespace EntitySpaces.TemplateUI
{
    partial class CustomMaster_Basic
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
            label6 = new System.Windows.Forms.Label();
            lboxTablesViews = new System.Windows.Forms.ListBox();
            cboTablesViews = new System.Windows.Forms.ComboBox();
            cboDatabase = new System.Windows.Forms.ComboBox();
            label5 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            txtNamespace = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            btnOutputPath = new System.Windows.Forms.Button();
            txtOutputPath = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label6.Location = new System.Drawing.Point(21, 209);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(72, 13);
            label6.TabIndex = 22;
            label6.Text = "Select Tables";
            // 
            // lboxTablesViews
            // 
            lboxTablesViews.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lboxTablesViews.FormattingEnabled = true;
            lboxTablesViews.IntegralHeight = false;
            lboxTablesViews.Location = new System.Drawing.Point(24, 237);
            lboxTablesViews.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lboxTablesViews.Name = "lboxTablesViews";
            lboxTablesViews.ScrollAlwaysVisible = true;
            lboxTablesViews.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            lboxTablesViews.Size = new System.Drawing.Size(397, 214);
            lboxTablesViews.TabIndex = 21;
            lboxTablesViews.KeyDown += lboxTablesViews_KeyDown;
            // 
            // cboTablesViews
            // 
            cboTablesViews.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            cboTablesViews.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboTablesViews.Location = new System.Drawing.Point(24, 181);
            cboTablesViews.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cboTablesViews.Name = "cboTablesViews";
            cboTablesViews.Size = new System.Drawing.Size(397, 23);
            cboTablesViews.TabIndex = 19;
            cboTablesViews.SelectedIndexChanged += cboTablesViews_SelectedIndexChanged;
            // 
            // cboDatabase
            // 
            cboDatabase.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            cboDatabase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboDatabase.Location = new System.Drawing.Point(24, 135);
            cboDatabase.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cboDatabase.Name = "cboDatabase";
            cboDatabase.Size = new System.Drawing.Size(397, 23);
            cboDatabase.TabIndex = 20;
            cboDatabase.SelectionChangeCommitted += cboDatabase_SelectionChangeCommitted;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label5.Location = new System.Drawing.Point(21, 163);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(82, 13);
            label5.TabIndex = 17;
            label5.Text = "Tables or Views";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label4.Location = new System.Drawing.Point(21, 115);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(95, 13);
            label4.TabIndex = 16;
            label4.Text = "Select a Database";
            // 
            // txtNamespace
            // 
            txtNamespace.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtNamespace.Location = new System.Drawing.Point(24, 87);
            txtNamespace.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtNamespace.Name = "txtNamespace";
            txtNamespace.Size = new System.Drawing.Size(397, 23);
            txtNamespace.TabIndex = 14;
            txtNamespace.Text = "BusinessObjects";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label2.Location = new System.Drawing.Point(21, 66);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(64, 13);
            label2.TabIndex = 13;
            label2.Text = "Namespace";
            // 
            // btnOutputPath
            // 
            btnOutputPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnOutputPath.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            btnOutputPath.Location = new System.Drawing.Point(429, 36);
            btnOutputPath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnOutputPath.Name = "btnOutputPath";
            btnOutputPath.Size = new System.Drawing.Size(37, 27);
            btnOutputPath.TabIndex = 12;
            btnOutputPath.Text = "...";
            btnOutputPath.UseVisualStyleBackColor = true;
            btnOutputPath.Click += btnOutputPath_Click;
            // 
            // txtOutputPath
            // 
            txtOutputPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtOutputPath.Location = new System.Drawing.Point(24, 39);
            txtOutputPath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtOutputPath.Name = "txtOutputPath";
            txtOutputPath.Size = new System.Drawing.Size(397, 23);
            txtOutputPath.TabIndex = 10;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label1.Location = new System.Drawing.Point(21, 21);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(115, 13);
            label1.TabIndex = 11;
            label1.Text = "Select the Output Path";
            // 
            // CustomMaster_Basic
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.Window;
            Controls.Add(label6);
            Controls.Add(lboxTablesViews);
            Controls.Add(cboTablesViews);
            Controls.Add(cboDatabase);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(txtNamespace);
            Controls.Add(label2);
            Controls.Add(btnOutputPath);
            Controls.Add(txtOutputPath);
            Controls.Add(label1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "CustomMaster_Basic";
            Size = new System.Drawing.Size(470, 464);
            Load += CustomMaster_Basic_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox lboxTablesViews;
        private System.Windows.Forms.ComboBox cboTablesViews;
        private System.Windows.Forms.ComboBox cboDatabase;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtNamespace;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOutputPath;
        private System.Windows.Forms.TextBox txtOutputPath;
        private System.Windows.Forms.Label label1;
    }
}
