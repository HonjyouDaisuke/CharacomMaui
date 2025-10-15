using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.Models;

namespace CharacomMaui.Infrastructure.Services;

public class BoxApiAuthService : IBoxAuthService
{
  private readonly string _clientId;
  private readonly string _clientSecret;

  // 認可 URI、トークンエンドポイント
  private const string AuthorizationEndpoint = "https://account.box.com/api/oauth2/authorize";
  private const string TokenEndpoint = "https://api.box.com/oauth2/token";

  public BoxApiAuthService(string clientId, string clientSecret)
  {
    _clientId = clientId;
    _clientSecret = clientSecret;
  }

  public string GetAuthorizationUrl()
  {
    // 必要なら state パラメータを含める
    string redirectUri = Uri.EscapeDataString("myapp://callback");  // アプリで登録した URI に置き換え
    return $"{AuthorizationEndpoint}?response_type=code&client_id={_clientId}&redirect_uri={redirectUri}";
  }

  public async Task<BoxAuthResult> ExchangeCodeForTokenAsync(string authorizationCode, string redirectUri)
  {
    using var client = new HttpClient();

    var pairs = new List<KeyValuePair<string, string>>
        {
            new("grant_type", "authorization_code"),
            new("code", authorizationCode),
            new("client_id", _clientId),
            new("client_secret", _clientSecret),
            new("redirect_uri", redirectUri)
        };

    var content = new FormUrlEncodedContent(pairs);
    var response = await client.PostAsync(TokenEndpoint, content);
    response.EnsureSuccessStatusCode();

    var json = await response.Content.ReadAsStringAsync();
    var doc = JsonDocument.Parse(json);
    var root = doc.RootElement;

    var result = new BoxAuthResult
    {
      AccessToken = root.GetProperty("access_token").GetString(),
      RefreshToken = root.GetProperty("refresh_token").GetString(),
      ExpiresIn = root.GetProperty("expires_in").GetInt32()
    };

    return result;
  }

  public async Task<BoxAuthResult> RefreshTokenAsync(string refreshToken)
  {
    using var client = new HttpClient();

    var pairs = new List<KeyValuePair<string, string>>
        {
            new("grant_type", "refresh_token"),
            new("refresh_token", refreshToken),
            new("client_id", _clientId),
            new("client_secret", _clientSecret)
        };

    var content = new FormUrlEncodedContent(pairs);
    var response = await client.PostAsync(TokenEndpoint, content);
    response.EnsureSuccessStatusCode();

    var json = await response.Content.ReadAsStringAsync();
    var doc = JsonDocument.Parse(json);
    var root = doc.RootElement;

    var result = new BoxAuthResult
    {
      AccessToken = root.GetProperty("access_token").GetString(),
      RefreshToken = root.GetProperty("refresh_token").GetString(),
      ExpiresIn = root.GetProperty("expires_in").GetInt32()
    };

    return result;
  }
}
