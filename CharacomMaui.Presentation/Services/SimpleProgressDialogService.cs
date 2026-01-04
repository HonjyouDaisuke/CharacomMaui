using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Extensions;
using CharacomMaui.Presentation.Dialogs;
using CharacomMaui.Presentation.Interfaces;

namespace CharacomMaui.Presentation.Services;

public class SimpleProgressDialogService : ISimpleProgressDialogService
{
  private SimpleProgressDialog? _dialog;
  private bool _isShowing = false;

  public async Task ShowAsync(string title, string message)
  {
    var currentPage = Shell.Current?.CurrentPage;
    if (currentPage == null) return;
    if (_isShowing && _dialog != null) return;

    // 不整合な状態をクリーンアップ
    if (_dialog != null)
    {
      System.Diagnostics.Debug.WriteLine("[警告] 不整合な状態を検出: _dialogがnullではありません");
      await CloseAsync();
    }
    _isShowing = true;
    await MainThread.InvokeOnMainThreadAsync(() =>
    {
      _dialog = new SimpleProgressDialog(title, message);
      currentPage.ShowPopup(_dialog);
    });
  }

  public async Task CloseAsync()
  {
    if (_dialog == null)
    {
      _isShowing = false;
      return;
    }

    await MainThread.InvokeOnMainThreadAsync(async () =>
    {
      try
      {
        await _dialog.CloseAsync();
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"errorMsg = {ex.Message}");
      }
      finally
      {
        _isShowing = false;
        _dialog = null;
      }
    });
  }
}
