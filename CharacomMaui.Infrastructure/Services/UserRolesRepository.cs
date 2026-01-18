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
  public UserRolesRepository(HttpClient http)
  {
    _http = http;
    if (_http.BaseAddress == null)
      throw new Exception("HttpClient.BaseAddress is NULL");
  }

  public async Task<List<UserRole>?> FetchUserRolesAsync(string accessToken)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
    });
    var content = new StringContent(json, Encoding.UTF8, "application/json");
    try
    {
      using var res = await _http.PostAsync(ApiEndpoints.GetUserRoles, content);
      var responseBody = await res.Content.ReadAsStringAsync();
      System.Diagnostics.Debug.WriteLine("----------User Roles server res--------------");
      System.Diagnostics.Debug.WriteLine(responseBody);
      var response = JsonDocument.Parse(responseBody);
      var success = response.RootElement.GetProperty("success").GetBoolean();
      if (!success)
      {
        System.Diagnostics.Debug.WriteLine("UserRoleが見つかりませんでした。");
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
      return roles;
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"想定外のエラー: {ex.Message}");
      return null;
    }
  }


}
