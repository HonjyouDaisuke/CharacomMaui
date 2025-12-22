using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Presentation.ViewModels;
using MauiApp = Microsoft.Maui.Controls.Application;

namespace CharacomMaui.Presentation;

public partial class MainPage : ContentPage
{
  private readonly BoxLoginViewModel _boxLoginViewModel;
  private readonly CreateAppUserViewModel _createAppUserViewModel;
  private readonly GetUserInfoUseCase _userUseCase;
  private readonly AppStatusUseCase _statusUseCase;
  private bool _isLoginProcessing = false;

  public MainPage(GetUserInfoUseCase userUseCase, AppStatusUseCase statusUseCase)
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
        PictureUrl = user.avatar_url,
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
      var accessToken = Preferences.Get("app_access_token", string.Empty);
      LogEditor.Text += $"app AccessToken = {accessToken}\n";
      var userInfo = await _userUseCase.GetUserInfoAsync(accessToken);
      _statusUseCase.SetUserInfo(userInfo);

      LogEditor.Text += "終了しました...\n";

      MauiApp.Current!.Windows[0].Page = new AppShell();
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
    var window = MauiApp.Current?.Windows.FirstOrDefault();
    if (window != null)
    {
      window.Page = new AppShell();
    }
  }

}

