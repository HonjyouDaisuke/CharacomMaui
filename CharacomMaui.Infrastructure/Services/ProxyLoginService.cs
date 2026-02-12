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

  public async Task<AppTokenResult> ProxyLoginAsync(string accessToken, string toUserId)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
      to_user_id = toUserId,
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
