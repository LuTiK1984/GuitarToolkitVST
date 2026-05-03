using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace GuitarToolkit.UI;

public interface IThemeAware
{
    void ApplyTheme();
}

public enum ToolkitTheme
{
    Dark,
    Light
}

public static class ThemeManager
{
    private static readonly Color DarkBackground = Color.FromRgb(26, 21, 37);
    private static readonly Color DarkPanel = Color.FromRgb(36, 27, 51);
    private static readonly Color DarkPanelAlt = Color.FromRgb(45, 34, 64);
    private static readonly Color DarkBorder = Color.FromRgb(53, 40, 71);
    private static readonly Color DarkSurface = Color.FromRgb(24, 18, 34);
    private static readonly Color Purple = Color.FromRgb(74, 56, 96);
    private static readonly Color PurpleHover = Color.FromRgb(91, 70, 118);
    private static readonly Color Accent = Color.FromRgb(203, 166, 247);
    private static readonly Color Green = Color.FromRgb(166, 227, 161);
    private static readonly Color Yellow = Color.FromRgb(249, 226, 175);
    private static readonly Color Red = Color.FromRgb(243, 139, 168);
    private static readonly Color DarkText = Color.FromRgb(205, 214, 244);
    private static readonly Color DarkMuted = Color.FromRgb(155, 139, 184);

    private static readonly Color LightBackground = Color.FromRgb(247, 242, 250);
    private static readonly Color LightPanel = Color.FromRgb(239, 231, 246);
    private static readonly Color LightPanelAlt = Color.FromRgb(232, 222, 241);
    private static readonly Color LightBorder = Color.FromRgb(216, 201, 232);
    private static readonly Color LightSurface = Color.FromRgb(255, 251, 255);
    private static readonly Color LightText = Color.FromRgb(36, 27, 51);
    private static readonly Color LightMuted = Color.FromRgb(105, 82, 128);
    private static readonly Color LightControl = Color.FromRgb(255, 251, 255);
    private static readonly Color LightControlHover = Color.FromRgb(232, 222, 241);
    private static readonly Color LightGreen = Color.FromRgb(68, 151, 83);
    private static readonly Color LightYellow = Color.FromRgb(151, 102, 28);
    private static readonly Color LightRed = Color.FromRgb(190, 58, 96);

    private static readonly Dictionary<string, Color> DarkResources = new()
    {
        ["AppBackgroundBrush"] = DarkBackground,
        ["PanelBrush"] = DarkPanel,
        ["PanelBorderBrush"] = DarkBorder,
        ["SectionBrush"] = DarkSurface,
        ["ControlBrush"] = Purple,
        ["ControlHoverBrush"] = PurpleHover,
        ["AccentBrush"] = Accent,
        ["AccentGreenBrush"] = Green,
        ["StartBrush"] = Green,
        ["GoodBrush"] = Green,
        ["WarnBrush"] = Yellow,
        ["DangerBrush"] = Red,
        ["FavBrush"] = Yellow,
        ["TextBrush"] = DarkText,
        ["MutedTextBrush"] = DarkMuted,
        ["DarkBrush"] = DarkBackground,
        ["TabBrush"] = DarkPanelAlt,
        ["TabHoverBrush"] = Purple,
        ["TabSelectedBrush"] = Accent,
        ["TabSelectedTextBrush"] = DarkBackground,
        ["TitleBarBrush"] = Color.FromRgb(32, 25, 45),
        ["TitleBorderBrush"] = Color.FromRgb(91, 70, 118),
        ["TitleInnerGlowBrush"] = Color.FromArgb(24, 203, 166, 247),
        ["TitleButtonBrush"] = Color.FromRgb(35, 28, 49),
        ["TitleTextBrush"] = DarkText,
        ["TitleMutedBrush"] = DarkMuted,
        ["TitleHoverBrush"] = Color.FromRgb(203, 166, 247),
        ["TitleCloseHoverBrush"] = Red,
        ["TitleDarkBrush"] = DarkBackground,
    };

    private static readonly Dictionary<string, Color> LightResources = new()
    {
        ["AppBackgroundBrush"] = LightBackground,
        ["PanelBrush"] = LightPanel,
        ["PanelBorderBrush"] = LightBorder,
        ["SectionBrush"] = LightSurface,
        ["ControlBrush"] = LightControl,
        ["ControlHoverBrush"] = LightControlHover,
        ["AccentBrush"] = Accent,
        ["AccentGreenBrush"] = LightGreen,
        ["StartBrush"] = LightGreen,
        ["GoodBrush"] = LightGreen,
        ["WarnBrush"] = LightYellow,
        ["DangerBrush"] = LightRed,
        ["FavBrush"] = LightYellow,
        ["TextBrush"] = LightText,
        ["MutedTextBrush"] = LightMuted,
        ["DarkBrush"] = LightSurface,
        ["TabBrush"] = LightPanelAlt,
        ["TabHoverBrush"] = Color.FromRgb(224, 211, 237),
        ["TabSelectedBrush"] = Accent,
        ["TabSelectedTextBrush"] = DarkBackground,
        ["TitleBarBrush"] = Color.FromRgb(250, 246, 253),
        ["TitleBorderBrush"] = Color.FromRgb(190, 160, 224),
        ["TitleInnerGlowBrush"] = Color.FromArgb(120, 203, 166, 247),
        ["TitleButtonBrush"] = Color.FromRgb(255, 251, 255),
        ["TitleTextBrush"] = LightText,
        ["TitleMutedBrush"] = LightMuted,
        ["TitleHoverBrush"] = Color.FromRgb(236, 216, 252),
        ["TitleCloseHoverBrush"] = LightRed,
        ["TitleDarkBrush"] = LightSurface,
    };

    public static ToolkitTheme CurrentTheme { get; private set; } = ToolkitTheme.Dark;

    public static ToolkitTheme Parse(string? themeName)
    {
        return string.Equals(themeName, "Light", StringComparison.OrdinalIgnoreCase)
            ? ToolkitTheme.Light
            : ToolkitTheme.Dark;
    }

    public static string ToSettingsValue(ToolkitTheme theme) => theme == ToolkitTheme.Light ? "Light" : "Dark";

    public static void Apply(ToolkitTheme theme)
    {
        CurrentTheme = theme;
        ApplyApplicationResources(theme);
    }

    public static void Apply(DependencyObject root, ToolkitTheme theme)
    {
        CurrentTheme = theme;
        ApplyApplicationResources(theme);

        var visited = new HashSet<DependencyObject>();
        ApplyResources(root, theme, addMissingResources: true, visited);
    }

    public static Color GetColor(string resourceName)
    {
        var palette = CurrentTheme == ToolkitTheme.Light ? LightResources : DarkResources;
        return palette.TryGetValue(resourceName, out Color color) ? color : Colors.Transparent;
    }

    public static SolidColorBrush GetBrush(string resourceName)
    {
        return new SolidColorBrush(GetColor(resourceName));
    }

    private static void ApplyResources(DependencyObject root, ToolkitTheme theme, bool addMissingResources, HashSet<DependencyObject> visited)
    {
        if (!visited.Add(root)) return;

        if (root is FrameworkElement element)
        {
            ApplyResourceDictionary(element.Resources, theme, addMissingResources);

            if (element is IThemeAware themedElement)
                themedElement.ApplyTheme();

            foreach (object child in LogicalTreeHelper.GetChildren(element))
            {
                if (child is DependencyObject childObject)
                    ApplyResources(childObject, theme, addMissingResources: false, visited);
            }
        }

        if (root is not Visual and not Visual3D)
            return;

        int count = VisualTreeHelper.GetChildrenCount(root);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            ApplyResources(child, theme, addMissingResources: false, visited);
        }
    }

    private static void ApplyApplicationResources(ToolkitTheme theme)
    {
        if (Application.Current == null) return;
        ApplyResourceDictionary(Application.Current.Resources, theme, addMissingResources: true);
    }

    private static void ApplyResourceDictionary(ResourceDictionary resources, ToolkitTheme theme, bool addMissingResources)
    {
        var palette = theme == ToolkitTheme.Light ? LightResources : DarkResources;

        if (addMissingResources)
        {
            foreach (var (resourceName, resourceColor) in palette)
                resources[resourceName] = new SolidColorBrush(resourceColor);

            return;
        }

        foreach (object key in resources.Keys.Cast<object>().ToArray())
        {
            if (key is string resourceName && palette.TryGetValue(resourceName, out Color resourceColor))
                resources[resourceName] = new SolidColorBrush(resourceColor);
        }
    }
}
