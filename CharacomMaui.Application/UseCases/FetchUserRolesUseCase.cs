using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.Sessions;

namespace CharacomMaui.Application.UseCases;

public class FetchUserRolesUseCase
{
  private readonly IUserRolesRepository _repository;
  private readonly UserRolesSession _session;

  public FetchUserRolesUseCase(
    IUserRolesRepository repository,
    UserRolesSession session)
  {
    _repository = repository;
    _session = session;
  }

  public async Task ExecuteAsync(string accessToken)
  {
    var roles = await _repository.FetchUserRolesAsync(accessToken);
    if (roles == null) return;
    _session.SetRoles(roles);
  }
}