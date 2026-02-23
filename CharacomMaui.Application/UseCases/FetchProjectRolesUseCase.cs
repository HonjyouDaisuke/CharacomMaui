using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.Sessions;
using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Application.UseCases;

public class FetchProjectRolesUseCase
{
  private readonly IProjectRolesRepository _repository;

  public FetchProjectRolesUseCase(
  IProjectRolesRepository repository)
  {
    _repository = repository;
  }

  public async Task<List<ProjectRole>> ExecuteAsync(string accessToken)
  {
    var roles = await _repository.FetchProjectRolesAsync(accessToken);
    if (roles == null) return new List<ProjectRole>();
    return roles;
  }
}