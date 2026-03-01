using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Infrastructure.Api;
using CharacomMaui.Infrastructure.Helpers;

namespace CharacomMaui.Infrastructure.Services;

public class UserRolesRepository : IUserRolesRepository
{
  private readonly HttpClient _http;
  private readonly AppStatus _appStatus;
  private readonly IAppLogger _logger;
  public UserRolesRepository(HttpClient http, AppStatus appStatus, IAppLogger logger)
  {
    _http = http;
    if (_http.BaseAddress == null)
      throw new Exception("HttpClient.BaseAddress is NULL");
    _appStatus = appStatus;
    _logger = logger;
  }

  public async Task<List<UserRole>?> FetchUserRolesAsync(string accessToken)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
    });

    _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.GetUserRoles, "[API]ユーザー権限取得", "ユーザー権限を取得します。");
    var content = new StringContent(json, Encoding.UTF8, "application/json");
    try
    {
      using var res = await _http.PostAsync(ApiEndpoints.GetUserRoles, content);
      var responseBody = await res.Content.ReadAsStringAsync();

      var response = JsonDocument.Parse(responseBody);
      var success = response.RootElement.GetProperty("success").GetBoolean();
      if (!success)
      {
        _logger.SystemWarning(_appStatus.UserId, ApiEndpoints.GetUserRoles, "[API]ユーザー権限取得", "ユーザー権限が見つかりませんでした。");
        return null;
      }

      List<UserRole> roles = new List<UserRole>();
      foreach (var item in response.RootElement.GetProperty("roles").EnumerateArray())
      {
        roles.Add(new UserRole
        {
          // TryGetProperty を使うか、?? で null を防ぎます
          Id = item.TryGetProperty("id", out var idEl) ? idEl.GetString() ?? "" : "",
          Name = item.TryGetProperty("name", out var nameEl) ? nameEl.GetString() ?? "" : "",
          Description = item.TryGetProperty("description", out var descEl) ? descEl.GetString() ?? "" : "",

          // // 数値型の場合は GetInt32() の前に型チェックが必要です
          Level = item.TryGetProperty("level", out var lvEl) && lvEl.ValueKind == JsonValueKind.Number
                   ? lvEl.GetInt32()
                   : 0,

          CreatedAt = DateTimeHelper.ParseDateTime(item.GetProperty("created_at")),
          UpdatedAt = DateTimeHelper.ParseDateTime(item.GetProperty("updated_at")),
        });

      }
      _logger.SystemWarning(_appStatus.UserId, ApiEndpoints.GetUserRoles, "[API]ユーザー権限取得", "ユーザー権限を取得しました。");

      return roles;
    }
    catch (Exception ex)
    {
      _logger.SystemError(ex, _appStatus.UserId, ApiEndpoints.GetUserRoles, "[API]ユーザー権限取得");
      return null;
    }
  }


}
