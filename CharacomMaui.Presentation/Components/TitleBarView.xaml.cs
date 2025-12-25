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

public partial class TitleBarView : ContentView
{
  private IDialogService? _dialogService;
  private AppStatusNotifier? _notifier;
  private AppStatus? _appStatus;
  private GetAvatarsUrlUseCase? _getAvatarsUrlUseCase;
  private UpdateUserInfoUseCase? _userInfoUseCase;
  public TitleBarView()
  {
    InitializeComponent();
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
    var vm = services.GetService<TitleBarViewModel>();

    if (vm == null)
    {
      System.Diagnostics.Debug.WriteLine("DI に TitleBarViewModel が登録されていません！");
      return;
    }

    BindingContext = vm;
  }
  private async void OnAvatarViewTapped(object sender, EventArgs e)
  {
    if (_dialogService == null || _notifier == null || _appStatus == null || _getAvatarsUrlUseCase == null || _userInfoUseCase == null)
      return;

    var dialog = new UserProfileDialog("ユーザー情報の更新", _dialogService, _notifier, _appStatus, _getAvatarsUrlUseCase, _userInfoUseCase);

    await Shell.Current.ShowPopupAsync(dialog);
  }
}
