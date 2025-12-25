using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Infrastructure.Services;

public class AvatarRepository : IAvatarRepository
{
  private readonly HttpClient _http;

  public AvatarRepository(HttpClient http)
  {
    _http = http;
    _http.BaseAddress = new Uri("http://localhost:8888/CharacomMauiHP/api/");
  }
  public async Task<List<string>> GetAvatarsUrl(string accessToken)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
    });

    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var res = await _http.PostAsync("get_avatar_images.php", content);
    var responseBody = await res.Content.ReadAsStringAsync();
    System.Diagnostics.Debug.WriteLine("----------Avatar res--------------");
    System.Diagnostics.Debug.WriteLine(responseBody);
    var response = JsonDocument.Parse(responseBody);

    var list = response.RootElement
        .GetProperty("avatars")
        .EnumerateArray()
        .Select(x => x.GetProperty("url").GetString()!)
        .Where(x => !string.IsNullOrEmpty(x))
        .ToList();

    return list;
  }


}