using CharacomMaui.Presentation.Components;
using CharacomMaui.Presentation.Models;

namespace CharacomMaui.Presentation.Services;

public static class SnackBarHost
{
  private static SnackBarView? _snackBar;

  public static void Initialize(SnackBarView snackBar)
  {
    _snackBar = snackBar;
  }

  public static Task ShowAsync(
      string message,
      SnackBarType type,
      int durationMs = 3000)
  {
    if (_snackBar == null)
      return Task.CompletedTask;

    return MainThread.InvokeOnMainThreadAsync(() =>
        _snackBar.ShowAsync(message, type, durationMs));
  }
}
