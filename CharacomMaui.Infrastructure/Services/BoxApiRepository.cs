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
}
