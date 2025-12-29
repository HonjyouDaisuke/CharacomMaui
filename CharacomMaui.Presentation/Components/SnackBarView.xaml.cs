using CharacomMaui.Presentation.Models;
using CharacomMaui.Presentation.Resources.Styles;
using CharacomMaui.Presentation.Services;

namespace CharacomMaui.Presentation.Components;

public partial class SnackBarView : ContentView
{
  public SnackBarView()
  {
    InitializeComponent();
    // 画面に現れたら、その時点の「有効なSnackBar」としてHostに自分を渡す
    this.Loaded += (s, e) => SnackBarHost.Initialize(this);
  }

  public async Task ShowAsync(string message, SnackBarType type, int durationMs = 3000)
  {
    var style = SnackBarStyles.Get(type);

    MainThread.BeginInvokeOnMainThread(() =>
    {
      Root.BackgroundColor = style.Background;
      Message.TextColor = style.TextColor;

      Icon.Text = style.Icon;
      Icon.TextColor = style.TextColor;

      Message.Text = message;
      this.IsVisible = true;
    });

    // 2. 表示状態にする
    this.IsVisible = true;
    this.Opacity = 0; // 一旦 0 にしてからフェードイン


    // 3. アニメーション
    await this.FadeTo(1, 200);
    await Task.Delay(durationMs);
    await this.FadeTo(0, 200);

    this.IsVisible = false;
  }
}
