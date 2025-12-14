namespace CharacomMaui.Presentation.Components;

using System.Windows.Input;
using CharacomMaui.Presentation.Helpers;
using MauiApp = Microsoft.Maui.Controls.Application;
using CommunityToolkit.Maui.Views;
using CharacomMaui.Domain.Entities;

public partial class ProjectInfoCard : ContentView
{
  private const int DoubleClickTime = 300; // ダブルクリック判定の最大間隔(ms)
  private DateTime _lastTapTime;
  private CancellationTokenSource? _cts;

  // イベントを追加
  public event EventHandler<ProjectInfoEventArgs>? EditRequested;
  public event EventHandler<ProjectInfoEventArgs>? DeleteRequested;
  public event EventHandler<ProjectInfoEventArgs>? InviteRequested;

  public ProjectInfoCard()
  {
    InitializeComponent();
    System.Diagnostics.Debug.WriteLine($"ProjectFolderId={ProjectFolderId}, CharaFolderId={CharaFolderId}");
  }

  public static readonly BindableProperty ProjectIdProperty =
        BindableProperty.Create(nameof(ProjectId), typeof(string), typeof(ProjectInfoCard), string.Empty);
  public static readonly BindableProperty ProjectNameProperty =
      BindableProperty.Create(nameof(ProjectName), typeof(string), typeof(ProjectInfoCard), string.Empty);
  public static readonly BindableProperty ProjectDescriptionProperty =
  BindableProperty.Create(nameof(ProjectDescription), typeof(string), typeof(ProjectInfoCard), string.Empty);
  public static readonly BindableProperty ProjectFolderIdProperty =
  BindableProperty.Create(nameof(ProjectFolderId), typeof(string), typeof(ProjectInfoCard), string.Empty);

  public static readonly BindableProperty CharaFolderIdProperty =
    BindableProperty.Create(nameof(CharaFolderId), typeof(string), typeof(ProjectInfoCard), string.Empty);

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
  public string ProjectDescription { get => (string)GetValue(ProjectDescriptionProperty); set => SetValue(ProjectDescriptionProperty, value); }
  public int CharaCount { get => (int)GetValue(CharaCountProperty); set => SetValue(CharaCountProperty, value); }
  public int UserCount { get => (int)GetValue(UserCountProperty); set => SetValue(UserCountProperty, value); }
  public string ProjectFolderId { get => (string)GetValue(ProjectFolderIdProperty); set => SetValue(ProjectFolderIdProperty, value); }
  public string CharaFolderId { get => (string)GetValue(CharaFolderIdProperty); set => SetValue(CharaFolderIdProperty, value); }

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
        ProjectName = ProjectName,
        ProjectDescription = ProjectDescription,
        ProjectFolderId = ProjectFolderId,
        CharaFolderId = CharaFolderId,
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
          ProjectName = ProjectName,
          ProjectDescription = ProjectDescription,
          ProjectFolderId = ProjectFolderId,
          CharaFolderId = CharaFolderId,
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
    var primary = ThemeHelper.GetColor("Primary");
    var onPrimary = ThemeHelper.GetColor("OnPrimary");
    var surface = ThemeHelper.GetColor("Surface");
    var onSurface = ThemeHelper.GetColor("OnSurface");

    var primaryColor = (Color)MauiApp.Current!.Resources["Primary"]; // App.xaml の Primary
    var onColor = isSelected ? onPrimary : onSurface;

    BackgroundBorder.BackgroundColor = isSelected
        ? primary
        : surface;
    ProjectLabel.TextColor = onColor;
    BorderLine.BackgroundColor = onColor;
    ImagesLabel.TextColor = onColor;
    ImagesCountLabel.TextColor = onColor;
    UsersLabel.TextColor = onColor;
    UsersCountLabel.TextColor = onColor;
  }

  private void OnEditProject(object sender, EventArgs e)
  {
    // 編集処理
    System.Diagnostics.Debug.WriteLine($"Project {ProjectName} を編集します。 ID={ProjectId} ");
    System.Diagnostics.Debug.WriteLine($"ProjectFolder ID={ProjectFolderId}, CharaFolder ID={CharaFolderId}");

    EditRequested?.Invoke(this, new ProjectInfoEventArgs
    {
      ProjectId = ProjectId,
      ProjectName = ProjectName,
      ProjectDescription = ProjectDescription,
      ProjectFolderId = ProjectFolderId,
      CharaFolderId = CharaFolderId,
    });
  }

  private void OnDeleteProject(object sender, EventArgs e)
  {
    // 編集処理
    System.Diagnostics.Debug.WriteLine($"Project {ProjectName} を削除します。 ID={ProjectId} ");
    DeleteRequested?.Invoke(this, new ProjectInfoEventArgs
    {
      ProjectId = ProjectId,
      ProjectName = ProjectName
    });
  }

  private void OnInviteProject(object sender, EventArgs e)
  {
    // 編集処理
    System.Diagnostics.Debug.WriteLine($"Project {ProjectName} に招待します。 ID={ProjectId} ");
    InviteRequested?.Invoke(this, new ProjectInfoEventArgs
    {
      ProjectId = ProjectId,
      ProjectName = ProjectName
    });
  }

}
public class ProjectInfoEventArgs : EventArgs
{
  public string ProjectId { get; set; } = string.Empty;
  public string ProjectName { get; set; } = string.Empty;
  public string ProjectDescription { get; set; } = string.Empty;
  public string ProjectFolderId { get; set; } = string.Empty;
  public string CharaFolderId { get; set; } = string.Empty;
}