using CharacomMaui.Presentation.Models;

namespace CharacomMaui.Presentation.Services;

public static class SnackBarService
{
  public static Task Error(string message)
       => string.IsNullOrWhiteSpace(message)
         ? Task.CompletedTask
         : SnackBarHost.ShowAsync(message, SnackBarType.Error);

  public static Task Warning(string message)
       => string.IsNullOrWhiteSpace(message)
         ? Task.CompletedTask
         : SnackBarHost.ShowAsync(message, SnackBarType.Warning);

  public static Task Success(string message)
       => string.IsNullOrWhiteSpace(message)
         ? Task.CompletedTask
         : SnackBarHost.ShowAsync(message, SnackBarType.Success);

  public static Task Info(string message)
       => string.IsNullOrWhiteSpace(message)
         ? Task.CompletedTask
         : SnackBarHost.ShowAsync(message, SnackBarType.Info);
}