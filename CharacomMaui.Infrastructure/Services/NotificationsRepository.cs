using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Infrastructure.Api;
using CharacomMaui.Infrastructure.Helpers;

namespace CharacomMaui.Infrastructure.Services;

public class NotificationsRepository : INotificationsRepository
{
  private readonly HttpClient _http;
  private readonly AppStatus _appStatus;
  private readonly IAppLogger _logger;
  public NotificationsRepository(HttpClient http, AppStatus appStatus, IAppLogger logger)
  {
    _http = http;
    if (_http.BaseAddress == null)
      throw new Exception("HttpClient.BaseAddress is NULL");
    _appStatus = appStatus;
    _logger = logger;
  }

  public async Task<List<NotificationItem>?> GetNotificationsAsync(string accessToken)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
    });

    _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.GetNotifications, "[API]通知の取得", "通知を取得します。");
    var content = new StringContent(json, Encoding.UTF8, "application/json");


    using var res = await _http.PostAsync(ApiEndpoints.GetNotifications, content);
    res.EnsureSuccessStatusCode();
    var responseBody = await res.Content.ReadAsStringAsync();

    try
    {
      var result = JsonSerializer.Deserialize<GetNotificationsResponse>(responseBody);
      if (result?.Success != true)
      {
        _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.GetNotifications, "[API]通知の取得", "通知メッセージが見つかりませんでした。");
        return null;
      }
      _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.GetNotifications, "[API]通知の取得", "通知メッセージを取得しました。", new { result?.Notifications?.Count });
      return result?.Notifications ?? new List<NotificationItem>();

    }
    catch (JsonException ex)
    {
      _logger.SystemError(ex, _appStatus.UserId, ApiEndpoints.GetNotifications, "[API]JSON形式への変換");
      return null;
    }
  }

  public async Task<bool> UpdateNotificationReadAsync(string accessToken, string id)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
      notification_id = id,
    });
    _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.UpdateNotificationRead, "[API]通知の既読アップデート", "通知を既読にします。", new { id });
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    using var res = await _http.PostAsync(ApiEndpoints.UpdateNotificationRead, content);
    res.EnsureSuccessStatusCode();
    var responseBody = await res.Content.ReadAsStringAsync();
    try
    {
      using var response = JsonDocument.Parse(responseBody);
      if (response.RootElement.TryGetProperty("success", out var successElem)
          && successElem.GetBoolean())
      {
        _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.UpdateNotificationRead, "[API]通知の既読アップデート", "通知を既読にしました。", new { id });
        return true;
      }
      _logger.SystemWarning(_appStatus.UserId, ApiEndpoints.UpdateNotificationRead, "[API]通知の既読アップデート", "通知の既読設定に失敗しました。", new { id });
      return false;
    }
    catch (JsonException ex)
    {
      _logger.SystemError(ex, _appStatus.UserId, ApiEndpoints.UpdateNotificationRead, "[API]通知の既読アップデート", new { id });
      return false;
    }
  }
  public async Task<bool> UpdateNotificationDeletedAsync(string accessToken, string id)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
      notification_id = id,
    });
    _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.UpdateNotificationDeleted, "[API]通知の削除設定", "通知を削除済みにします。", new { id });
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    using var res = await _http.PostAsync(ApiEndpoints.UpdateNotificationDeleted, content);
    res.EnsureSuccessStatusCode();
    var responseBody = await res.Content.ReadAsStringAsync();
    try
    {
      using var response = JsonDocument.Parse(responseBody);
      if (response.RootElement.TryGetProperty("success", out var successElem)
          && successElem.GetBoolean())
      {
        _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.UpdateNotificationDeleted, "[API]通知の削除設定", "通知を削除済みにしました。", new { id });
        return true;
      }
      _logger.SystemWarning(_appStatus.UserId, ApiEndpoints.UpdateNotificationDeleted, "[API]通知の削除設定", "通知の削除に失敗しました。", new { id });
      return false;
    }
    catch (JsonException ex)
    {
      _logger.SystemError(ex, _appStatus.UserId, ApiEndpoints.UpdateNotificationDeleted, "[API]通知の削除設定", new { id });
      return false;
    }
  }
}
public class GetNotificationsResponse
{
  [JsonPropertyName("success")]
  public bool Success { get; set; }
  [JsonPropertyName("notifications")]
  public List<NotificationItem> Notifications { get; set; }
}