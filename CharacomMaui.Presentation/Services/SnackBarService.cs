namespace CharacomMaui.Presentation.Services;
public static class SnackBarService
{
    public static Task Error(string message)
        => SnackBarHost.ShowAsync(message, SnackBarType.Error);

    public static Task Warning(string message)
        => SnackBarHost.ShowAsync(message, SnackBarType.Warning);

    public static Task Success(string message)
        => SnackBarHost.ShowAsync(message, SnackBarType.Success);

    public static Task Info(string message)
        => SnackBarHost.ShowAsync(message, SnackBarType.Info);
}