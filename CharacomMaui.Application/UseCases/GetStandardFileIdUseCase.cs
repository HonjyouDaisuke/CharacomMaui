namespace CharacomMaui.Application.UseCases;

using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

public class GetStandardFileIdUseCase
{
  private readonly IStandardMasterRepository _repo;

  public GetStandardFileIdUseCase(IStandardMasterRepository repo)
  {
    _repo = repo;
  }

  public async Task<string> ExecuteAsync(string accessToken, string charaName)
  {
    return await _repo.GetStandardFileIdAsync(accessToken, charaName);
  }
}
