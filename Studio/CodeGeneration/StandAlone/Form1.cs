using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System;

using System.Reflection;

using EntitySpaces.AddIn;
using EntitySpaces;

namespace EntitySpaces
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.Text = $"{VersionInfo.ProductName} - Persistence Layer and Business Objects for Microsoft .NET";

            // Fires once per app launch. Silent, async, never blocks startup.
            TelemetryHelper.PingLaunch();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutBox aboutBox = new AboutBox())
            {
                aboutBox.ShowDialog();
            }
        }

        private void feedBackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FeedbackBox feedbackBox = new FeedbackBox())
            {
                feedbackBox.ShowDialog();
            }
        }
    }
}
