using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;
using Org.BouncyCastle.Asn1.Misc;
using Org.BouncyCastle.Math.EC.Rfc7748;

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

  public async Task<AppUser> GetUserInfoAsync(string accessToken)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
    });

    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var res = await _http.PostAsync("get_user_info.php", content);
    var responseBody = await res.Content.ReadAsStringAsync();
    System.Diagnostics.Debug.WriteLine("----------User Info server res--------------");
    System.Diagnostics.Debug.WriteLine($"AccessToken = {accessToken}  ");
    System.Diagnostics.Debug.WriteLine(responseBody);

    var response = JsonDocument.Parse(responseBody).RootElement;
    var success = response.GetProperty("success").GetBoolean();
    if (!success)
    {
      System.Diagnostics.Debug.WriteLine("success = Falseだよ");
      return null;
    }

    return new AppUser
    {
      Id = response.GetProperty("id").GetString(),
      Name = response.GetProperty("name").GetString(),
      Email = response.GetProperty("email").GetString(),
      PictureUrl = response.GetProperty("picture_url").GetString(),
      RoleId = response.GetProperty("role_id").GetString(),
    };
  }

  public async Task<string> GetAvatarImgStringAsync(string accessToken)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
    });

    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var res = await _http.PostAsync("get_user_avatar.php", content);
    var responseBody = await res.Content.ReadAsStringAsync();
    var response = JsonDocument.Parse(responseBody).RootElement;
    System.Diagnostics.Debug.WriteLine("----------User Avatar server res--------------");
    System.Diagnostics.Debug.WriteLine($"AccessToken = {accessToken}  ");
    System.Diagnostics.Debug.WriteLine(responseBody);
    var success = response.GetProperty("success").GetBoolean();

    if (!success)
    {
      return null;
    }

    return response.GetProperty("avatar_base64").GetString();
  }
}
