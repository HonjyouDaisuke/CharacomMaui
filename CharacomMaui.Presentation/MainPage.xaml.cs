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

  private bool _isLoginProcessing = false;

  public MainPage(IGetUserInfoUseCase userUseCase, AppStatusUseCase statusUseCase, IAppTokenStorageService tokenStorage)
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
      _statusUseCase = statusUseCase;
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


      LogEditor.Text += "ログイン処理を開始...\n";
      var res = await _boxLoginViewModel.LoginAsync();
      if (res == null)
      {
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
        LogEditor.Text += "ユーザー情報保存に失敗\n";
        return;
      }

      LogEditor.Text += "ユーザー情報を保存しました...\n";
      var tokens = await _tokenStorage.GetTokensAsync();
      var accessToken = tokens?.AccessToken;
      LogEditor.Text += $"app AccessToken = {accessToken}\n";
      var userInfo = await _userUseCase.GetUserInfoAsync(accessToken);
      _statusUseCase.SetUserInfo(userInfo);

      LogEditor.Text += "終了しました...\n";
      MainThread.BeginInvokeOnMainThread(() =>
      {
        // App クラスのインスタンスをキャストしてメソッドを呼ぶ
        if (MauiApp.Current is App myApp)
        {
          myApp.MainPage = new AppShell();
        }
      });
      //MauiApp.Current!.Windows[0].Page = new AppShell();
    }
    catch (Exception ex)
    {
      LogEditor.Text += $"エラーが発生しました：{ex.Message}\n";
      System.Diagnostics.Debug.WriteLine($"[Login Error] {ex.Message}");
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

