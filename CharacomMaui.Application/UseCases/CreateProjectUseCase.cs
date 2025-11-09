namespace CharacomMaui.Application.UseCases;

using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

public class CreateProjectUseCase
{
  private readonly IProjectRepository _repo;

  public CreateProjectUseCase(IProjectRepository repo)
  {
    _repo = repo;
  }

  public async Task<SimpleApiResult> ExecuteAsync(string accessToken, Project project)
  {
    return await _repo.CreateProjectAsync(accessToken, project);
  }
}
