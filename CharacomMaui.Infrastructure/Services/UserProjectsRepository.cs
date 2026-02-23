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

public class UserProjectsRepository : IUserProjectsRepository
{
  private readonly HttpClient _http;
  public UserProjectsRepository(HttpClient http)
  {
    _http = http;
    if (_http.BaseAddress == null)
      throw new Exception("HttpClient.BaseAddress is NULL");
  }

  public async Task<SimpleApiResult> InviteToProjectAsync(string accessToken, string projectId, string toUserId, string toRoleId)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
      project_id = projectId,
      to_user_id = toUserId,
      to_role_id = toRoleId,
    });
    using var content = new StringContent(json, Encoding.UTF8, "application/json");
    try
    {
      using var res = await _http.PostAsync(ApiEndpoints.InviteToProject, content);
      var responseBody = await res.Content.ReadAsStringAsync();
      System.Diagnostics.Debug.WriteLine("----------Invite To Project server res--------------");
      System.Diagnostics.Debug.WriteLine(responseBody);
      using var response = JsonDocument.Parse(responseBody);
      var success = response.RootElement.GetProperty("success").GetBoolean();
      if (!success)
      {
        var message = response.RootElement.GetProperty("message").GetString() ?? string.Empty; ;
        System.Diagnostics.Debug.WriteLine($"プロジェクトへの招待に失敗しました: {message}");
        return new SimpleApiResult(false, message);
      }
      System.Diagnostics.Debug.WriteLine("プロジェクトへの招待に成功しました。");
      return new SimpleApiResult(true, "招待に成功しました。");
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"プロジェクトへの招待中にエラーが発生しました: {ex.Message}");
      return new SimpleApiResult(false, "プロジェクトへの招待中にエラーが発生しました。");
    }
  }

}