using MauiApp = Microsoft.Maui.Controls.Application;


namespace CharacomMaui.Presentation.Helpers;

public static class ThemeHelper
{
  public static Color GetColor(string key)
  {
    string LightOrDark = MauiApp.Current!.RequestedTheme == AppTheme.Light ? "" : "Dark";
    key = $"{key}{LightOrDark}";
    if (MauiApp.Current!.Resources.TryGetValue(key, out var value) && value is Color color)
      return color;
    return Colors.Transparent;
  }
}