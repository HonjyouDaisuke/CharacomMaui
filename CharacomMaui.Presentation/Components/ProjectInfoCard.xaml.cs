namespace CharacomMaui.Presentation.Components;

using MauiApp = Microsoft.Maui.Controls.Application;

public partial class ProjectInfoCard : ContentView
{
  private const int DoubleClickTime = 300; // ダブルクリック判定の最大間隔(ms)
  private DateTime _lastTapTime;
  private CancellationTokenSource? _cts;

  public ProjectInfoCard()
  {
    InitializeComponent();
  }

  public static readonly BindableProperty ProjectIdProperty =
        BindableProperty.Create(nameof(ProjectId), typeof(string), typeof(ProjectInfoCard), string.Empty);
  public static readonly BindableProperty ProjectNameProperty =
      BindableProperty.Create(nameof(ProjectName), typeof(string), typeof(ProjectInfoCard), string.Empty);
  public static readonly BindableProperty CharaCountProperty =
      BindableProperty.Create(nameof(CharaCount), typeof(int), typeof(ProjectInfoCard), 0);
  public static readonly BindableProperty UserCountProperty =
      BindableProperty.Create(nameof(UserCount), typeof(int), typeof(ProjectInfoCard), 0);

  public static readonly BindableProperty IsSelectedProperty =
      BindableProperty.Create(
          nameof(IsSelected),
          typeof(bool),
          typeof(ProjectInfoCard),
          false,
          propertyChanged: OnIsSelectedChanged);

  public bool IsSelected
  {
    get => (bool)GetValue(IsSelectedProperty);
    set => SetValue(IsSelectedProperty, value);
  }

  public string ProjectId { get => (string)GetValue(ProjectIdProperty); set => SetValue(ProjectIdProperty, value); }
  public string ProjectName { get => (string)GetValue(ProjectNameProperty); set => SetValue(ProjectNameProperty, value); }
  public int CharaCount { get => (int)GetValue(CharaCountProperty); set => SetValue(CharaCountProperty, value); }
  public int UserCount { get => (int)GetValue(UserCountProperty); set => SetValue(UserCountProperty, value); }

  // イベント
  public event EventHandler<ProjectInfoEventArgs>? CardClicked;
  public event EventHandler<ProjectInfoEventArgs>? CardDoubleClicked;

  // クリック時の処理
  private void OnCardTapped(object? sender, EventArgs e)
  {
    // すでに遅延処理がある場合 → ダブルクリック
    if (_cts != null)
    {
      _cts.Cancel();
      _cts.Dispose();
      _cts = null;

      CardDoubleClicked?.Invoke(this, new ProjectInfoEventArgs
      {
        ProjectId = ProjectId,
        ProjectName = ProjectName
      });
      return; // ここで早期リターン
    }

    // シングルクリックの遅延処理を準備
    _cts = new CancellationTokenSource();
    var token = _cts.Token;

    Task.Delay(DoubleClickTime, token).ContinueWith(t =>
    {
      if (t.IsCanceled)
        return;

      MainThread.BeginInvokeOnMainThread(() =>
      {
        CardClicked?.Invoke(this, new ProjectInfoEventArgs
        {
          ProjectId = ProjectId,
          ProjectName = ProjectName
        });
      });

      _cts?.Dispose();
      _cts = null;

    }, token);
  }

  //背景色の切り替え
  private static void OnIsSelectedChanged(BindableObject bindable, object oldValue, object newValue)
  {
    if (bindable is ProjectInfoCard card && newValue is bool isSelected)
    {
      card.UpdateBackground(isSelected);
    }
  }

  private void UpdateBackground(bool isSelected)
  {
    var primaryColor = (Color)MauiApp.Current!.Resources["Primary"]; // App.xaml の Primary
    var normalLight = Color.FromArgb("#FFFFFF");
    var normalDark = Color.FromArgb("#1E1E1E");

    BackgroundBorder.BackgroundColor = isSelected
        ? primaryColor
        : App.Current.RequestedTheme == AppTheme.Light ? normalLight : normalDark;
  }

}
public class ProjectInfoEventArgs : EventArgs
{
  public string ProjectId { get; set; } = string.Empty;
  public string ProjectName { get; set; } = string.Empty;
}