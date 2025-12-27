using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Application.UseCases;

public class GetAvatarsUrlUseCase
{
  private readonly IAvatarRepository _repository;

  public GetAvatarsUrlUseCase(IAvatarRepository repository)
  {
    _repository = repository;
  }

  public async Task<List<string>> ExecuteAsync(string accessToken)
  {
    return await _repository.GetAvatarsUrl(accessToken);
  }
}