using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Infrastructure.Api;
using CharacomMaui.Application.Interfaces;

namespace CharacomMaui.Infrastructure.Services;

public class ProxyLoginService : IProxyLoginService
{
  private readonly HttpClient _http;

  public ProxyLoginService(HttpClient http)
  {
    _http = http;
    if (_http.BaseAddress == null)
      throw new InvalidOperationException("HttpClient.BaseAddress is NULL");
  }

  public async Task<AppTokenResult> ProxyLoginAsync(string accessToken, AppUser user, string toUserId, string toUserName, string toUserEmail, string toBoxUserId)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
      user_name = user.Name,
      user_email = user.Email,
      user_role_id = user.RoleId,
      to_user_id = toUserId,
      to_user_name = toUserName,
      to_user_email = toUserEmail,
      to_box_user_id = toBoxUserId,
      to_box_access_token = user.BoxAccessToken,
      to_box_refresh_token = user.BoxRefreshToken,
    });

    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var res = await _http.PostAsync(ApiEndpoints.ProxyLogin, content);
    res.EnsureSuccessStatusCode();
    var responseBody = await res.Content.ReadAsStringAsync();

    System.Diagnostics.Debug.WriteLine("---------- ProxyLogin server res--------------");
    System.Diagnostics.Debug.WriteLine(responseBody);
    try
    {
      var result = JsonSerializer.Deserialize<AppTokenResult>(responseBody);
      if (result == null)
        throw new InvalidOperationException("ProxyLogin response deserialized to null");
      return result;
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine("!!!! JSON Deserialize ERROR !!!!");
      System.Diagnostics.Debug.WriteLine(ex.Message);
      throw; // ← ここで投げれば MAUI の output にも出る
    }
  }
}
