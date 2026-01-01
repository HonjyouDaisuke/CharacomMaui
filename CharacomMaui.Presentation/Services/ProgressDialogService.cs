using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Extensions;
using CharacomMaui.Presentation.Dialogs;

namespace CharacomMaui.Presentation.Services;

public class ProgressDialogService : IProgressDialogService
{
  private ProgressDialog? _dialog;
  private Page? _hostPage;

  // AppShell / Page 起動時に注入
  public void SetHost(Page page)
  {
    _hostPage = page;
  }

  public async Task ShowAsync(string title, string message)
  {
    if (_hostPage == null) return;
    if (_dialog != null)
    {
      System.Diagnostics.Debug.WriteLine("すでにダイアログが存在するため終了");
      await CloseAsync();
    }
    await MainThread.InvokeOnMainThreadAsync(async () =>
    {
      _dialog = new ProgressDialog(title, message);
      _hostPage.ShowPopup(_dialog);
    });
  }

  public async Task Update(string message, double progress)
  {
    if (_dialog == null) return;

    await MainThread.InvokeOnMainThreadAsync(async () =>
    {
      System.Diagnostics.Debug.WriteLine($"progress = {progress}");
      _dialog.Message = message;
      await _dialog.AnimateProgressAsync(progress);
    });
  }

  public async Task CloseAsync()
  {
    if (_dialog == null) return;

    await MainThread.InvokeOnMainThreadAsync(async () =>
    {
      await _dialog.CloseAsync();
      _dialog = null;
    });
  }
}
