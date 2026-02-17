using System.Threading.Tasks;

namespace CharacomMaui.Presentation.Panels;

public partial class NotificationsPanel : ContentView
{
  public NotificationsPanel()
  {
    InitializeComponent();

    // テスト用のダミーデータ（本来はViewModelから渡します）
    BindingContext = new
    {
      Notifications = new[] {
                new { Id = "aaaa", Title = "システム更新", Message = "新しいバージョンが利用可能です。", TypeId = "admin_message"},
                new { Id = "bbbb", Title = "メッセージ", Message = "田中さんから連絡があります。", TypeId = "comment"},
                new { Id = "cccc", Title = "プロジェクト招待", Message = "プロジェクトに参加するよう招待されました。", TypeId = "project_invite" },
                new { Id = "dddd", Title = "プロジェクト更新", Message = "プロジェクトの状態が更新されました。", TypeId = "project_update" },
                new { Id = "eeee", Title = "リマインダー", Message = "定期的な作業を確認してください。", TypeId = "reminder" },
                new { Id = "ffff", Title = "システム通知", Message = "システムのメンテナンスが予定されています。", TypeId = "system" },
            }
    };
  }

  private async void OnCloseClicked(object sender, EventArgs e)
  {
    MessagingCenter.Send<object>(this, "CloseNotifications");
    // await this.TranslateTo(300, 0, 250, Easing.CubicIn); // 右にスライドして消えるアニメーション
    // // このコンポーネント自体を非表示にする
    // this.IsVisible = false;
    // this.TranslationX = 0; // 次回表示のために位置をリセット
  }
}