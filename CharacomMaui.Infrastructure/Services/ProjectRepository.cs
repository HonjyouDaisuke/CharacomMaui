using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Infrastructure.Services;

public class ProjectRepository : IProjectRepository
{
  private readonly HttpClient _http;

  public ProjectRepository(HttpClient http)
  {
    _http = http;
    _http.BaseAddress = new Uri("http://localhost:8888/CharacomMauiHP/api/");
  }

  public async Task<SimpleApiResult> CreateProjectAsync(string accessToken, Project project)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
      name = project.Name,
      description = project.Description,
      project_folder_id = project.FolderId,
      chara_folder_id = project.CharaFolderId,
    });

    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var res = await _http.PostAsync("create_project.php", content);
    var responseBody = await res.Content.ReadAsStringAsync();
    System.Diagnostics.Debug.WriteLine("----------create Project server res--------------");
    System.Diagnostics.Debug.WriteLine($"AccessToken = {accessToken}  ");
    System.Diagnostics.Debug.WriteLine(responseBody);
    try
    {
      var root = JsonDocument.Parse(responseBody).RootElement;

      if (root.TryGetProperty("success", out var successProp) && successProp.GetBoolean())
      {
        System.Diagnostics.Debug.WriteLine("こっち");
        return new SimpleApiResult
        {
          Success = true,
          Message = "Success Create Project...",
        };
      }


      var message = root.GetProperty("message").GetString();
      System.Diagnostics.Debug.WriteLine($"サーバーエラー: {message}");
      return new SimpleApiResult
      {
        Success = false,
        Message = $"サーバーエラー: {message}",
      };
    }
    catch (Exception ex)
    {
      return new SimpleApiResult
      {
        Success = false,
        Message = $"想定外のエラーが発生しました。。。{ex.Message}",
      };
    }
  }
}
