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
                new { Title = "システム更新", Message = "新しいバージョンが利用可能です。" },
                new { Title = "メッセージ", Message = "田中さんから連絡があります。" },
                new { Title = "アラート", Message = "ストレージの空き容量が少なくなっています。" }
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