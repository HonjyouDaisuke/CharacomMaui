using CommunityToolkit.Maui.Views;
using CharacomMaui.Domain.Entities;
using UraniumUI.Dialogs;
using System.ComponentModel;
using System.Diagnostics;
using CharacomMaui.Presentation.ViewModels;
using System.Threading.Tasks;
using Mopups.Pages;
using Mopups.Services;
using CharacomMaui.Presentation.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CharacomMaui.Application.UseCases;

namespace CharacomMaui.Presentation.Dialogs;

public partial class UserProfileDialog : Popup
{

  private readonly IDialogService _dialogService;
  private readonly AppStatusNotifier _notifier;
  private readonly AppStatus _appStatus;
  private readonly GetAvatarsUrlUseCase _avatarsUrlUseCase;
  private readonly UpdateUserInfoUseCase _userInfoUseCase;

  public ObservableCollection<string> Avatars { get; } = new();
  // ========== Title ==========
  public static readonly BindableProperty TitleProperty =
      BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(UserProfileDialog),
        string.Empty);
  public string Title
  {
    get => (string)GetValue(TitleProperty);
    set => SetValue(TitleProperty, value);
  }
  // ========== UserName ==========
  public static readonly BindableProperty AppUserNameProperty =
      BindableProperty.Create(
        nameof(AppUserName),
        typeof(string),
        typeof(UserProfileDialog),
        string.Empty);
  public string AppUserName
  {
    get => (string)GetValue(AppUserNameProperty);
    set => SetValue(AppUserNameProperty, value);
  }
  // ========== email ==========
  public static readonly BindableProperty AppEmailAddressProperty =
      BindableProperty.Create(
        nameof(AppEmailAddress),
        typeof(string),
        typeof(UserProfileDialog),
        string.Empty);
  public string AppEmailAddress
  {
    get => (string)GetValue(AppEmailAddressProperty);
    set => SetValue(AppEmailAddressProperty, value);
  }
  // ========== Avatar Url ==========
  public static readonly BindableProperty AvatarUrlProperty =
      BindableProperty.Create(
        nameof(AvatarUrl),
        typeof(string),
        typeof(UserProfileDialog),
        string.Empty);
  public string AvatarUrl
  {
    get => (string)GetValue(AvatarUrlProperty);
    set => SetValue(AvatarUrlProperty, value);
  }
  // ========== UserId ==========
  public static readonly BindableProperty UserIdProperty =
      BindableProperty.Create(
        nameof(UserId),
        typeof(string),
        typeof(UserProfileDialog),
        string.Empty);
  public string UserId
  {
    get => (string)GetValue(UserIdProperty);
    set => SetValue(UserIdProperty, value);
  }
  // ========== UserRole ==========
  public static readonly BindableProperty UserRoleProperty =
      BindableProperty.Create(
        nameof(UserRole),
        typeof(string),
        typeof(UserProfileDialog),
        string.Empty);
  public string UserRole
  {
    get => (string)GetValue(UserRoleProperty);
    set => SetValue(UserRoleProperty, value);
  }
  // ========== IsEditMode ==========
  public static readonly BindableProperty IsEditModeProperty =
      BindableProperty.Create(
        nameof(IsEditMode),
        typeof(bool),
        typeof(UserProfileDialog),
        true);
  public bool IsEditMode
  {
    get => (bool)GetValue(IsEditModeProperty);
    set => SetValue(IsEditModeProperty, value);
  }

  // ========== IsEditMode ==========
  public static readonly BindableProperty IsAvatarSelectModeProperty =
      BindableProperty.Create(
        nameof(IsAvatarSelectMode),
        typeof(bool),
        typeof(UserProfileDialog),
        true);
  public bool IsAvatarSelectMode
  {
    get => (bool)GetValue(IsAvatarSelectModeProperty);
    set => SetValue(IsAvatarSelectModeProperty, value);
  }

  public UserProfileDialog(string title, IDialogService dialogService, AppStatusNotifier notifier, AppStatus appStatus, GetAvatarsUrlUseCase avatarsUrlUseCase, UpdateUserInfoUseCase userInfoUseCase)
  {
    InitializeComponent();

    BindingContext = this;
    Title = title;
    _dialogService = dialogService;
    _notifier = notifier;
    _appStatus = appStatus;
    _avatarsUrlUseCase = avatarsUrlUseCase;
    _userInfoUseCase = userInfoUseCase;
    AvatarUrl = _notifier.AvatarUrl ?? string.Empty;
    AppUserName = _notifier.UserName ?? string.Empty;
    AppEmailAddress = _notifier.UserEmail ?? string.Empty;
    UserRole = _appStatus.UserRole;
    UserId = _appStatus.UserId;
    IsAvatarSelectMode = false;
    IsEditMode = true;
    AvatarsLoad();
  }

  private async void AvatarsLoad()
  {
    var access_token = Preferences.Get("app_access_token", string.Empty);
    var avatars = await _avatarsUrlUseCase.ExecuteAsync(access_token);
    Avatars.Clear();
    foreach (var avatar in avatars)
    {
      System.Diagnostics.Debug.WriteLine($"avatarURL = {avatar}");
      Avatars.Add(avatar);
    }

  }

  private async void OnOkClicked(object sender, EventArgs e)
  {
    // TODO:APIを叩いて、DBに保存してもらう
    _notifier.UserName = AppUserName;
    _notifier.UserEmail = AppEmailAddress;
    _notifier.AvatarUrl = AvatarUrl;
    var access_token = Preferences.Get("app_access_token", string.Empty);
    var res = await _userInfoUseCase.ExecuteAsync(access_token, _appStatus.UserId, AppUserName, AppEmailAddress, AvatarUrl);
    if (!res.Success)
    {
      System.Diagnostics.Debug.WriteLine($"エラーが発生しました。{res.Message}");
    }
    await CloseAsync();
  }

  private async void OnCancelClicked(object sender, EventArgs e)
  {
    await CloseAsync();
  }

  private void OnAvaterSelectClicked(object sender, EventArgs e)
  {
    System.Diagnostics.Debug.WriteLine("AvatarSelectClicked");
    IsEditMode = false;
    IsAvatarSelectMode = true;
  }
  private void OnAvatarCancel(object sender, EventArgs e)
  {
    Debug.WriteLine("AvatarCancel");
    IsEditMode = true;
    IsAvatarSelectMode = false;
  }
  private void OnAvatarTapped(object sender, TappedEventArgs e)
  {
    if (sender is not View avatarView)
      return;
    var avatarUrl = e.Parameter as string;
    if (string.IsNullOrEmpty(avatarUrl))
      return;

    AvatarUrl = avatarUrl;
    IsEditMode = true;
    IsAvatarSelectMode = false;

    // TODO:APIを叩いて、DBに保存してもらう
  }
}