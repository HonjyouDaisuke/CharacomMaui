namespace CharacomMaui.Application.UseCases;

using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

public class UpdateCharaSelectedUseCase
{
  private readonly ICharaDataRepository _repo;

  public UpdateCharaSelectedUseCase(ICharaDataRepository repo)
  {
    _repo = repo;
  }

  public async Task<SimpleApiResult> ExecuteAsync(string accessToken, string charaId, bool isSelected)
  {
    return await _repo.UpdateSelectdCharaAsync(accessToken, charaId, isSelected);
  }
}
