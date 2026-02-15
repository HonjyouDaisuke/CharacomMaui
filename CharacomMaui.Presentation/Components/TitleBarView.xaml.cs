namespace CharacomMaui.Presentation.Components;

using CharacomMaui.Presentation.Dialogs;
using CharacomMaui.Presentation.ViewModels;
using UraniumUI.Dialogs;
using CommunityToolkit.Maui.Views;
using MauiApp = Microsoft.Maui.Controls;
using Mopups.Services;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.Messaging;
using CharacomMaui.Presentation.Models;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.Sessions;

public partial class TitleBarView : ContentView
{
  private IDialogService? _dialogService;
  private AppStatusNotifier? _notifier;
  private AppStatus? _appStatus;
  private GetAvatarsUrlUseCase? _getAvatarsUrlUseCase;
  private UpdateUserInfoUseCase? _userInfoUseCase;
  private IAppTokenStorageService? _tokenStorage;
  private UserRolesSession _userRolesSession;
  private IGetUserInfoUseCase? _getUserInfoUseCase;
  private TitleBarViewModel _viewModel;
  public TitleBarView()
  {
    InitializeComponent();
    _viewModel = Handler?.MauiContext?.Services.GetService<TitleBarViewModel>()
                  ?? IPlatformApplication.Current.Services.GetService<TitleBarViewModel>();

    this.BindingContext = _viewModel;
    this.Loaded += OnLoaded;

  }
  private void OnLoaded(object? sender, EventArgs e)
  {
    var services = Handler?.MauiContext?.Services;
    if (services == null)
    {
      System.Diagnostics.Debug.WriteLine("MauiContext が取得できません");
      return;
    }

    _dialogService = services.GetService<IDialogService>();
    _notifier = services.GetService<AppStatusNotifier>();
    _appStatus = services.GetService<AppStatus>();
    _getAvatarsUrlUseCase = services.GetService<GetAvatarsUrlUseCase>();
    _userInfoUseCase = services.GetService<UpdateUserInfoUseCase>();
    _tokenStorage = services.GetService<IAppTokenStorageService>();
    _userRolesSession = services.GetService<UserRolesSession>();
    _getUserInfoUseCase = services.GetService<IGetUserInfoUseCase>();
  }
  private bool isNullInstance()
  {
    return _dialogService == null ||
        _notifier == null ||
        _appStatus == null ||
        _getAvatarsUrlUseCase == null ||
        _userInfoUseCase == null ||
        _tokenStorage == null ||
        _viewModel == null ||
        _userRolesSession == null;
  }
  private async void OnAvatarViewTapped(object sender, EventArgs e)
  {
    if (isNullInstance()) return;

    var dialog = new UserProfileDialog("ユーザー情報の更新", _dialogService, _notifier, _appStatus, _getAvatarsUrlUseCase, _userInfoUseCase, _tokenStorage, _userRolesSession);

    await Shell.Current.ShowPopupAsync(dialog);
  }

  private async void OnProxyUserTapped(object sender, EventArgs e)
  {
    if (isNullInstance()) return;
    if (_tokenStorage == null) return;
    if (_getUserInfoUseCase == null) return;
    try
    {
      var tokens = await _tokenStorage.GetTokensAsync();
      var accessToken = tokens?.AccessToken;
      if (accessToken == null) return;

      if (_notifier!.IsProxy)
      {
        await _viewModel.ProxyLogoutAsync();
        return;
      }
      var users = await _getUserInfoUseCase.GetUserListAsync(accessToken);
      var dialog = new SelectUserDialog("代理ログインのユーザー", _dialogService, users);

      await Shell.Current.ShowPopupAsync(dialog);
      if (dialog.IsCanceled)
      {
        System.Diagnostics.Debug.WriteLine("キャンセルされました");
      }
      else
      {
        System.Diagnostics.Debug.WriteLine($"選択された user_id:{dialog.SelectedUserId}");

        await _viewModel.ProxyLoginAsync(dialog.SelectedUserId);
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"代理ログインの処理中にエラーが発生: {ex.Message}");
    }
  }

  private async void OnNotificationTapped(object sender, EventArgs e)
  {
    if (isNullInstance()) return;
    MessagingCenter.Send<object>(this, "OpenNotifications");
  }
}