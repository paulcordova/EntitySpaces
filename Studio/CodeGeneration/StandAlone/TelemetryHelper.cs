using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EntitySpaces
{
    /// <summary>
    /// Anonymous usage ping. Sends only an install-scoped GUID, event type,
    /// and (server-side, via Cloudflare) coarse geolocation from the request IP.
    /// No personal data is collected. Failures are always silent and never
    /// affect the application.
    /// </summary>
    internal static class TelemetryHelper
    {
        private const string BaseUrl = "https://es-banner.paul-netstep.workers.dev";

        private static readonly HttpClient _client;

        static TelemetryHelper()
        {
            _client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(5)
            };

            // Clear application identifier for server logs
            _client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("EntitySpacesStudio", "1.0")
            );
        }

        /// <summary>
        /// Returns the persistent per-install id, generating and saving one
        /// on first run. Requires a user-scoped string setting named
        /// "InstallId" (Project Properties → Settings).
        /// </summary>
        public static string GetOrCreateInstallId()
        {
            var id = Properties.Settings.Default.InstallId;

            if (string.IsNullOrWhiteSpace(id))
            {
                id = Guid.NewGuid().ToString("N");
                Properties.Settings.Default.InstallId = id;
                Properties.Settings.Default.Save();
            }

            return id;
        }

        /// <summary>Call once when the application starts (Form1 constructor/Load).</summary>
        public static void PingLaunch()
        {
            _ = PingAsync("studio_legacy_launch");
        }

        /// <summary>Call when the user opens the Help → Feedback screen.</summary>
        public static void PingFeedbackOpened()
        {
            _ = PingAsync("studio_legacy_feedback");
        }

        private static async Task PingAsync(string source)
        {
            try
            {
                var cid = GetOrCreateInstallId();
                var url = $"{BaseUrl}?src={source}&cid={cid}";

                // ResponseHeadersRead stops downloading as soon as headers are received
                // (the banner image body is intentionally discarded).
                using (var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                {
                    // Response is intentionally discarded here —
                    // this call exists only to register the event server-side.
                }
            }
            catch
            {
                // No internet, blocked domain, timeout, etc. — never surface this
                // to the user, and never let it delay or break app startup.
            }
        }
    }
}