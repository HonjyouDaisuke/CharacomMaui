using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Extensions;
using CharacomMaui.Presentation.Dialogs;
using CharacomMaui.Presentation.Interfaces;

namespace CharacomMaui.Presentation.Services;

public class ProgressDialogService : IProgressDialogService
{
  private ProgressDialog? _dialog;
  private bool _isShowing;

  public async Task<IProgressDialogSession> ShowAsync(string title, string message)
  {
    var currentPage = Shell.Current?.CurrentPage;
    if (currentPage == null)
      throw new InvalidOperationException("ページを取得できません");
    if (_isShowing && _dialog != null)
      throw new InvalidOperationException("ProgressDialogはすでに表示中です");

    _isShowing = true;

    await MainThread.InvokeOnMainThreadAsync(() =>
    {
      _dialog = new ProgressDialog(title, message);
      currentPage.ShowPopup(_dialog);
    });
    return new ProgressDialogSession(this);
  }

  internal async Task UpdateAsync(string message, double progress)
  {
    if (_dialog == null) return;

    await MainThread.InvokeOnMainThreadAsync(() =>
    {
      _dialog.Message = message;
      _dialog.AnimateProgress(progress);
    });
  }

  internal async Task CloseAsync()
  {
    System.Diagnostics.Debug.WriteLine("[ProgressDialog]クローズ呼びました。");
    if (_dialog == null) return;

    await MainThread.InvokeOnMainThreadAsync(async () =>
    {
      try
      {
        await _dialog.CloseAsync();
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"[ProgressDialog] Close error: {ex.Message}");
      }
      finally
      {
        _dialog = null;
        _isShowing = false;
      }
    });
  }
}

