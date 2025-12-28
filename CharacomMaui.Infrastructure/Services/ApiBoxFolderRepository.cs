using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Infrastructure.Api;

namespace CharacomMaui.Infrastructure.Services;

public class ApiBoxFolderRepository : IBoxFolderRepository
{
  private readonly HttpClient _http;

  public ApiBoxFolderRepository(HttpClient http)
  {
    _http = http;
    if (_http.BaseAddress == null)
      throw new InvalidOperationException("HttpClient.BaseAddress is NULL");
  }

  public async Task<List<BoxItem>> GetFolderItemsAsync(string accessToken, string? folderId = null)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
      folder_id = folderId,
    });
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var res = await _http.PostAsync(ApiEndpoints.GetFolderItems, content);
    var responseBody = await res.Content.ReadAsStringAsync();

    System.Diagnostics.Debug.WriteLine("----------server res--------------");
    System.Diagnostics.Debug.WriteLine(responseBody);
    try
    {
      var root = JsonDocument.Parse(responseBody).RootElement;

      if (root.TryGetProperty("success", out var successProp) && successProp.GetBoolean())
      {
        JsonElement folderItemsElement = root.GetProperty("folderItems");

        // folderItems を List<BoxItem> に変換
        var folderItems = JsonSerializer.Deserialize<List<BoxItem>>(folderItemsElement.GetRawText());

        foreach (var item in folderItems)
        {
          Console.WriteLine($"{item.Id}: {item.Name} ({item.Type})");
        }
        return folderItems;
      }


      var message = root.GetProperty("message").GetString();
      System.Diagnostics.Debug.WriteLine($"サーバーエラー: {message}");
      return null;
    }
    catch
    {
      return null;
    }
  }
}
