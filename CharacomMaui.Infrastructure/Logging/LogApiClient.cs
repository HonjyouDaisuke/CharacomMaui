namespace CharacomMaui.Infrastructure.Logging;

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;   // ← これを追加

using CharacomMaui.Application.Logging;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Infrastructure.Api;

public class LogApiClient : ILogApiClient
{
  private readonly HttpClient _http;
  private readonly IAppTokenStorageService _tokenStorage;

  public LogApiClient(HttpClient http, IAppTokenStorageService tokenStorage)
  {
    _http = http;
    if (_http.BaseAddress == null)
      throw new Exception("HttpClient.BaseAddress is NULL");
    _tokenStorage = tokenStorage;
  }

  public async Task SendAsync(LogRequest request)
  {
    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    if (accessToken == null) return;

    var payload = new
    {
      token = accessToken,
      log_data = new
      {
        level = request.Level,
        screen = request.Screen,
        action = request.Action,
        message = request.Message,
        data = request.Data,
        correlation_id = request.CorrelationId
      }
    };

    var json = JsonSerializer.Serialize(payload);
    var content = new StringContent(
        json,
        Encoding.UTF8,
        "application/json");

    try
    {
      var res = await _http.PostAsync(
          ApiEndpoints.AddLog,
          content);
      var responseBody = await res.Content.ReadAsStringAsync();
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"-失敗{ex}");
    }
  }
}