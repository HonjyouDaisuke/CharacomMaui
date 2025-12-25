using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace CharacomMaui.Infrastructure.Services;

public class BoxApiAuthService : IBoxApiAuthService
{
  private string _clientId;
  private string _clientSecret;

  // 認可 URI、トークンエンドポイント
  private const string AuthorizationEndpoint = "https://account.box.com/api/oauth2/authorize";
  private const string TokenEndpoint = "https://api.box.com/oauth2/token";
  private const string RedirectUri = "myapp://callback";

  public BoxApiAuthService()
  {
  }

  public void SetBoxKeyString(string clientId, string clientSecret)
  {
    _clientId = clientId;
    _clientSecret = clientSecret;
  }

  public async Task<BoxUserResult> GetUserInfoAsync(string accessToken)
  {
    using var http = new HttpClient();
    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    var response = await http.GetAsync($"https://api.box.com/2.0/users/me");
    var json = await response.Content.ReadAsStringAsync();
    response.EnsureSuccessStatusCode();
    using var doc = JsonDocument.Parse(json);
    var root = doc.RootElement;

    var user = new BoxUserResult
    {
      id = root.GetProperty("id").GetString(),
      name = root.GetProperty("name").GetString(),
      login = root.GetProperty("login").GetString(),
      status = root.GetProperty("status").GetString(),
      // avatar_url = root.GetProperty("avatar_url").GetString(),
    };

    return user;
    //return JsonSerializer.Deserialize<BoxUserInfo>(json) ?? new BoxUserInfo();
  }
  public string GetAuthorizationUrl(string clientId, string clientSecret)
  {
    _clientId = clientId;
    _clientSecret = clientSecret;
    // BoxのOAuth2認可エンドポイントに必要なクエリを組み立てる
    var baseUrl = "https://account.box.com/api/oauth2/authorize";
    var url =
        $"{baseUrl}?response_type=code" +
        $"&client_id={Uri.EscapeDataString(clientId)}" +
        $"&redirect_uri={Uri.EscapeDataString(RedirectUri)}";
    return url;
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
            new("redirect_uri", "myapp://callback")
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
      ExpiresAt = root.GetProperty("expires_in").GetInt32()
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
      ExpiresAt = root.GetProperty("expires_in").GetInt32()
    };

    return result;
  }
}
