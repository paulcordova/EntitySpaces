using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Krypton.Toolkit;

namespace EntitySpaces
{
    /// <summary>
    /// Feedback &amp; Support screen, opened from Help → Feedback.
    /// Follows the same usage pattern as AboutBox (new instance, ShowDialog, dispose).
    /// </summary>
    public partial class FeedbackBox : KryptonForm
    {
        private const string SurveyUrl = "https://docs.google.com/forms/d/e/1FAIpQLSd-FVQiC3deoaIarYnsOCH4pdj-4zjGKznN68uUtyx9CpuKgA/viewform?usp=header";
        private const string SupportUrl = "https://netstep.cl/entityspaces/support/";

        public FeedbackBox()
        {
            InitializeComponent();

            KryptonColors();
        }

        private void KryptonColors()
        {
            // Set labels and link labels to transparent to inherit the form's background color
            this.lblTitle.BackColor = Color.Transparent;
            this.lblMessage.BackColor = Color.Transparent;
            this.lblDisclosure.BackColor = Color.Transparent;
            this.linkSupport.BackColor = Color.Transparent;
            this.linkSurvey.BackColor = Color.Transparent;

            // Retrieve the exact background color from the active Krypton palette
            Color kryptonBackColor = KryptonManager.CurrentGlobalPalette.GetBackColor1(PaletteBackStyle.PanelClient, PaletteState.Normal);

            // Apply the palette color to the button 
            // Note: If btnClose is a standard WinForms button, this might be overridden by FlatStyle or UseVisualStyleBackColor
            this.btnClose.BackColor = kryptonBackColor;
        }


        private void FeedbackBox_Load(object sender, EventArgs e)
        {
            // The launch ping already fired from Form1. This one is specific
            // to opening this screen, so we can measure engagement separately
            // from raw app launches.
            TelemetryHelper.PingFeedbackOpened();
        }

        private void linkSurvey_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenUrl(SurveyUrl);
        }

        private void linkSupport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenUrl(SupportUrl);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private static void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch
            {
                // If the default browser can't be launched for any reason,
                // fail silently rather than showing an error to the user.
            }
        }
    }
}
