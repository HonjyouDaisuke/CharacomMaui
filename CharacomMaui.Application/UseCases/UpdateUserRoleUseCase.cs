namespace CharacomMaui.Application.UseCases;

using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

public class UpdateUserRoleUseCase
{
  private readonly IUserRepository _repo;

  public UpdateUserRoleUseCase(IUserRepository repo)
  {
    _repo = repo;
  }

  public async Task<SimpleApiResult> ExecuteAsync(string accessToken, string userId, string userRoleId)
  {
    return await _repo.UpdateUserRoleAsync(accessToken, userId, userRoleId);
  }
}
