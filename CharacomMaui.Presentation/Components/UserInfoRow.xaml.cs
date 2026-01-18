namespace CharacomMaui.Presentation.Components;

using CharacomMaui.Domain.Entities;
using CharacomMaui.Presentation.Helpers;
using MauiApp = Microsoft.Maui.Controls.Application;
using CharacomMaui.Presentation.Models;

public partial class UserInfoRow : ContentView
{
  private const int DoubleClickTime = 300; // ダブルクリック判定の最大間隔(ms)
  private DateTime _lastTapTime;
  private CancellationTokenSource? _cts;


  public string UserId { get => (string)GetValue(UserIdProperty); set => SetValue(UserIdProperty, value); }
  public string UserName { get => (string)GetValue(UserNameProperty); set => SetValue(UserNameProperty, value); }
  public string UserEmail { get => (string)GetValue(UserEmailProperty); set => SetValue(UserEmailProperty, value); }
  public string UserAvatarUrl { get => (string)GetValue(UserAvatarUrlProperty); set => SetValue(UserAvatarUrlProperty, value); }
  public string UserRole { get => (string)GetValue(UserRoleProperty); set => SetValue(UserRoleProperty, value); }
  public string UserRoleName { get => (string)GetValue(UserRoleNameProperty); set => SetValue(UserRoleNameProperty, value); }

  public static readonly BindableProperty UserIdProperty =
    BindableProperty.Create(nameof(UserId), typeof(string), typeof(UserInfoRow), string.Empty);
  public static readonly BindableProperty UserNameProperty =
    BindableProperty.Create(nameof(UserName), typeof(string), typeof(UserInfoRow), string.Empty);
  public static readonly BindableProperty UserEmailProperty =
    BindableProperty.Create(nameof(UserEmail), typeof(string), typeof(UserInfoRow), string.Empty);
  public static readonly BindableProperty UserAvatarUrlProperty =
    BindableProperty.Create(nameof(UserAvatarUrl), typeof(string), typeof(UserInfoRow), string.Empty);
  public static readonly BindableProperty UserRoleProperty =
    BindableProperty.Create(nameof(UserRole), typeof(string), typeof(UserInfoRow), string.Empty);
  public static readonly BindableProperty UserRoleNameProperty =
    BindableProperty.Create(nameof(UserRoleName), typeof(string), typeof(UserInfoRow), string.Empty);


  public bool IsSelected
  {
    get => (bool)GetValue(IsSelectedProperty);
    set => SetValue(IsSelectedProperty, value);
  }

  public static readonly BindableProperty IsSelectedProperty =
     BindableProperty.Create(
         nameof(IsSelected),
         typeof(bool),
         typeof(UserInfoRow),
         false,
         propertyChanged: OnIsSelectedChanged);

  public static readonly BindableProperty IsOddProperty =
    BindableProperty.Create(
      nameof(IsOdd),
      typeof(bool),
      typeof(UserInfoRow),
      false,
      propertyChanged: OnIsOddChanged);


  public bool IsOdd
  {
    get => (bool)GetValue(IsOddProperty);
    set => SetValue(IsOddProperty, value);
  }

  // イベント
  public event EventHandler<UserInfoRowEventArgs>? RowClicked;
  public event EventHandler<UserInfoRowEventArgs>? RowDoubleClicked;
  public UserInfoRow()
  {
    InitializeComponent();
    //this.Content.BindingContext = this;
  }

  // クリック時の処理
  private void OnCardTapped(object? sender, EventArgs e)
  {
    // すでに遅延処理がある場合 → ダブルクリック
    if (_cts != null)
    {
      _cts.Cancel();
      _cts.Dispose();
      _cts = null;
      RowDoubleClicked?.Invoke(this, new UserInfoRowEventArgs
      {
        UserId = UserId,
        UserName = UserName,
        UserRole = UserRole
      });
      System.Diagnostics.Debug.WriteLine("ダブルクリック検出");

      return; // ここで早期リターン
    }

    // シングルクリックの遅延処理を準備
    _cts = new CancellationTokenSource();
    var token = _cts.Token;

    Task.Delay(DoubleClickTime, token).ContinueWith(t =>
    {
      if (t.IsCanceled)
      {
        return;
      }

      MainThread.BeginInvokeOnMainThread(() =>
      {
        RowClicked?.Invoke(this, new UserInfoRowEventArgs
        {
          UserId = UserId,
          UserName = UserName,
          UserRole = UserRole
        });
      });

      _cts?.Dispose();
      _cts = null;

    }, token);
  }

  //背景色の切り替え
  private static void OnIsSelectedChanged(BindableObject bindable, object oldValue, object newValue)
  {
    if (bindable is UserInfoRow row)
      row.UpdateVisualState();
  }
  private static void OnIsOddChanged(BindableObject bindable, object oldValue, object newValue)
  {
    if (bindable is UserInfoRow row)
      row.UpdateVisualState();
  }
  protected override void OnPropertyChanged(string? propertyName = null)
  {
    base.OnPropertyChanged(propertyName);

    if (propertyName == IsEnabledProperty.PropertyName)
    {
      if (IsEnabled)
      {
        // 🔥 Disabled から必ず引き戻す
        UpdateVisualState();
      }
    }
  }
  private void OnBackgroundLoaded(object? sender, EventArgs e)
  {
    UpdateVisualState();
  }

  protected override void OnBindingContextChanged()
  {
    base.OnBindingContextChanged();
    UpdateVisualState();
  }

  private void UpdateVisualState()
  {
    if (IsSelected)
    {
      VisualStateManager.GoToState(BackgroundBorder, "Selected");
    }
    else
    {
      VisualStateManager.GoToState(
        BackgroundBorder,
        IsOdd ? "NormalOdd" : "NormalEven");
    }

  }
}
public class UserInfoRowEventArgs : EventArgs
{
  public string UserName { get; set; } = string.Empty;
  public string UserId { get; set; } = string.Empty;
  public string UserRole { get; set; } = string.Empty;
}