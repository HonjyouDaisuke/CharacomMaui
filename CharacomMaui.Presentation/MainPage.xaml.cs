using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Presentation.ViewModels;
using MauiApp = Microsoft.Maui.Controls.Application;

namespace CharacomMaui.Presentation;

public partial class MainPage : ContentPage
{
  private readonly BoxLoginViewModel _boxLoginViewModel;
  private readonly CreateAppUserViewModel _createAppUserViewModel;
  private readonly IGetUserInfoUseCase _userUseCase;
  private readonly IAppTokenStorageService _tokenStorage;
  private readonly AppStatusUseCase _statusUseCase;
  private readonly AppStatus _appStatus;
  private readonly IAppLogger _logger;
  private readonly FetchUserRolesUseCase _userRolesUseCase;
  private bool _isLoginProcessing = false;

  public MainPage(
    IGetUserInfoUseCase userUseCase,
    AppStatusUseCase statusUseCase,
    IAppTokenStorageService tokenStorage,
    AppStatus appStatus,
    IAppLogger logger,
    FetchUserRolesUseCase userRolesUseCase)
  {
    try
    {
      InitializeComponent();
      _boxLoginViewModel = MauiApp.Current!.Handler.MauiContext!.Services
                               .GetRequiredService<BoxLoginViewModel>();
      _createAppUserViewModel = MauiApp.Current.Handler.MauiContext.Services
                             .GetRequiredService<CreateAppUserViewModel>();
      BindingContext = _boxLoginViewModel;
      _userUseCase = userUseCase;
      _userRolesUseCase = userRolesUseCase;
      _statusUseCase = statusUseCase;
      _logger = logger;
      _appStatus = appStatus;
      _tokenStorage = tokenStorage;
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[MainPage ctor] {ex}");
      throw;
    }
  }

  private async void OnLoginClicked(object sender, EventArgs e)
  {
    if (_isLoginProcessing) return;

    _isLoginProcessing = true;
    try
    {
      if (sender is VisualElement button) button.IsEnabled = false; //UIをロック

      await _logger.UserAction("", this.GetType().Name, "ログイン", "ログイン処理を開始します。");
      LogEditor.Text += "ログイン処理を開始...\n";
      var res = await _boxLoginViewModel.LoginAsync();
      if (res == null)
      {
        await _logger.UserAction("", this.GetType().Name, "ログイン", "ログインに失敗しました。");
        LogEditor.Text += "ログインに失敗しました。。。\n";
        return;
      }
      var user = await _boxLoginViewModel.GetUserInfoAsync(res.AccessToken);
      LogEditor.Text += $"AccessToken = {res.AccessToken}\n";
      LogEditor.Text += $"RefreshToken = {res.RefreshToken}\n";
      LogEditor.Text += $"id = {user.id}\n";
      LogEditor.Text += $"name = {user.name}\n";
      LogEditor.Text += $"login = {user.login}\n";
      LogEditor.Text += $"picture = {user.avatar_url}\n";
      LogEditor.Text += $"status = {user.status}\n";

      var appUser = new AppUser
      {
        Id = user.id,
        Name = user.name,
        Email = user.login,
        BoxAccessToken = res.AccessToken,
        BoxRefreshToken = res.RefreshToken,
      };
      // ユーザーをアプリに登録
      var success = await _createAppUserViewModel.CreateUserAsync(appUser);

      // 失敗終了（早期リターン）
      if (!success)
      {
        await _logger.SystemWarning(user.id, this.GetType().Name, "ログイン", "ユーザー情報登録に失敗しました。");
        LogEditor.Text += "ユーザー情報保存に失敗\n";
        return;
      }
      await _logger.UserAction(appUser.Id, this.GetType().Name, "ログイン", "ユーザーを作成しました。", new { appUser.Name });

      LogEditor.Text += "ユーザー情報を保存しました...\n";
      var tokens = await _tokenStorage.GetTokensAsync();
      var accessToken = tokens?.AccessToken;
      LogEditor.Text += $"app AccessToken = {accessToken}\n";
      // ユーザー権限一覧を取得
      await _userRolesUseCase.ExecuteAsync(accessToken!);
      await _logger.SystemInfo(appUser.Id, this.GetType().Name, "ログイン", "ユーザー情報詳細を取得しました。", new { appUser.Name });

      var userInfo = await _userUseCase.GetUserInfoAsync(accessToken);
      _statusUseCase.SetUserInfo(userInfo);

      LogEditor.Text += "終了しました...\n";
      MainThread.BeginInvokeOnMainThread(() =>
      {
        // App クラスのインスタンスをキャストしてメソッドを呼ぶ
        MauiApp.Current!.Windows[0].Page = new AppShell(_appStatus);
      });
      //MauiApp.Current!.Windows[0].Page = new AppShell();
    }
    catch (Exception ex)
    {
      LogEditor.Text += $"エラーが発生しました：{ex.Message}\n";
      await _logger.UserActionError(ex, "", this.GetType().Name, "ログイン", "ログイン中にエラーが発生しました。");
    }
    finally
    {
      _isLoginProcessing = false;
      if (sender is VisualElement button) button.IsEnabled = true;
    }
  }

  private async void OnNewPageButtonClick(object sender, EventArgs e)
  {
    await SecureStorage.SetAsync("test", "hello");
    var v = await SecureStorage.GetAsync("test");
    LogEditor.Text += v; // "hello" が出れば SecureStorage OK
  }

}

