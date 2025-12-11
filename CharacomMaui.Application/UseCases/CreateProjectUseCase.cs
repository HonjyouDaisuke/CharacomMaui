namespace CharacomMaui.Application.UseCases;

using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

public class CreateOrUpdateProjectUseCase
{
  private readonly IProjectRepository _repo;

  public CreateOrUpdateProjectUseCase(IProjectRepository repo)
  {
    _repo = repo;
  }

  public async Task<SimpleApiResult> ExecuteAsync(string accessToken, Project project)
  {
    return await _repo.CreateOrUpdateProjectAsync(accessToken, project);
  }
}
