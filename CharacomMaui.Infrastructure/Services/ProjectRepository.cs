using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Infrastructure.Api;

namespace CharacomMaui.Infrastructure.Services;

public class ProjectRepository : IProjectRepository
{
  private readonly HttpClient _http;

  public ProjectRepository(HttpClient http)
  {
    _http = http;
    if (_http.BaseAddress == null)
      throw new Exception("HttpClient.BaseAddress is NULL");
  }

  public async Task<SimpleApiResult> CreateOrUpdateProjectAsync(string accessToken, Project project)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
      id = project.Id,
      name = project.Name,
      description = project.Description,
      project_folder_id = project.FolderId,
      chara_folder_id = project.CharaFolderId,
    });
    System.Diagnostics.Debug.WriteLine($"更新するデータ：名前{project.Name} 説明{project.Description}");
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var res = await _http.PostAsync(ApiEndpoints.CreateOrUpdateProject, content);
    var responseBody = await res.Content.ReadAsStringAsync();
    System.Diagnostics.Debug.WriteLine("----------create Project server res--------------");
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

  public async Task<List<string>> GetProjectsAsync(string accessToken)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
    });

    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var res = await _http.PostAsync(ApiEndpoints.GetUserProjects, content);
    var responseBody = await res.Content.ReadAsStringAsync();
    System.Diagnostics.Debug.WriteLine("----------User Projects server res--------------");
    System.Diagnostics.Debug.WriteLine(responseBody);
    var response = JsonDocument.Parse(responseBody);

    var list = response.RootElement
        .GetProperty("projects")
        .EnumerateArray()
        .Select(x => x.GetProperty("project_id").GetString())
        .ToList();
    return list;
  }

  public async Task<List<Project>> GetProjectsInfoAsync(string accessToken)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
    });

    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var res = await _http.PostAsync(ApiEndpoints.GetUserProjects, content);
    var responseBody = await res.Content.ReadAsStringAsync();
    System.Diagnostics.Debug.WriteLine("----------User Projects server res--------------");
    System.Diagnostics.Debug.WriteLine(responseBody);
    var response = JsonDocument.Parse(responseBody);

    var options = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true
    };

    var result = JsonSerializer.Deserialize<ProjectInfoResponse>(responseBody, options);

    if (result == null || !result.Success)
      return new List<Project>();

    List<Project> projects = [.. result.Projects.Select(x => new Project
    {
        Id = x.ProjectId,
        Name = x.Name,
        Description = x.Description,
        FolderId = x.FolderId,
        CharaFolderId = x.CharaFolderId,
        CharaCount = x.CharaCount,
        UserCount = x.UserCount
    })];

    return projects;
  }

  public async Task<SimpleApiResult> DeleteProjectAsync(string accessToken, string projectId)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
      project_id = projectId,
    });

    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var res = await _http.PostAsync(ApiEndpoints.ProjectDelete, content);
    if (!res.IsSuccessStatusCode)
    {
      return new SimpleApiResult
      {
        Success = false,
        Message = $"HTTPエラー: {res.StatusCode}",
      };
    }

    var responseBody = await res.Content.ReadAsStringAsync();
    System.Diagnostics.Debug.WriteLine("----------delete Project server res--------------");
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
          Message = "Success Delete Project...",
        };
      }
      var message = root.TryGetProperty("message", out var msgProp)
        ? msgProp.GetString()
        : "不明なエラー";

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
  public async Task<ProjectDetails> GetProjectDetailsAsync(string accessToken, string projectId)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
      project_id = projectId,
    });

    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var res = await _http.PostAsync(ApiEndpoints.GetProjectDetails, content);
    if (!res.IsSuccessStatusCode) return null;
    var responseBody = await res.Content.ReadAsStringAsync();

    System.Diagnostics.Debug.WriteLine("----------GetProjectDetails server res--------------");
    System.Diagnostics.Debug.WriteLine(responseBody);

    var options = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true
    };

    var apiResponse =
        JsonSerializer.Deserialize<ProjectDetailsResponse>(responseBody, options);

    if (apiResponse == null || !apiResponse.Success || apiResponse.Data == null)
      return null;

    var d = apiResponse.Data;

    // Domain / ViewModel 用 Project に変換
    return new ProjectDetails
    {
      Id = d.Id,
      Name = d.Name,
      Description = d.Description,
      ProjectFolderId = d.ProjectFolderId,
      CharaFolderId = d.CharaFolderId,
      CreatedAt = d.CreatedAt,
      CreatedBy = d.CreatedBy,
      UpdatedAt = d.UpdatedAt,
      CharaCount = d.CharaCount,
      Participants = d.Participants
    };
  }
}
