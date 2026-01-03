using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Extensions;
using CharacomMaui.Presentation.Dialogs;
using CharacomMaui.Presentation.Interfaces;

namespace CharacomMaui.Presentation.Services;

public class SimpleProgressDialogService : ISimpleProgressDialogService
{
  private SimpleProgressDialog? _dialog;
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
      _dialog = new SimpleProgressDialog(title, message);
      _hostPage.ShowPopup(_dialog);
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
