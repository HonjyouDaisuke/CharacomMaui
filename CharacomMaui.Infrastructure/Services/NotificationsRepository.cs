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

  public async Task<List<NotificationItem>?> GetNotifications(string accessToken)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
    });
    var content = new StringContent(json, Encoding.UTF8, "application/json");


    using var res = await _http.PostAsync(ApiEndpoints.GetNotifications, content);
    var responseBody = await res.Content.ReadAsStringAsync();
    System.Diagnostics.Debug.WriteLine("----------GetNotifications server res--------------");
    System.Diagnostics.Debug.WriteLine(responseBody);
    var response = JsonDocument.Parse(responseBody);
    var success = response.RootElement.GetProperty("success").GetBoolean();
    if (!success)
    {
      System.Diagnostics.Debug.WriteLine("通知メッセージが見つかりませんでした。");
      return null;
    }
    try
    {
      var result = JsonSerializer.Deserialize<GetNotificationsResponse>(responseBody);
      System.Diagnostics.Debug.WriteLine($"通知件数: {result?.Notifications.Count}");
      return result?.Notifications ?? new List<NotificationItem>();

    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine("!!!! JSON Deserialize ERROR !!!!");
      System.Diagnostics.Debug.WriteLine(ex.Message);
      throw; // ← ここで投げれば MAUI の output にも出る
    }
  }

  public async Task<bool> UpdateNotificationRead(string accessToken, string id)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
      notification_id = id,
    });
    var content = new StringContent(json, Encoding.UTF8, "application/json");


    using var res = await _http.PostAsync(ApiEndpoints.UpdateNotificationRead, content);
    var responseBody = await res.Content.ReadAsStringAsync();
    System.Diagnostics.Debug.WriteLine("----------UpdateNotificationRead server res--------------");
    System.Diagnostics.Debug.WriteLine(responseBody);
    var response = JsonDocument.Parse(responseBody);
    var success = response.RootElement.GetProperty("success").GetBoolean();
    if (!success)
    {
      System.Diagnostics.Debug.WriteLine("既読設定に失敗しました");
      return false;
    }
    return true;
  }
}
public class GetNotificationsResponse
{
  [JsonPropertyName("success")]
  public bool Success { get; set; }
  [JsonPropertyName("notifications")]
  public List<NotificationItem> Notifications { get; set; }
}