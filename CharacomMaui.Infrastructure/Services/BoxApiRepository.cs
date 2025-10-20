// CharacomMaui.Infrastructure/Repositories/BoxApiRepository.cs
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace CharacomMaui.Infrastructure.Services;

public class BoxApiRepository : IBoxApiRepository
{
  private readonly HttpClient _httpClient;

  public BoxApiRepository(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<List<BoxItem>> GetFolderItemsAsync(string accessToken, string folderId)
  {
    _httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", accessToken);

    var url = $"https://api.box.com/2.0/folders/{folderId}/items?fields=id,name,size,type,modified_at";
    var response = await _httpClient.GetAsync(url);
    var json = await response.Content.ReadAsStringAsync();
    if (!response.IsSuccessStatusCode)
      throw new Exception($"Box API error: {(int)response.StatusCode} {response.ReasonPhrase}");

    var items = new List<BoxItem>();

    using (var doc = JsonDocument.Parse(json))
    {
      var root = doc.RootElement;
      if (!root.TryGetProperty("entries", out var entries))
        throw new Exception($"entries が見つかりません。レスポンス: {json}");


      foreach (var entry in entries.EnumerateArray())
      {
        items.Add(new BoxItem
        {
          Id = entry.GetProperty("id").GetString() ?? "",
          Name = entry.GetProperty("name").GetString() ?? "",
          Type = entry.GetProperty("type").GetString() ?? ""
        });
      }
    }

    return items;
  }

  public async Task<List<BoxImageItem>> GetJpgImagesAsync(string accessToken, string folderId)
  {
    var items = await GetFolderItemsAsync(accessToken, folderId);
    var result = new List<BoxImageItem>();

    foreach (var item in items)
    {
      if (item.Type == "file" &&
          (item.Name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
           item.Name.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)))
      {
        var bytes = await DownloadFileAsync(accessToken, item.Id);
        result.Add(new BoxImageItem
        {
          Id = item.Id,
          Name = item.Name,
          Type = item.Type,
          Image = bytes
        });
      }
    }

    return result;
  }

  private async Task<byte[]> DownloadFileAsync(string accessToken, string fileId)
  {
    _httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", accessToken);

    var url = $"https://api.box.com/2.0/files/{fileId}/content";
    using var response = await _httpClient.GetAsync(url);
    response.EnsureSuccessStatusCode();

    return await response.Content.ReadAsByteArrayAsync();
  }
}
