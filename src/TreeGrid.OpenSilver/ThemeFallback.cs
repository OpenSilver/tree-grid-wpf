using System;
using System.Windows;

namespace TreeGrid.Wpf
{
    /// <summary>
    /// WPF resolves the TreeGrid.* brushes from this library's Generic.xaml when the app does not define
    /// them; OpenSilver only falls back to its own theme, which would leave the expander glyphs, grid lines
    /// and header and row backgrounds unpainted. The light brushes are merged in instead.
    /// </summary>
    internal static class ThemeFallback
    {
        private const string ProbeKey = "TreeGrid.Background";
        private static bool _appliedToApp;

        /// <summary>
        /// Merges the light brushes first at app scope, so the app's own dictionaries (FluentDark, custom
        /// brushes) still take precedence, including ones merged later to switch themes.
        /// </summary>
        public static void EnsureAppBrushes()
        {
            if (_appliedToApp || Application.Current is not Application app)
                return;

            _appliedToApp = true;
            app.Resources.MergedDictionaries.Insert(0, CreateLightTheme());
        }

        /// <summary>
        /// Merges the light brushes into the grid when it cannot reach them, e.g. inside a host that skips the
        /// app resources (ResourceLookupMode="SkipAllNow"), as visual designers do.
        /// </summary>
        public static void EnsureBrushes(FrameworkElement grid)
        {
            if (grid.TryFindResource(ProbeKey) != null)
                return;

            grid.Resources.MergedDictionaries.Insert(0, CreateLightTheme());
        }

        // OpenSilver reads ResourceDictionary.Source at compile time only, so the compiled XAML is loaded instead.
        private static ResourceDictionary CreateLightTheme()
        {
            var theme = new ResourceDictionary();
            Application.LoadComponent(theme, new Uri("/TreeGrid.OpenSilver;component/Themes/FluentLight.xaml", UriKind.Relative));
            return theme;
        }
    }
}
