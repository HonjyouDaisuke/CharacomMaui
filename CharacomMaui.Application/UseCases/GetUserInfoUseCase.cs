using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Application.UseCases;

public class GetUserInfoUseCase
{
  private readonly IUserRepository _repository;

  public GetUserInfoUseCase(IUserRepository repository)
  {
    _repository = repository;
  }

  public async Task<AppUser> GetUserInfoAsync(string accessToken)
  {
    return await _repository.GetUserInfoAsync(accessToken);
  }
}