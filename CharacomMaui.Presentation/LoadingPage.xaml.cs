
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Presentation.Helpers;
using CharacomMaui.Presentation.Services;
using MauiApp = Microsoft.Maui.Controls.Application;

namespace CharacomMaui.Presentation;

public partial class LoadingPage : ContentPage
{
  private readonly IGetUserInfoUseCase _userUseCase;
  private readonly AppStatusUseCase _statusUseCase;
  private readonly IAppTokenStorageService _tokenStorage;
  public LoadingPage(IGetUserInfoUseCase userUseCase, AppStatusUseCase statusUseCase, IAppTokenStorageService tokenStorage)
  {
    InitializeComponent();

    _userUseCase = userUseCase;
    _statusUseCase = statusUseCase;
    _tokenStorage = tokenStorage;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();
    System.Diagnostics.Debug.WriteLine($"スタート----");
    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
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
        await SnackBarService.Error("AccessTokenが有効ではありません。");
        System.Diagnostics.Debug.WriteLine($"Error --tokenRes isValid = {isValid}");
        MauiApp.Current!.Windows[0].Page = new MainPage(_userUseCase, _statusUseCase, _tokenStorage);
        return;
      }
    }
    System.Diagnostics.Debug.WriteLine($"isValid = {isValid}");
    if (!isValid)
    {
      System.Diagnostics.Debug.WriteLine($"TokenError = {isValid}");
      MauiApp.Current!.Windows[0].Page = new MainPage(_userUseCase, _statusUseCase, _tokenStorage);
      return;
    }

    var user = await _userUseCase.GetUserInfoAsync(accessToken!);

    if (user == null || user.Id == null)
    {
      isValid = false;
    }
    System.Diagnostics.Debug.WriteLine($"Token有効後のisValid = {isValid}");
    if (isValid)
    {
      if (user != null) _statusUseCase.SetUserInfo(user);
      MainThread.BeginInvokeOnMainThread(() =>
      {
        MauiApp.Current!.Windows[0].Page = new AppShell();
      });
    }
    else
    {
      MauiApp.Current!.Windows[0].Page = new MainPage(_userUseCase, _statusUseCase, _tokenStorage);
    }
  }
}
