using Krypton.Toolkit;
using System;
using System.Drawing;

namespace EntitySpaces.Common
{
    public enum AppThemeMode
    {
        Standard,
        EntitySpaces,
        Dark
    }

    public static class AppTheme
    {
        // Add this event
        public static event Action ThemeChanged;

        // 1. Create a static instance of KryptonManager
        private static readonly KryptonManager _kryptonManager = new KryptonManager();
        public static AppThemeMode CurrentMode { get; set; } = AppThemeMode.EntitySpaces;


        // Returns the background color for controls and panels
        public static Color GetBackgroundColor()
        {
            switch (CurrentMode)
            {
                case AppThemeMode.Standard:
                    return Color.White;
                case AppThemeMode.EntitySpaces:
                    // CHANGE: Use FormMain instead of PanelClient to get the actual blue color
                    return KryptonManager.CurrentGlobalPalette.GetBackColor1(PaletteBackStyle.FormMain, PaletteState.Normal);
                // Or if you prefer a specific fixed light blue:
                // return Color.FromArgb(214, 232, 250);
                case AppThemeMode.Dark:
                    return Color.FromArgb(45, 45, 48);
                default:
                    return Color.White;
            }
        }

        // Returns the foreground color (text) for labels, etc.
        public static Color GetForegroundColor()
        {
            return CurrentMode == AppThemeMode.Dark ? Color.White : Color.Black;
        }

        // Applies the Krypton global theme
        public static void ApplyKryptonGlobalTheme()
        {
            switch (CurrentMode)
            {
                case AppThemeMode.Standard:
                    // 2. Use the instance to set the property
                    _kryptonManager.GlobalPaletteMode = PaletteMode.Office2010Blue;
                    break;
                case AppThemeMode.EntitySpaces:
                    _kryptonManager.GlobalPaletteMode = PaletteMode.Office2010Blue; // Adjust if you have a specific palette
                    break;
                case AppThemeMode.Dark:
                    // A built-in dark palette, e.g., Visual Studio 2022 Dark
                    _kryptonManager.GlobalPaletteMode = PaletteMode.Microsoft365Black;
                    break;
            }

            // Trigger the event so all subscribed controls can refresh
            ThemeChanged?.Invoke();
        }


    }
}