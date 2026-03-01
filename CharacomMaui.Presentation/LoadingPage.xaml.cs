
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Application.Interfaces;
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
  private readonly IAppLogger _logger;

  public LoadingPage(
    IAppLogger logger,
    IGetUserInfoUseCase userUseCase,
   AppStatusUseCase statusUseCase,
    IAppTokenStorageService tokenStorage,
     FetchUserRolesUseCase userRolesUseCase)
  {
    InitializeComponent();

    _userUseCase = userUseCase;
    _statusUseCase = statusUseCase;
    _tokenStorage = tokenStorage;
    _userRolesUseCase = userRolesUseCase;
    _logger = logger;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();
    _logger.UserAction("", this.GetType().Name, "システムスタート", "Characom Imager Pro Maui Start--------");
    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    var tokenService = ServiceHelper.GetService<ITokenValidationService>();
    bool isValid = false;
    if (!string.IsNullOrEmpty(accessToken))
    {
      try
      {
        var result = await tokenService.ValidateAsync(accessToken);
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
          _logger.SystemWarning("", this.GetType().Name, "自動ログイン", "前回代理ログインで終了したので、ログインページに戻ります。");
          await _tokenStorage.ClearTokensAsync();
          isValid = false;
        }
      }
      catch
      {
        isValid = false;
        await SnackBarService.Error("AccessTokenが有効ではありません。");
        _logger.SystemWarning("", this.GetType().Name, "自動ログイン", "アクセストークンの有効期限が切れていますので、ログインページに戻ります。");
        MauiApp.Current!.Windows[0].Page = new MainPage(_userUseCase, _statusUseCase, _tokenStorage, _logger, _userRolesUseCase);
        return;
      }
    }
    System.Diagnostics.Debug.WriteLine($"isValid = {isValid}   ----");
    if (!isValid)
    {
      _logger.SystemWarning("", this.GetType().Name, "自動ログイン", "アクセストークンが有効ではありません。ログインページに戻ります。");
      MauiApp.Current!.Windows[0].Page = new MainPage(_userUseCase, _statusUseCase, _tokenStorage, _logger, _userRolesUseCase);
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
        _logger.UserAction(user.Id, this.GetType().Name, "自動ログイン", "自動ログイン 成功");
        MauiApp.Current!.Windows[0].Page = new AppShell();
      });
    }
    else
    {
      _logger.SystemWarning(user.Id, this.GetType().Name, "自動ログイン", "自動ログインできなかったので、ログインページに戻ります。");
      MauiApp.Current!.Windows[0].Page = new MainPage(_userUseCase, _statusUseCase, _tokenStorage, _logger, _userRolesUseCase);
    }
  }
}
