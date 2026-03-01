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
  private readonly IAppLogger _logger;
  private readonly AppStatus _appStatus;

  public ProjectRepository(HttpClient http, IAppLogger logger, AppStatus appStatus)
  {
    _http = http;
    if (_http.BaseAddress == null)
      throw new Exception("HttpClient.BaseAddress is NULL");
    _logger = logger;
    _appStatus = appStatus;
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
    _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.CreateOrUpdateProject, "[API]プロジェクトの新規作成/更新", "プロジェクトの新規作成または更新を行います。", new { project.Id, project.Name, project.Description });
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var res = await _http.PostAsync(ApiEndpoints.CreateOrUpdateProject, content);
    var responseBody = await res.Content.ReadAsStringAsync();

    try
    {
      var root = JsonDocument.Parse(responseBody).RootElement;

      if (root.TryGetProperty("success", out var successProp) && successProp.GetBoolean())
      {
        _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.CreateOrUpdateProject, "[API]プロジェクトの新規作成/更新", "プロジェクトの新規作成または更新に成功しました。", new { project.Id, project.Name, project.Description });
        return new SimpleApiResult
        {
          Success = true,
          Message = "Success Create Project...",
        };
      }

      var message = root.GetProperty("message").GetString();
      _logger.SystemWarning(_appStatus.UserId, ApiEndpoints.CreateOrUpdateProject, "[API]プロジェクトの新規作成/更新", "プロジェクトの新規作成または更新に失敗しました。", new { project.Id, project.Name, project.Description, message });
      return new SimpleApiResult
      {
        Success = false,
        Message = $"サーバーエラー: {message}",
      };
    }
    catch (Exception ex)
    {
      _logger.SystemError(ex, _appStatus.UserId, ApiEndpoints.CreateOrUpdateProject, "[API]プロジェクトの新規作成/更新", new { project.Id, project.Name, project.Description });
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
    _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.GetUserProjects, "[API]プロジェクト取得", "関与するプロジェクトを取得します。");

    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var res = await _http.PostAsync(ApiEndpoints.GetUserProjects, content);
    var responseBody = await res.Content.ReadAsStringAsync();
    var response = JsonDocument.Parse(responseBody);

    var list = response.RootElement
        .GetProperty("projects")
        .EnumerateArray()
        .Select(x => x.GetProperty("project_id").GetString())
        .ToList();
    _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.GetUserProjects, "[API]プロジェクト取得", "関与するプロジェクトを取得しました。", new { list.Count });

    return list;
  }

  public async Task<List<Project>> GetProjectsInfoAsync(string accessToken)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
    });

    _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.GetUserProjects, "[API]プロジェクト取得", "関与するプロジェクトを取得します。");
    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var res = await _http.PostAsync(ApiEndpoints.GetUserProjects, content);
    var responseBody = await res.Content.ReadAsStringAsync();
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
    _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.GetUserProjects, "[API]プロジェクト情報取得", "関与するプロジェクト情報を取得しました。", new { projects.Count });

    return projects;
  }

  public async Task<SimpleApiResult> DeleteProjectAsync(string accessToken, string projectId)
  {
    var json = JsonSerializer.Serialize(new
    {
      token = accessToken,
      project_id = projectId,
    });

    _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.ProjectDelete, "[API]プロジェクトの削除", "プロジェクトを削除します。", new { projectId });
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var res = await _http.PostAsync(ApiEndpoints.ProjectDelete, content);
    if (!res.IsSuccessStatusCode)
    {
      _logger.SystemWarning(_appStatus.UserId, ApiEndpoints.ProjectDelete, "[API]プロジェクトの削除", "プロジェクトの削除中にHTTPエラーが発生しました。", new { res.StatusCode });

      return new SimpleApiResult
      {
        Success = false,
        Message = $"HTTPエラー: {res.StatusCode}",
      };
    }

    var responseBody = await res.Content.ReadAsStringAsync();
    try
    {
      var root = JsonDocument.Parse(responseBody).RootElement;

      if (root.TryGetProperty("success", out var successProp) && successProp.GetBoolean())
      {
        _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.ProjectDelete, "[API]プロジェクトの削除", "プロジェクトの削除に成功しました", new { projectId });
        return new SimpleApiResult
        {
          Success = true,
          Message = "Success Delete Project...",
        };
      }
      var message = root.TryGetProperty("message", out var msgProp)
        ? msgProp.GetString()
        : "不明なエラー";

      _logger.SystemWarning(_appStatus.UserId, ApiEndpoints.ProjectDelete, "[API]プロジェクトの削除", "プロジェクトの削除中にサーバー側でエラーが発生しました。", new { projectId, message });
      System.Diagnostics.Debug.WriteLine($"サーバーエラー: {message}");
      return new SimpleApiResult
      {
        Success = false,
        Message = $"サーバーエラー: {message}",
      };
    }
    catch (Exception ex)
    {
      _logger.SystemError(ex, _appStatus.UserId, ApiEndpoints.ProjectDelete, "[API]プロジェクトの削除", new { projectId });
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
    _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.GetProjectDetails, "[API]プロジェクト詳細の取得", "プロジェクト詳細を取得します。", new { projectId });
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var res = await _http.PostAsync(ApiEndpoints.GetProjectDetails, content);
    if (!res.IsSuccessStatusCode) return null;
    var responseBody = await res.Content.ReadAsStringAsync();

    var options = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true
    };

    var apiResponse =
        JsonSerializer.Deserialize<ProjectDetailsResponse>(responseBody, options);

    if (apiResponse == null || !apiResponse.Success || apiResponse.Data == null)
    {
      _logger.SystemWarning(_appStatus.UserId, ApiEndpoints.GetProjectDetails, "[API]プロジェクト詳細の取得", "サーバーからのレスポンスがありません。", new { projectId });
      return null;
    }


    var d = apiResponse.Data;
    _logger.SystemInfo(_appStatus.UserId, ApiEndpoints.GetProjectDetails, "[API]プロジェクト詳細の取得", "プロジェクト詳細を取得しました。", new { projectId, d.Name });

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
