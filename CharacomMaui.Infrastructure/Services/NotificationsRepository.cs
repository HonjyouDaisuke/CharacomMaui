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
  public NotificationsRepository(HttpClient http)
  {
    _http = http;
    if (_http.BaseAddress == null)
      throw new Exception("HttpClient.BaseAddress is NULL");
  }

  public async Task<List<NotificationItem>?> GetNotificationsAsync(string accessToken)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
    });
    var content = new StringContent(json, Encoding.UTF8, "application/json");


    using var res = await _http.PostAsync(ApiEndpoints.GetNotifications, content);
    res.EnsureSuccessStatusCode();
    var responseBody = await res.Content.ReadAsStringAsync();
    System.Diagnostics.Debug.WriteLine("----------GetNotifications server res--------------");
    System.Diagnostics.Debug.WriteLine(responseBody);

    try
    {
      var result = JsonSerializer.Deserialize<GetNotificationsResponse>(responseBody);
      if (result?.Success != true)
      {
        System.Diagnostics.Debug.WriteLine("通知メッセージが見つかりませんでした。");
        return null;
      }
      System.Diagnostics.Debug.WriteLine($"通知件数: {result?.Notifications?.Count}");
      return result?.Notifications ?? new List<NotificationItem>();

    }
    catch (JsonException ex)
    {
      System.Diagnostics.Debug.WriteLine("!!!! JSON Deserialize ERROR !!!!");
      System.Diagnostics.Debug.WriteLine(ex.Message);
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
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    using var res = await _http.PostAsync(ApiEndpoints.UpdateNotificationRead, content);
    res.EnsureSuccessStatusCode();
    var responseBody = await res.Content.ReadAsStringAsync();
    System.Diagnostics.Debug.WriteLine("----------UpdateNotificationRead server res--------------");
    System.Diagnostics.Debug.WriteLine(responseBody);
    try
    {
      using var response = JsonDocument.Parse(responseBody);
      if (response.RootElement.TryGetProperty("success", out var successElem)
          && successElem.GetBoolean())
      {
        return true;
      }
      System.Diagnostics.Debug.WriteLine("既読設定に失敗しました");
      return false;
    }
    catch (JsonException ex)
    {
      System.Diagnostics.Debug.WriteLine("!!!! JSON Parse ERROR !!!!");
      System.Diagnostics.Debug.WriteLine(ex.Message);
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
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    using var res = await _http.PostAsync(ApiEndpoints.UpdateNotificationDeleted, content);
    res.EnsureSuccessStatusCode();
    var responseBody = await res.Content.ReadAsStringAsync();
    System.Diagnostics.Debug.WriteLine("----------UpdateNotificationDeleted server res--------------");
    System.Diagnostics.Debug.WriteLine(responseBody);
    try
    {
      using var response = JsonDocument.Parse(responseBody);
      if (response.RootElement.TryGetProperty("success", out var successElem)
          && successElem.GetBoolean())
      {
        return true;
      }
      System.Diagnostics.Debug.WriteLine("通知の削除に失敗しました");
      return false;
    }
    catch (JsonException ex)
    {
      System.Diagnostics.Debug.WriteLine("!!!! JSON Parse ERROR !!!!");
      System.Diagnostics.Debug.WriteLine(ex.Message);
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