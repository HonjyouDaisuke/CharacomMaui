using CharacomMaui.Presentation.Models;

namespace CharacomMaui.Presentation.Services;

public static class SnackBarService
{
  public static Task Error(string message, int durationMs = 3000)
       => string.IsNullOrWhiteSpace(message)
         ? Task.CompletedTask
         : SnackBarHost.ShowAsync(message, SnackBarType.Error, durationMs);

  public static Task Warning(string message, int durationMs = 3000)
       => string.IsNullOrWhiteSpace(message)
         ? Task.CompletedTask
         : SnackBarHost.ShowAsync(message, SnackBarType.Warning, durationMs);

  public static Task Success(string message, int durationMs = 3000)
       => string.IsNullOrWhiteSpace(message)
         ? Task.CompletedTask
         : SnackBarHost.ShowAsync(message, SnackBarType.Success, durationMs);

  public static Task Info(string message, int durationMs = 3000)
       => string.IsNullOrWhiteSpace(message)
         ? Task.CompletedTask
         : SnackBarHost.ShowAsync(message, SnackBarType.Info, durationMs);
}