using CommunityToolkit.Maui.Views;

namespace CharacomMaui.Presentation.Dialogs;

public partial class DownloadingDialog : Popup
{
  public event Action? CancelRequested;

  public DownloadingDialog(CancellationTokenSource? cts = null)
  {
    InitializeComponent();
  }

  private void OnCloseClicked(object sender, EventArgs e)
  {
    CancelRequested?.Invoke(); //ダイアログを閉じる(キャンセル通知)
  }

  public void UpdateProgress(double progress)
  {
    ProgressBar.Progress = progress; // 0.0 ~ 1.0
  }
}
