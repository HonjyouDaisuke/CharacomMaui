using CharacomMaui.Presentation.Resources.Styles;
using CharacomMaui.Presentation.Services;

namespace CharacomMaui.Presentation.Components;

public partial class SnackBarView : ContentView
{
  public SnackBarView()
  {
    InitializeComponent();
    this.Loaded += (s, e) =>
    {
      System.Diagnostics.Debug.WriteLine("★★★ SnackBarView Loaded!");
      SnackBarHost.Initialize(this);
    };

    this.Unloaded += (s, e) =>
    {
      System.Diagnostics.Debug.WriteLine("★★★ SnackBarView Unloaded!");
    };
  }

  public async Task ShowAsync(
      string message,
      SnackBarType type,
      int durationMs = 3000)
  {
    var style = SnackBarStyles.Get(type);

    Root.BackgroundColor = style.Background;
    Message.TextColor = style.TextColor;
    Icon.TextColor = style.TextColor;

    Icon.Text = style.Icon;
    Message.Text = message;

    IsVisible = true;
    System.Diagnostics.Debug.WriteLine(message);
    await this.FadeTo(1, 200);
    await Task.Delay(durationMs);
    await this.FadeTo(0, 200);

    IsVisible = false;
  }
}
