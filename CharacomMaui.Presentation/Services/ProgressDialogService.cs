using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Extensions;
using CharacomMaui.Presentation.Dialogs;
using CharacomMaui.Presentation.Interfaces;

namespace CharacomMaui.Presentation.Services;

public class ProgressDialogService : IProgressDialogService
{
  private ProgressDialog? _dialog;
  private Page? _hostPage;
  private bool _isShowing;

  public void SetHost(Page page)
  {
    _hostPage = page;
  }

  public async Task ShowAsync(string title, string message)
  {
    if (_hostPage == null) return;
    if (_isShowing) return;


    _isShowing = true;
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

