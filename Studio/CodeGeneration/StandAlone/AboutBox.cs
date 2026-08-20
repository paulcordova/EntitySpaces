using EntitySpaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms;

namespace EntitySpaces
{
    public partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();

            this.label2.Text = $"Version {VersionInfo.Version}";
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void linkLabel1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://github.com/paulcordova/EntitySpaces/") { UseShellExecute = true });
            }
            catch { }
        }
    }
}
