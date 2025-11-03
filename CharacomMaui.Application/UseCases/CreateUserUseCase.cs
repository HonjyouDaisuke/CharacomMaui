namespace CharacomMaui.Application.UseCases;

using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

public class CreateUserUseCase
{
  private readonly IUserRepository _repo;

  public CreateUserUseCase(IUserRepository repo)
  {
    _repo = repo;
  }

  public async Task<AppTokenResult> ExecuteAsync(AppUser user)
  {
    return await _repo.CreateUserAsync(user);
  }
}
