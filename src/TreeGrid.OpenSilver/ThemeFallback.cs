using System;
using System.Windows;
using System.Windows.Media;

namespace TreeGrid.Wpf
{
    /// <summary>
    /// WPF resolves the TreeGrid.* brushes from this library's Generic.xaml when the app does not define
    /// them; OpenSilver only falls back to its own theme, which would leave the expander glyphs, grid lines
    /// and header and row backgrounds unpainted. The light brushes are merged in instead, and recolored from
    /// the palette of the Modern theme (Theme_* resources) when the app uses it, following its light and dark modes.
    /// </summary>
    internal static class ThemeFallback
    {
        private const string ProbeKey = "TreeGrid.Background";
        private const string PaletteProbeKey = "Theme_BackgroundBrush";
        private static ResourceDictionary _appTheme;

        // Each palette switch replaces the Theme_* brushes, so a reference to one of them reports every switch.
        private static readonly DependencyProperty PaletteBrushProperty =
            DependencyProperty.RegisterAttached("PaletteBrush", typeof(object), typeof(ThemeFallback),
                new PropertyMetadata(null, OnPaletteBrushChanged));

        private static readonly DependencyProperty DefaultThemeProperty =
            DependencyProperty.RegisterAttached("DefaultTheme", typeof(ResourceDictionary), typeof(ThemeFallback),
                new PropertyMetadata(null));

        /// <summary>
        /// Merges the light brushes first at app scope, so the app's own dictionaries (FluentDark, custom
        /// brushes) still take precedence, including ones merged later to switch themes.
        /// </summary>
        public static void EnsureAppBrushes()
        {
            if (_appTheme != null || Application.Current is not Application app)
                return;

            _appTheme = CreateLightTheme();
            app.Resources.MergedDictionaries.Insert(0, _appTheme);
        }

        /// <summary>
        /// Gives the grid its own default brushes when it cannot reach any, e.g. inside a host that skips the app
        /// resources (ResourceLookupMode="SkipAllNow") as visual designers do, or when it can reach a Modern palette.
        /// Brushes defined by the app or around the grid, such as FluentDark, keep precedence.
        /// </summary>
        public static void EnsureBrushes(TreeGridControl grid)
        {
            if (grid.GetValue(DefaultThemeProperty) != null)
                return;

            object brush = grid.TryFindResource(ProbeKey);
            bool isDefault = brush == null || (_appTheme != null && ReferenceEquals(brush, _appTheme[ProbeKey]));
            if (!isDefault || (brush != null && !TryReadPalette(grid, out _)))
                return;

            var theme = CreateLightTheme();
            grid.Resources.MergedDictionaries.Insert(0, theme);
            grid.SetValue(DefaultThemeProperty, theme);
            grid.SetResourceReference(PaletteBrushProperty, PaletteProbeKey);
        }

        private static void OnPaletteBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = (TreeGridControl)d;
            if (grid.GetValue(DefaultThemeProperty) is not ResourceDictionary theme)
                return;

            if (TryReadPalette(grid, out var palette))
            {
                ApplyPalette(theme, palette);
            }
            else
            {
                ApplyTheme(theme, CreateLightTheme());
            }

            grid.RefreshAppearance();
        }

        // Mirrors the Modern theme's DataGrid: rows on the control background, its alternate rows, borders and
        // text; the shades TreeGrid adds are blends of those colors, as the theme's BrushBuilder derives its own.
        private static void ApplyPalette(ResourceDictionary theme, Palette p)
        {
            SetColor(theme, "Background", p.Background);
            SetColor(theme, "EditorBackground", p.Background);
            SetColor(theme, "PopupBackground", p.Background);
            SetColor(theme, "GroupChipBackground", p.Background);
            SetColor(theme, "AlternatingRowBackground", p.AlternateRow);
            SetColor(theme, "HeaderBackground", Mix(p.Background, p.Text, 0.04));
            SetColor(theme, "FooterBackground", Mix(p.Background, p.Text, 0.04));
            SetColor(theme, "GroupAreaBackground", Mix(p.Background, p.Text, 0.06));
            SetColor(theme, "GroupRowBackground", Mix(p.Background, p.Primary, 0.08));
            SetColor(theme, "HeaderForeground", p.Text);
            SetColor(theme, "CellForeground", p.Text);
            SetColor(theme, "BorderBrush", p.Border);
            SetColor(theme, "GridLineBrush", Mix(p.Background, p.Border, 0.6));
            SetColor(theme, "SelectedRowBackground", Mix(p.Background, p.Primary, 0.22));
            SetColor(theme, "SortBadgeBackground", Mix(p.Background, p.Primary, 0.18));
            SetColor(theme, "CurrentCellBorder", p.Primary);
            SetColor(theme, "DropIndicator", p.Primary);
            SetColor(theme, "FilterGlyphActive", p.Primary);
            SetColor(theme, "ExpanderGlyph", Mix(p.Text, p.Background, 0.4));
            SetColor(theme, "FilterGlyph", Mix(p.Text, p.Background, 0.5));
            SetColor(theme, "GripperHover", Mix(p.Background, p.Text, 0.3));
            SetColor(theme, "FrozenLine", Mix(p.Background, p.Text, 0.4));
            SetColor(theme, "ErrorBrush", p.Error);
        }

        // The brushes are recolored rather than replaced, so the grid's resolved appearance keeps up with them.
        private static void ApplyTheme(ResourceDictionary theme, ResourceDictionary source)
        {
            foreach (object key in source.Keys)
            {
                if (source[key] is SolidColorBrush brush && theme[key] is SolidColorBrush target)
                {
                    target.Color = brush.Color;
                }
            }
        }

        private static void SetColor(ResourceDictionary theme, string name, Color color)
        {
            if (theme["TreeGrid." + name] is SolidColorBrush brush)
            {
                brush.Color = color;
            }
        }

        private static bool TryReadPalette(FrameworkElement grid, out Palette palette)
        {
            palette = default;
            if (!TryGetColor(grid, "ControlBackground", out palette.Background)
                || !TryGetColor(grid, "AlternateRow", out palette.AlternateRow)
                || !TryGetColor(grid, "Border", out palette.Border)
                || !TryGetColor(grid, "Text", out palette.Text)
                || !TryGetColor(grid, "Primary", out palette.Primary)
                || !TryGetColor(grid, "Error", out palette.Error))
                return false;

            return true;
        }

        private static bool TryGetColor(FrameworkElement grid, string name, out Color color)
        {
            object value = grid.TryFindResource("Theme_" + name + "Color");
            color = value is Color c ? c : default;
            return value is Color;
        }

        private static Color Mix(Color from, Color to, double amount) => Color.FromArgb(
            Lerp(from.A, to.A, amount), Lerp(from.R, to.R, amount), Lerp(from.G, to.G, amount), Lerp(from.B, to.B, amount));

        private static byte Lerp(byte from, byte to, double amount) => (byte)Math.Round(from + (to - from) * amount);

        // OpenSilver reads ResourceDictionary.Source at compile time only, so the compiled XAML is loaded instead.
        private static ResourceDictionary CreateLightTheme()
        {
            var theme = new ResourceDictionary();
            Application.LoadComponent(theme, new Uri("/TreeGrid.OpenSilver;component/Themes/FluentLight.xaml", UriKind.Relative));
            return theme;
        }

        private struct Palette
        {
            public Color Background;
            public Color AlternateRow;
            public Color Border;
            public Color Text;
            public Color Primary;
            public Color Error;
        }
    }
}
