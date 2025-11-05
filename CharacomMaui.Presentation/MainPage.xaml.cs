using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Infrastructure.Services;
using CharacomMaui.Presentation.ViewModels;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Text.Json;
using MauiApp = Microsoft.Maui.Controls.Application;

namespace CharacomMaui.Presentation;

public partial class MainPage : ContentPage
{
  private readonly BoxLoginViewModel _boxLoginViewModel;
  private readonly CreateAppUserViewModel _createAppUserViewModel;
  public MainPage()
  {
    try
    {
      InitializeComponent();
      _boxLoginViewModel = MauiApp.Current.Handler.MauiContext.Services
                               .GetRequiredService<BoxLoginViewModel>();
      _createAppUserViewModel = MauiApp.Current.Handler.MauiContext.Services
                             .GetRequiredService<CreateAppUserViewModel>();
      BindingContext = _boxLoginViewModel;
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[MainPage ctor] {ex}");
      throw;
    }
  }

  private async void OnLoginClicked(object sender, EventArgs e)
  {
    LogEditor.Text += "ログイン処理を開始...\n";
    var res = await _boxLoginViewModel.LoginAsync();
    if (res == null)
    {
      LogEditor.Text += "ログインに失敗しました。。。\nå";
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
    if (success)
    {
      LogEditor.Text += "ユーザー情報を保存しました...\n";
      var accessToken = Preferences.Get("app_access_token", string.Empty);
      LogEditor.Text += $"app AccessToken = {accessToken}\n";
    }
    else
    {
      LogEditor.Text += "ユーザー情報保存に失敗\n";
    }

    LogEditor.Text += "終了しました...\n";

    var window = MauiApp.Current?.Windows.FirstOrDefault();
    if (window != null)
    {
      window.Page = new AppShell();
    }
    /**
    StatusLabel.Text = "ログイン処理を開始...";
    await _boxLoginViewModel.LoginAsync();
    var accessToken = Preferences.Get(BOX_ACCESS_TOKEN, string.Empty);
    System.Diagnostics.Debug.WriteLine("ユーザ情報取得開始");
    await _boxLoginViewModel.GetUserInfoAsync(accessToken);
    StatusLabel.Text = "ログイン成功！";
    var window = MauiApp.Current?.Windows.FirstOrDefault();
    if (window != null)
    {
      window.Page = new AppShell();
    }
    **/
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

