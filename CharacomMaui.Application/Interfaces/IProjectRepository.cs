namespace CharacomMaui.Application.Interfaces;

using CharacomMaui.Domain.Entities;

public interface IProjectRepository
{
  Task<SimpleApiResult> CreateOrUpdateProjectAsync(string accessToken, Project project);
  Task<List<string>> GetProjectsAsync(string accessToken);
  Task<List<Project>> GetProjectsInfoAsync(string accessToken);
  Task<SimpleApiResult> DeleteProjectAsync(string accessToken, string projectId);
  Task<ProjectDetails?> GetProjectDetailsAsync(string accessToken, string projectId);
}
