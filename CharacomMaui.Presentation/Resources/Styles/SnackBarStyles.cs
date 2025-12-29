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
            Background = Colors.DarkRed,
            TextColor = Colors.White,
            Icon = FontAwesomeIcons.Error
        },
        SnackBarType.Warning => new()
        {
            Background = Colors.Orange,
            TextColor = Colors.Black,
            Icon = FontAwesomeIcons.Warning
        },
        SnackBarType.Success => new()
        {
            Background = Colors.Green,
            TextColor = Colors.White,
            Icon = FontAwesomeIcons.Success
        },
        _ => new()
        {
            Background = Colors.DodgerBlue,
            TextColor = Colors.White,
            Icon = FontAwesomeIcons.Info
        }
    };
}
