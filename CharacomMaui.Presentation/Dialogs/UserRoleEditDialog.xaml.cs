using CommunityToolkit.Maui.Views;
using CharacomMaui.Domain.Entities;
using UraniumUI.Dialogs;
using System.ComponentModel;
using System.Diagnostics;
using CharacomMaui.Presentation.ViewModels;
using CharacomMaui.Presentation.Services;
using System.Threading.Tasks;
using Mopups.Pages;
using Mopups.Services;
using CharacomMaui.Presentation.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Application.Interfaces;

namespace CharacomMaui.Presentation.Dialogs;

public partial class UserRoleEditDialog : Popup
{

  private readonly IDialogService _dialogService;
  private readonly AppStatusNotifier _notifier;
  private readonly AppStatus _appStatus;
  private readonly GetAvatarsUrlUseCase _avatarsUrlUseCase;
  private readonly UpdateUserInfoUseCase _userInfoUseCase;
  private readonly IAppTokenStorageService _tokenStorage;

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
        typeof(UserRoleEditDialog),
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
        typeof(UserRoleEditDialog),
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
        typeof(UserRoleEditDialog),
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
        typeof(UserRoleEditDialog),
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
        typeof(UserRoleEditDialog),
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
        typeof(UserRoleEditDialog),
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
        typeof(UserRoleEditDialog),
        true);
  public bool IsAvatarSelectMode
  {
    get => (bool)GetValue(IsAvatarSelectModeProperty);
    set => SetValue(IsAvatarSelectModeProperty, value);
  }

  public UserRoleEditDialog(string title, UserInfoSummary userInfo,
                           IDialogService dialogService)
  {
    InitializeComponent();

    BindingContext = this;
    Title = title;
    AppUserName = userInfo.Name;
    AppEmailAddress = userInfo.Email;
    AvatarUrl = userInfo.AvatarUrl;
    UserId = userInfo.Id;
    UserRole = userInfo.RoleId;
    _dialogService = dialogService;
  }


  private async void OnOkClicked(object sender, EventArgs e)
  {
    await CloseAsync();
    await SnackBarService.Success("ユーザーロールが更新されました。");
  }

  private async void OnCancelClicked(object sender, EventArgs e)
  {
    await CloseAsync();
    await SnackBarService.Warning("キャンセルされました。");
  }
}