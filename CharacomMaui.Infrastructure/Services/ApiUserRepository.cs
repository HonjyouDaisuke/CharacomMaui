using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Infrastructure.Services;

public class ApiUserRepository : IUserRepository
{
  private readonly HttpClient _http;

  public ApiUserRepository(HttpClient http)
  {
    _http = http;
    _http.BaseAddress = new Uri("http://localhost:8888/CharacomMauiHP/api/");
  }

  public async Task<AppTokenResult> CreateUserAsync(AppUser user)
  {
    var json = JsonSerializer.Serialize(new
    {
      id = user.Id,
      name = user.Name,
      email = user.Email,
      box_user_id = user.Id,
      picture_url = user.PictureUrl,
      box_access_token = user.BoxAccessToken,
      box_refresh_token = user.BoxRefreshToken
    });

    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var res = await _http.PostAsync("create_user.php", content);
    var responseBody = await res.Content.ReadAsStringAsync();
    System.Diagnostics.Debug.WriteLine("----------server res--------------");
    System.Diagnostics.Debug.WriteLine(responseBody);
    try
    {
      var root = JsonDocument.Parse(responseBody).RootElement;

      if (root.TryGetProperty("success", out var successProp) && successProp.GetBoolean())
      {
        System.Diagnostics.Debug.WriteLine("こっち");
        return new AppTokenResult
        {
          Success = true,
          AccessToken = root.GetProperty("access_token").GetString(),
          RefreshToken = root.GetProperty("refresh_token").GetString(),
          ExpiresAt = (int)root.GetProperty("expire_at").GetInt64(),
          Message = "Success get AppToken...",
        };
      }


      var message = root.GetProperty("message").GetString();
      System.Diagnostics.Debug.WriteLine($"サーバーエラー: {message}");
      return new AppTokenResult
      {
        Success = false,
        Message = $"サーバーエラー: {message}",
      };
    }
    catch
    {
      return new AppTokenResult
      {
        Success = false,
        Message = $"想定外のエラーが発生しました。。。",
      };
    }
  }
}
