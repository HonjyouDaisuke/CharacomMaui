namespace CharacomMaui.Application.Interfaces;

using CharacomMaui.Domain.Entities;

public interface IProjectRepository
{
  Task<SimpleApiResult> CreateProjectAsync(string accessToken, Project project);
}
