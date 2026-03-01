using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Infrastructure.Api;


namespace CharacomMaui.Infrastructure.Services;

public class ApiUserRepository : IUserRepository
{
  private readonly HttpClient _http;
  private readonly AppStatus _appStatus;
  private readonly IAppLogger _logger;

  public ApiUserRepository(HttpClient http, AppStatus appStatus, IAppLogger logger)
  {
    _http = http;
    if (_http.BaseAddress == null)
      throw new Exception("HttpClient.BaseAddress is NULL");
    _appStatus = appStatus;
    _logger = logger;
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
    _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.CreateUser, "[API]ユーザー作成", "ユーザーを作成します。");
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var res = await _http.PostAsync(ApiEndpoints.CreateUser, content);
    var responseBody = await res.Content.ReadAsStringAsync();

    try
    {
      var root = JsonDocument.Parse(responseBody).RootElement;

      if (root.TryGetProperty("success", out var successProp) && successProp.GetBoolean())
      {
        _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.CreateUser, "[API]ユーザー作成", "ユーザー情報作成しに成功しました。");

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
      _logger.SystemWarning(_appStatus.UserId, ApiEndpoints.CreateUser, "[API]ユーザー作成", "ユーザー情報作成時にサーバーでエラーが発生しました。", new { message });
      return new AppTokenResult
      {
        Success = false,
        Message = $"サーバーエラー: {message}",
      };
    }
    catch (Exception ex)
    {
      _logger.SystemError(ex, _appStatus.UserId, ApiEndpoints.CreateUser, "[API]ユーザー作成");

      return new AppTokenResult
      {
        Success = false,
        Message = $"想定外のエラーが発生しました。。。",
      };
    }
  }

  public async Task<List<AppUser>> GetUserListAsync(string accessToken)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
    });
    _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.GetUserList, "[API]ユーザー一覧", "ユーザー一覧を取得します。");
    var content = new StringContent(json, Encoding.UTF8, "application/json");
    try
    {
      var res = await _http.PostAsync(ApiEndpoints.GetUserList, content);
      var responseBody = await res.Content.ReadAsStringAsync();
      var response = JsonDocument.Parse(responseBody).RootElement;
      var success = response.GetProperty("success").GetBoolean();
      if (!success)
      {
        _logger.SystemWarning(_appStatus.UserId, ApiEndpoints.GetUserList, "[API]ユーザー一覧", "ユーザー一覧の取得に失敗しました。");
        return null;
      }

      var users = new List<AppUser>();
      foreach (var item in response.GetProperty("users").EnumerateArray())
      {
        users.Add(new AppUser
        {
          Id = item.GetProperty("id").GetString(),
          Name = item.GetProperty("name").GetString(),
          Email = item.GetProperty("email").GetString(),
          PictureUrl = item.GetProperty("picture_url").GetString(),
          RoleId = item.GetProperty("role_id").GetString(),
        });
      }
      _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.GetUserList, "[API]ユーザー一覧", "ユーザー一覧を取得しました。");

      return users;
    }
    catch (Exception ex)
    {
      _logger.SystemError(ex, _appStatus.UserId, ApiEndpoints.GetUserList, "[API]ユーザー一覧");
      return new List<AppUser>();
    }

  }

  public async Task<AppUser> GetUserInfoAsync(string accessToken)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
    });
    _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.GetUserInfo, "[API]ユーザー情報取得", "ユーザー情報を取得します。");
    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var res = await _http.PostAsync(ApiEndpoints.GetUserInfo, content);
    var responseBody = await res.Content.ReadAsStringAsync();

    var response = JsonDocument.Parse(responseBody).RootElement;
    var success = response.GetProperty("success").GetBoolean();
    if (!success)
    {
      _logger.SystemWarning(_appStatus.UserId, ApiEndpoints.GetUserInfo, "[API]ユーザー情報取得", "ユーザー情報取得に失敗しました。");
      return null;
    }
    _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.GetUserInfo, "[API]ユーザー情報取得", "ユーザー情報を取得しました。",
      new
      {
        Name = response.GetProperty("name").GetString(),
        RoleId = response.GetProperty("role_id").GetString(),
      });
    return new AppUser
    {
      Id = response.GetProperty("id").GetString(),
      Name = response.GetProperty("name").GetString(),
      Email = response.GetProperty("email").GetString(),
      PictureUrl = response.GetProperty("picture_url").GetString(),
      RoleId = response.GetProperty("role_id").GetString(),
    };
  }
  public async Task<SimpleApiResult> UpdateUserInfoAsync(string accessToken, string userId, string userName, string userEmail, string avatarUrl)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
      user_name = userName,
      user_email = userEmail,
      avatar_url = avatarUrl,
    });
    _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.UpdateUserInfo, "[API]ユーザー情報更新", "ユーザー情報を更新します。");

    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var res = await _http.PostAsync(ApiEndpoints.UpdateUserInfo, content);
    var responseBody = await res.Content.ReadAsStringAsync();

    try
    {
      var root = JsonDocument.Parse(responseBody).RootElement;

      if (root.TryGetProperty("success", out var successProp) && successProp.GetBoolean())
      {
        _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.UpdateUserInfo, "[API]ユーザー情報更新", "ユーザー情報を更新しました。", new { userName });
        return new SimpleApiResult
        {
          Success = true,
          Message = "Success Update User Info...",
        };
      }


      var message = root.GetProperty("message").GetString();
      _logger.SystemWarning(_appStatus.UserId, ApiEndpoints.UpdateUserInfo, "[API]ユーザー情報更新", "ユーザー情報更新に失敗しました。", new { userName, message });

      return new SimpleApiResult
      {
        Success = false,
        Message = $"サーバーエラー: {message}",
      };
    }
    catch (Exception ex)
    {
      _logger.SystemError(ex, _appStatus.UserId, ApiEndpoints.UpdateUserInfo, "[API]ユーザー情報更新");
      return new SimpleApiResult
      {
        Success = false,
        Message = $"想定外のエラーが発生しました。。。{ex.Message}",
      };
    }
  }

  public async Task<SimpleApiResult> UpdateUserRoleAsync(string accessToken, string userId, string userRoleId)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
      user_id = userId,
      user_role_id = userRoleId,
    });
    _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.UpdateUserRole, "[API]ユーザー権限の更新", "ユーザー権限を更新します。");

    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var res = await _http.PostAsync(ApiEndpoints.UpdateUserRole, content);
    var responseBody = await res.Content.ReadAsStringAsync();

    try
    {
      var root = JsonDocument.Parse(responseBody).RootElement;

      if (root.TryGetProperty("success", out var successProp) && successProp.GetBoolean())
      {
        _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.UpdateUserRole, "[API]ユーザー権限の更新", "権限の更新に成功しました。", new { userRoleId });
        return new SimpleApiResult
        {
          Success = true,
          Message = "Success Update User Role...",
        };
      }

      var message = root.GetProperty("message").GetString();

      _logger.SystemWarning(_appStatus.UserId, ApiEndpoints.UpdateUserRole, "[API]ユーザー権限の更新", "ユーザー権限の更新に失敗しました。", new { userRoleId, message });

      return new SimpleApiResult
      {
        Success = false,
        Message = $"サーバーエラー: {message}",
      };
    }
    catch (Exception ex)
    {
      _logger.SystemError(ex, _appStatus.UserId, ApiEndpoints.UpdateUserRole, "[API]ユーザー権限の更新");
      return new SimpleApiResult
      {
        Success = false,
        Message = $"想定外のエラーが発生しました。。。{ex.Message}",
      };
    }
  }
}
