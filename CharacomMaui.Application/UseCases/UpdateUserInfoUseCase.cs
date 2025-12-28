namespace CharacomMaui.Application.UseCases;

using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

public class UpdateUserInfoUseCase
{
  private readonly IUserRepository _repo;

  public UpdateUserInfoUseCase(IUserRepository repo)
  {
    _repo = repo;
  }

  public async Task<SimpleApiResult> ExecuteAsync(string accessToken, string userId, string userName, string userEmail, string avatarUrl)
  {
    return await _repo.UpdateUserInfoAsync(accessToken, userId, userName, userEmail, avatarUrl);
  }
}
