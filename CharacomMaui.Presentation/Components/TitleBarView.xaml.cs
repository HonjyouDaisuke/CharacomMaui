namespace CharacomMaui.Presentation.Components;

using CharacomMaui.Presentation.Dialogs;
using CharacomMaui.Presentation.ViewModels;
using UraniumUI.Dialogs;
using CommunityToolkit.Maui.Views;
using MauiApp = Microsoft.Maui.Controls;
using Mopups.Services;
using CommunityToolkit.Maui.Extensions;
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
  public TitleBarView()
  {
    InitializeComponent();
    this.BindingContext = Handler?.MauiContext?.Services.GetService<TitleBarViewModel>()
                          ?? IPlatformApplication.Current.Services.GetService<TitleBarViewModel>();
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

  }
  private async void OnAvatarViewTapped(object sender, EventArgs e)
  {
    if (_dialogService == null ||
        _notifier == null ||
        _appStatus == null ||
        _getAvatarsUrlUseCase == null ||
        _userInfoUseCase == null ||
        _tokenStorage == null ||
        _userRolesSession == null)
      return;

    var dialog = new UserProfileDialog("ユーザー情報の更新", _dialogService, _notifier, _appStatus, _getAvatarsUrlUseCase, _userInfoUseCase, _tokenStorage, _userRolesSession);

    await Shell.Current.ShowPopupAsync(dialog);
  }
}
