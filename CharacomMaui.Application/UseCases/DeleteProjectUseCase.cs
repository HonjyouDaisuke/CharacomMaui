namespace CharacomMaui.Application.UseCases;

using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

public class DeleteProjectUseCase
{
  private readonly IProjectRepository _repo;

  public DeleteProjectUseCase(IProjectRepository repo)
  {
    _repo = repo;
  }

  public async Task<SimpleApiResult> ExecuteAsync(string accessToken, string projectId)
  {
    return await _repo.DeleteProjectAsync(accessToken, projectId);
  }
}
