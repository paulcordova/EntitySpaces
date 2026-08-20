using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace EntitySpaces
{
    /// <summary>
    /// Feedback &amp; Support screen, opened from Help → Feedback.
    /// Follows the same usage pattern as AboutBox (new instance, ShowDialog, dispose).
    /// </summary>
    public partial class FeedbackBox : Form
    {
        private const string SurveyUrl = "https://docs.google.com/forms/d/e/1FAIpQLSd-FVQiC3deoaIarYnsOCH4pdj-4zjGKznN68uUtyx9CpuKgA/viewform?usp=header";
        private const string SupportUrl = "https://netstep.cl/entityspaces/support/";

        public FeedbackBox()
        {
            InitializeComponent();
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
