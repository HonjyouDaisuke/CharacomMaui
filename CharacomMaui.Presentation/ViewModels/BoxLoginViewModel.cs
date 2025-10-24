using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.Models;
using CharacomMaui.Application.UseCases;
using System.Text.Json;
using MauiApp = Microsoft.Maui.Controls.Application;

namespace CharacomMaui.Presentation.ViewModels;

public class BoxLoginViewModel
{
  private readonly LoginToBoxUseCase _loginUseCase;
  private readonly GetBoxConfigUseCase _getBoxConfigUseCase;
  private readonly ITokenStorageService _tokenStorage;


  public BoxLoginViewModel(
      LoginToBoxUseCase loginUseCase,
      GetBoxConfigUseCase getBoxConfigUseCase,
      ITokenStorageService tokenStorage)
  {
    _loginUseCase = loginUseCase;
    _getBoxConfigUseCase = getBoxConfigUseCase;
    _tokenStorage = tokenStorage;

  }



  public async Task<BoxAuthResult?> LoginAsync()
  {

    var (clientId, clientSecret) = await _getBoxConfigUseCase.ExecuteAsync();
    var authUrl = _loginUseCase.GetAuthorizationUrl(clientId, clientSecret);
    var tokens = new BoxAuthResult();
    System.Diagnostics.Debug.WriteLine("🟢 [OAuthDebug]Login start!!.");
    if (DeviceInfo.Platform == DevicePlatform.WinUI || DeviceInfo.Platform == DevicePlatform.Android)
    {
      // WebViewで認可コード取得
      var tcs = new TaskCompletionSource<BoxAuthResult>();
      var webView = new WebView { Source = authUrl };
      var page = new ContentPage { Content = webView };
      var callbackUrl = "myapp://callback";
      webView.Navigating += async (s, ev) =>
      {
        if (ev.Url.StartsWith(callbackUrl, StringComparison.OrdinalIgnoreCase))
        {
          ev.Cancel = true;
          var uri = new Uri(ev.Url);
          var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
          var code = query.Get("code");

          if (!string.IsNullOrEmpty(code))
          {
            try
            {
              // 認可コードからBoxAuthResultを取得
              var token = await ExchangeCodeForTokenAsync(clientId, clientSecret, code);
              tcs.TrySetResult(token);
            }
            catch (Exception ex)
            {
              tcs.TrySetException(ex);
            }

            // WebViewを閉じる
            await MauiApp.Current.MainPage.Navigation.PopModalAsync();
          }
          else
          {
            tcs.TrySetException(new Exception("認可コードが取得できませんでした"));
          }
        }
      };

      await MauiApp.Current.MainPage.Navigation.PushModalAsync(page);
      tokens = await tcs.Task; // ここがTask<BoxAuthResult>になる
    }
    else
    {
      var callbackUrl = new Uri("myapp://callback");
      var result = await WebAuthenticator.AuthenticateAsync(new Uri(authUrl), callbackUrl);
      if (!result.Properties.TryGetValue("code", out var code))
        throw new Exception("認可コードが取得できませんでした。");

      tokens = await _loginUseCase.LoginWithCodeAsync(code, callbackUrl.ToString());

    }
    await _tokenStorage.SaveTokensAsync(tokens);
    return tokens;

  }

  public async Task<BoxAuthResult> ExchangeCodeForTokenAsync(
  string clientId,
  string clientSecret,
  string authorizationCode)
  {
    using var client = new HttpClient();

    var pairs = new List<KeyValuePair<string, string>>
    {
        new("grant_type", "authorization_code"),
        new("code", authorizationCode),
        new("client_id", clientId),
        new("client_secret", clientSecret),
        new("redirect_uri", "myapp://callback")
    };

    var content = new FormUrlEncodedContent(pairs);
    var response = await client.PostAsync("https://api.box.com/oauth2/token", content);
    response.EnsureSuccessStatusCode();

    var json = await response.Content.ReadAsStringAsync();
    using var doc = JsonDocument.Parse(json);
    var root = doc.RootElement;

    return new BoxAuthResult
    {
      AccessToken = root.GetProperty("access_token").GetString() ?? "",
      RefreshToken = root.GetProperty("refresh_token").GetString() ?? "",
      ExpiresAt = root.GetProperty("expires_in").GetInt32()
    };
  }
}
