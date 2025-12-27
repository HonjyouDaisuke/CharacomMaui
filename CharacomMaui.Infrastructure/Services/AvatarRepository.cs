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
using CharacomMaui.Infrastructure.Api;

namespace CharacomMaui.Infrastructure.Services;

public class AvatarRepository : IAvatarRepository
{
  private readonly HttpClient _http;

  public AvatarRepository(HttpClient http)
  {
    _http = http;
    if (_http.BaseAddress == null)
      throw new Exception("HttpClient.BaseAddress is NULL");
  }
  public async Task<List<string>> GetAvatarsUrl(string accessToken)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
    });

    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var res = await _http.PostAsync(ApiEndpoints.GetAvatarsImages, content);

    if (!res.IsSuccessStatusCode)
    {
      System.Diagnostics.Debug.WriteLine($"HTTP Error: {res.StatusCode}");
      return new List<string>();
    }

    var responseBody = await res.Content.ReadAsStringAsync();

    if (string.IsNullOrEmpty(responseBody))
    {
      System.Diagnostics.Debug.WriteLine("Empty response body");
      return new List<string>();
    }

    System.Diagnostics.Debug.WriteLine("----------Avatar res--------------");
    System.Diagnostics.Debug.WriteLine(responseBody);

    JsonDocument response;
    try
    {
      response = JsonDocument.Parse(responseBody);
    }
    catch (JsonException ex)
    {
      System.Diagnostics.Debug.WriteLine($"JSON Parse Error: {ex.Message}");
      return new List<string>();
    }

    if (!response.RootElement.TryGetProperty("avatars", out var avatarsElement))
    {
      System.Diagnostics.Debug.WriteLine("Missing 'avatars' property in response");
      return new List<string>();
    }

    var list = avatarsElement
        .EnumerateArray()
        .Select(x => x.TryGetProperty("url", out var url) ? url.GetString() : null)
        .Where(x => !string.IsNullOrEmpty(x))
        .Select(x => x!)
        .ToList();

    return list;
  }


}