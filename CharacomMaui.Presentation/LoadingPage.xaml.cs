
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Presentation.Helpers;
using MauiApp = Microsoft.Maui.Controls.Application;

namespace CharacomMaui.Presentation;

public partial class LoadingPage : ContentPage
{
  private readonly GetUserInfoUseCase _userUseCase;
  private readonly AppStatusUseCase _statusUseCase;
  public LoadingPage(GetUserInfoUseCase userUseCase, AppStatusUseCase statusUseCase)
  {
    InitializeComponent();

    _userUseCase = userUseCase;
    _statusUseCase = statusUseCase;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();
    System.Diagnostics.Debug.WriteLine($"スタート----");

    var accessToken = Preferences.Get("app_access_token", string.Empty);
    var tokenService = ServiceHelper.GetService<ITokenValidationService>();
    bool isValid = false;
    System.Diagnostics.Debug.WriteLine($"チェック開始 : {accessToken}");
    if (!string.IsNullOrEmpty(accessToken))
    {
      try
      {
        var result = await tokenService.ValidateAsync(accessToken);
        System.Diagnostics.Debug.WriteLine($"tokenRes = {result}");

        isValid = result.Success;
      }
      catch
      {
        isValid = false;
        System.Diagnostics.Debug.WriteLine($"Error --tokenRes isValid = {isValid}");
        MauiApp.Current!.Windows[0].Page = new MainPage(_userUseCase, _statusUseCase);
        return;
      }
    }
    System.Diagnostics.Debug.WriteLine($"isValid = {isValid}");
    if (!isValid)
    {
      System.Diagnostics.Debug.WriteLine($"TokenError = {isValid}");
      MauiApp.Current!.Windows[0].Page = new MainPage(_userUseCase, _statusUseCase);
      return;
    }
    var user = await _userUseCase.GetUserInfoAsync(accessToken);

    if (user == null || user.Id == null)
    {
      isValid = false;
    }
    System.Diagnostics.Debug.WriteLine($"Token有効後のisValid = {isValid}");
    if (isValid)
    {
      var avaterImg = await _userUseCase.GetAvatarImgStringAsync(accessToken);
      System.Diagnostics.Debug.WriteLine($"avaterImg = {avaterImg}");
      user.AvatarImgString = avaterImg;
      _statusUseCase.SetUserInfo(user);

      MauiApp.Current!.Windows[0].Page = new AppShell();
    }
    else
    {
      MauiApp.Current!.Windows[0].Page = new MainPage(_userUseCase, _statusUseCase);
    }
  }
}
