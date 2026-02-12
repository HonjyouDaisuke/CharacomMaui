using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Infrastructure.Api;
using CharacomMaui.Application.Interfaces;

namespace CharacomMaui.Infrastructure.Services;

public class ProxyLogoutService : IProxyLogoutService
{
  private readonly HttpClient _http;

  public ProxyLogoutService(HttpClient http)
  {
    _http = http;
    if (_http.BaseAddress == null)
      throw new InvalidOperationException("HttpClient.BaseAddress is NULL");
  }

  public async Task<AppTokenResult> ProxyLogout(string accessToken)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
    });

    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var res = await _http.PostAsync(ApiEndpoints.ProxyLogout, content);
    res.EnsureSuccessStatusCode();
    var responseBody = await res.Content.ReadAsStringAsync();

    System.Diagnostics.Debug.WriteLine("---------- ProxyLogout server res--------------");
    System.Diagnostics.Debug.WriteLine($"AccessToken = {accessToken}");
    System.Diagnostics.Debug.WriteLine(responseBody);
    try
    {
      var result = JsonSerializer.Deserialize<AppTokenResult>(responseBody);
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
