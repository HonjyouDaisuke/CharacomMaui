using CharacomMaui.Presentation.Helpers;
using CharacomMaui.Presentation.Models;
using CharacomMaui.Presentation.Resources.Icons;

namespace CharacomMaui.Presentation.Resources.Styles;

public class SnackBarStyle
{
  public Color Background { get; init; }
  public Color TextColor { get; init; }
  public string Icon { get; init; }
}

public static class SnackBarStyles
{
  public static SnackBarStyle Get(SnackBarType type) => type switch
  {
    SnackBarType.Error => new()
    {
      Background = ThemeHelper.GetColor("Error"),
      TextColor = ThemeHelper.GetColor("OnError"),
      Icon = MaterialIcons.Error
    },
    SnackBarType.Warning => new()
    {
      Background = ThemeHelper.GetColor("Warning"),
      TextColor = ThemeHelper.GetColor("OnWarning"),
      Icon = MaterialIcons.Warning
    },
    SnackBarType.Success => new()
    {
      Background = ThemeHelper.GetColor("Success"),
      TextColor = ThemeHelper.GetColor("OnSuccess"),
      Icon = MaterialIcons.Success
    },
    _ => new()
    {
      Background = ThemeHelper.GetColor("Info"),
      TextColor = ThemeHelper.GetColor("OnInfo"),
      Icon = MaterialIcons.Info
    }
  };
}
