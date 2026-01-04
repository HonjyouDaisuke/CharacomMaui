using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Extensions;
using CharacomMaui.Presentation.Dialogs;
using CharacomMaui.Presentation.Interfaces;

namespace CharacomMaui.Presentation.Services;

public class ProgressDialogService : IProgressDialogService
{
  private ProgressDialog? _dialog;
  private bool _isShowing;

  public async Task ShowAsync(string title, string message)
  {
    var currentPage = Shell.Current?.CurrentPage;
    if (currentPage == null) return;
    if (_isShowing) return;

    _isShowing = true;

    await MainThread.InvokeOnMainThreadAsync(async () =>
    {
      _dialog = new ProgressDialog(title, message);
      currentPage.ShowPopup(_dialog);
    });
  }

  public async Task UpdateAsync(string message, double progress)
  {
    if (_dialog == null) return;

    await MainThread.InvokeOnMainThreadAsync(() =>
    {
      _dialog.Message = message;
      _dialog.AnimateProgress(progress);
    });
  }

  public async Task CloseAsync()
  {
    System.Diagnostics.Debug.WriteLine("[ProgressDialog]クローズ呼びました。");
    if (!_isShowing || _dialog == null)
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
      finally
      {
        _dialog = null;
        _isShowing = false; // ここでフラグを false に戻す
      }
    });
  }
}

