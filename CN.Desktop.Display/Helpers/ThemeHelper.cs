using MaterialDesignColors;

using MaterialDesignThemes.Wpf;

namespace CN.Desktop.Display.Helpers;
internal static class ThemeHelper
{
    internal static void SetTheme()
    {
        PaletteHelper paletteHelper = new();
        ITheme theme = paletteHelper.GetTheme();

        theme.SetBaseTheme(Theme.Dark);
        theme.SetPrimaryColor(SwatchHelper.Lookup[MaterialDesignColor.Yellow500]);
        theme.SetSecondaryColor(SwatchHelper.Lookup[MaterialDesignColor.Teal400]);
        paletteHelper.SetTheme(theme);
    }
}
