
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Presentation.Helpers;
using CharacomMaui.Presentation.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using MauiApp = Microsoft.Maui.Controls.Application;

namespace CharacomMaui.Presentation;

public partial class LoadingPage : ContentPage
{
  private readonly IGetUserInfoUseCase _userUseCase;
  private readonly AppStatusUseCase _statusUseCase;
  private readonly IAppTokenStorageService _tokenStorage;
  private readonly FetchUserRolesUseCase _userRolesUseCase;
  public LoadingPage(IGetUserInfoUseCase userUseCase, AppStatusUseCase statusUseCase, IAppTokenStorageService tokenStorage, FetchUserRolesUseCase userRolesUseCase)
  {
    InitializeComponent();

    _userUseCase = userUseCase;
    _statusUseCase = statusUseCase;
    _tokenStorage = tokenStorage;
    _userRolesUseCase = userRolesUseCase;
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
        bool isProxy = false;

        if (result.Payload.ValueKind == JsonValueKind.Object &&
            result.Payload.TryGetProperty("is_proxy", out var isProxyProp))
        {
          isProxy = isProxyProp.ValueKind == JsonValueKind.True
                 || (isProxyProp.ValueKind == JsonValueKind.String &&
                     bool.TryParse(isProxyProp.GetString(), out var b) && b);
        }

        isValid = result.Success;
        if (isProxy)
        {
          System.Diagnostics.Debug.WriteLine("前回代理ログインで終了したので、ログインし直してください");
          isValid = false;
        }
      }
      catch
      {
        isValid = false;
        await SnackBarService.Error("AccessTokenが有効ではありません。");
        System.Diagnostics.Debug.WriteLine($"Error --tokenRes isValid = {isValid}");
        MauiApp.Current!.Windows[0].Page = new MainPage(_userUseCase, _statusUseCase, _tokenStorage, _userRolesUseCase);
        return;
      }
    }
    System.Diagnostics.Debug.WriteLine($"isValid = {isValid}");
    if (!isValid)
    {
      System.Diagnostics.Debug.WriteLine($"TokenError = {isValid}");
      MauiApp.Current!.Windows[0].Page = new MainPage(_userUseCase, _statusUseCase, _tokenStorage, _userRolesUseCase);
      return;
    }
    // ユーザー権限一覧を取得
    await _userRolesUseCase.ExecuteAsync(accessToken!);

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
      MauiApp.Current!.Windows[0].Page = new MainPage(_userUseCase, _statusUseCase, _tokenStorage, _userRolesUseCase);
    }
  }
}
