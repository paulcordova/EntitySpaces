using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EntitySpaces;

using Krypton.Toolkit;

namespace EntitySpaces
{
    public partial class AboutBox : KryptonForm
    {
        public AboutBox()
        {
            InitializeComponent();

            this.label2.Text = $"Version {VersionInfo.Version}";

            this.label1.Text = $".NET Version {Environment.Version.ToString()}";

            Kryptoncolors();
        }

        private void Kryptoncolors()
        {
            // Set labels to transparent to inherit the form's background color
            this.label1.BackColor = Color.Transparent;
            this.label2.BackColor = Color.Transparent;
            this.label3.BackColor = Color.Transparent;

            // Retrieve the exact background color from the active Krypton palette
            Color kryptonBackColor = KryptonManager.CurrentGlobalPalette.GetBackColor1(PaletteBackStyle.PanelClient, PaletteState.Normal);

            // Apply the palette color to controls that don't support transparency properly
            this.pictureBox1.BackColor = kryptonBackColor;
            this.richTextBox1.BackColor = kryptonBackColor;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://mikegriffinreborn.github.io/EntitySpaces/");
            }
            catch { }
        }
    }
}
