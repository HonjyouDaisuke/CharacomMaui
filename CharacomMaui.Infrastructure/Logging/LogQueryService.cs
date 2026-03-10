namespace CharacomMaui.Infrastructure.Logging;

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using CharacomMaui.Application.Logging;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Infrastructure.Api;

public class LogQueryService : ILogQueryService
{
  private readonly HttpClient _http;
  private readonly IAppTokenStorageService _tokenStorage;
  private readonly AppStatus _appStatus;
  private readonly IAppLogger _logger;

  public LogQueryService(HttpClient http,
                         AppStatus appStatus,
                         IAppLogger logger,
                         IAppTokenStorageService tokenStorage)
  {
    _http = http;
    if (_http.BaseAddress == null)
      throw new Exception("HttpClient.BaseAddress is NULL");
    _tokenStorage = tokenStorage;
    _appStatus = appStatus;
    _logger = logger;
  }

  public async Task<LogQueryResult> GetLogsAsync(DateTime targetDate, int limit = 50, int page = 1)
  {
    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    if (accessToken == null)
      return new LogQueryResult();
    string dateString = targetDate.ToString("yyyy/MM/dd");
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
      from = dateString,
      to = dateString,
      limit,
      page,
    });
    // _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.CreateUser, "[API]ユーザー作成", "ユーザーを作成します。");
    var content = new StringContent(json, Encoding.UTF8, "application/json");
    try
    {
      var res = await _http.PostAsync(ApiEndpoints.GetLogs, content);
      var responseBody = await res.Content.ReadAsStringAsync();
      System.Diagnostics.Debug.WriteLine(responseBody);
      var response = JsonDocument.Parse(responseBody).RootElement;

      var success = response.GetProperty("success").GetBoolean();
      if (!success)
      {
        await _logger.SystemWarning(_appStatus.UserId, ApiEndpoints.GetLogs, "[API]ログ取得", "ログの取得に失敗しました。");
        return null;
      }

      int logsCount = response.GetProperty("logs_count").GetInt32();
      var logs = new List<LogDto>();
      foreach (var item in response.GetProperty("logs").EnumerateArray())
      {
        logs.Add(new LogDto
        {
          Id = item.GetProperty("id").GetString(),
          UserId = item.GetProperty("user_id").GetString(),
          Level = item.GetProperty("level").GetString(),
          Screen = item.GetProperty("screen").GetString(),
          Action = item.GetProperty("action").GetString(),
          Message = item.GetProperty("message").GetString(),
          CreatedAt = item.GetProperty("created_at").GetString(),
        });
      }
      await _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.GetLogs, "[API]ログ取得", "ログを取得しました。");

      return new LogQueryResult
      {
        LogsCount = logsCount,
        Logs = logs
      };
    }
    catch (Exception ex)
    {
      await _logger.SystemError(ex, _appStatus.UserId, ApiEndpoints.GetLogs, "[API]ログ取得");

      return new LogQueryResult();
    }
  }
}