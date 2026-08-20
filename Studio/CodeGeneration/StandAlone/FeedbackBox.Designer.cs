namespace EntitySpaces
{
    partial class FeedbackBox
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.LinkLabel linkSurvey;
        private System.Windows.Forms.LinkLabel linkSupport;
        private System.Windows.Forms.Label lblDisclosure;
        private System.Windows.Forms.Button btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.linkSurvey = new System.Windows.Forms.LinkLabel();
            this.linkSupport = new System.Windows.Forms.LinkLabel();
            this.lblDisclosure = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 18);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(160, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "❤️ Feedback & Support";
            //
            // lblMessage
            //
            this.lblMessage.Location = new System.Drawing.Point(20, 55);
            this.lblMessage.MaximumSize = new System.Drawing.Size(360, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(360, 60);
            this.lblMessage.TabIndex = 1;
            this.lblMessage.Text = "Using EntitySpaces Studio for new projects, legacy maintenance, or just " +
    "checking it out? A 2-minute survey helps me prioritize what to build next.";
            //
            // linkSurvey
            //
            this.linkSurvey.AutoSize = true;
            this.linkSurvey.Location = new System.Drawing.Point(20, 125);
            this.linkSurvey.Name = "linkSurvey";
            this.linkSurvey.Size = new System.Drawing.Size(110, 15);
            this.linkSurvey.TabIndex = 2;
            this.linkSurvey.TabStop = true;
            this.linkSurvey.Text = "📋 Take the survey";
            this.linkSurvey.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkSurvey_LinkClicked);
            //
            // linkSupport
            //
            this.linkSupport.AutoSize = true;
            this.linkSupport.Location = new System.Drawing.Point(20, 150);
            this.linkSupport.Name = "linkSupport";
            this.linkSupport.Size = new System.Drawing.Size(150, 15);
            this.linkSupport.TabIndex = 3;
            this.linkSupport.TabStop = true;
            this.linkSupport.Text = "❤️ Support this project";
            this.linkSupport.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkSupport_LinkClicked);
            //
            // lblDisclosure
            //
            this.lblDisclosure.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblDisclosure.Font = new System.Drawing.Font("Segoe UI", 7.5F);
            this.lblDisclosure.Location = new System.Drawing.Point(20, 190);
            this.lblDisclosure.MaximumSize = new System.Drawing.Size(360, 0);
            this.lblDisclosure.Name = "lblDisclosure";
            this.lblDisclosure.Size = new System.Drawing.Size(360, 55);
            this.lblDisclosure.TabIndex = 4;
            this.lblDisclosure.Text = "Opening this app sends an anonymous ping (city-level location, derived " +
    "from your IP address) to help me see how many people use it — no personal data is collected.";
            //
            // btnClose
            //
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.btnClose.Location = new System.Drawing.Point(305, 245);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // FeedbackBox
            //
            this.AcceptButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(400, 290);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblDisclosure);
            this.Controls.Add(this.linkSupport);
            this.Controls.Add(this.linkSurvey);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FeedbackBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Feedback & Support";
            this.Load += new System.EventHandler(this.FeedbackBox_Load);
            this.ResumeLayout(false);
        }
    }
}
