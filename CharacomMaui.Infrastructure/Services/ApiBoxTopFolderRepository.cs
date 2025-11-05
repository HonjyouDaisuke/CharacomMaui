using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Infrastructure.Services;

public class ApiBoxTopFolderRepository : IBoxTopFolderRepository
{
  private readonly HttpClient _http;

  public ApiBoxTopFolderRepository(HttpClient http)
  {
    _http = http;
    _http.BaseAddress = new Uri("http://localhost:8888/CharacomMauiHP/api/");
  }

  public async Task<List<BoxItem>> GetTopFolders(string accessToken)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
    });
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var res = await _http.PostAsync("get_top_folders.php", content);
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
