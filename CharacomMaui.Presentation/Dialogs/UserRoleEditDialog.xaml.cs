using CommunityToolkit.Maui.Views;
using CharacomMaui.Domain.Entities;
using UraniumUI.Dialogs;
using System.ComponentModel;
using System.Diagnostics;
using CharacomMaui.Presentation.ViewModels;
using CharacomMaui.Presentation.Services;
using CharacomMaui.Presentation.Components;
using System.Threading.Tasks;
using Mopups.Pages;
using Mopups.Services;
using CharacomMaui.Presentation.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.Sessions;
using System.Dynamic;

namespace CharacomMaui.Presentation.Dialogs;

public partial class UserRoleEditDialog : Popup<UserInfoRowEventArgs>
{

  private readonly IDialogService _dialogService;
  // private readonly AppStatusNotifier _notifier;
  // private readonly AppStatus _appStatus;
  // private readonly GetAvatarsUrlUseCase _avatarsUrlUseCase;
  // private readonly UpdateUserInfoUseCase _userInfoUseCase;
  // private readonly IAppTokenStorageService _tokenStorage;

  public ObservableCollection<string> Avatars { get; } = new();
  // ========== Title ==========
  public static readonly BindableProperty TitleProperty =
      BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(UserRoleEditDialog),
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
  // ========== SelectedRoleId ==========
  public static readonly BindableProperty SelectedRoleIdProperty =
    BindableProperty.Create(
      nameof(SelectedRoleId),
      typeof(string),
      typeof(UserRoleEditDialog),
      string.Empty);

  public string SelectedRoleId
  {
    get => (string)GetValue(SelectedRoleIdProperty);
    set => SetValue(SelectedRoleIdProperty, value);
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

  private readonly UserRolesSession _userRolesSession;
  public ObservableCollection<string> UserRoles { get; private set; } = new() { "管理者", "デビルマン", "スーパーマン" };

  private string? _selectedRole;
  public string? SelectedRole
  {
    get => _selectedRole;
    set
    {
      _selectedRole = value;
      SelectedRoleId = _userRolesSession.GetRoleIdFromRoleName(value ?? string.Empty);
      OnPropertyChanged();
    }
  }

  public bool IsCanceled { get; private set; } = true;
  public UserRoleEditDialog(string title, UserInfoSummary userInfo, IDialogService dialogService, UserRolesSession userRolesSession)
  {

    _userRolesSession = userRolesSession;
    UserRolesInit(userInfo.RoleId);                  // ← ③ 最後に Roles & SelectedRole

    InitializeComponent();        // ← ① 先に UI を作る
    BindingContext = this;

    Title = title;
    AppUserName = userInfo.Name;
    AppEmailAddress = userInfo.Email;
    AvatarUrl = userInfo.AvatarUrl;
    UserId = userInfo.Id;

    //SelectedRoleId = userInfo.RoleId; // ← ② Id を先に入れる
    System.Diagnostics.Debug.WriteLine($"UserRoleId = {userInfo.RoleId} Name = {_userRolesSession.GetRoleNameFromRoleId(userInfo.RoleId)}");
    SelectedRole = _userRolesSession.GetRoleNameFromRoleId(userInfo.RoleId);

    _dialogService = dialogService;
  }

  private void UserRolesInit(string initialRoleId)
  {

    var roles = _userRolesSession.Roles;
    var list = roles.Select(r => r.Name).ToList();
    UserRoles = new ObservableCollection<string>(list);
  }

  private async void OnOkClicked(object sender, EventArgs e)
  {
    IsCanceled = false;

    var result = new UserInfoRowEventArgs
    {
      UserId = this.UserId,
      UserName = this.AppUserName,
      UserRole = SelectedRoleId
    };
    System.Diagnostics.Debug.WriteLine($"OnOkClicked: {result.UserName} ({result.UserId}) Role: {result.UserRole}");
    // await SnackBarService.Success("ユーザーロールが更新されました。");
    await CloseAsync(result);

  }

  private async void OnCancelClicked(object sender, EventArgs e)
  {
    IsCanceled = true;
    System.Diagnostics.Debug.WriteLine("OnCancelClicked");
    // await SnackBarService.Warning("キャンセルされました。");
    await CloseAsync(null);

  }
}