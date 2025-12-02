namespace CharacomMaui.Application.UseCases;

using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

public class GetStrokeFileIdUseCase
{
  private readonly IStrokeMasterRepository _repo;

  public GetStrokeFileIdUseCase(IStrokeMasterRepository repo)
  {
    _repo = repo;
  }

  public async Task<string> ExecuteAsync(string accessToken, string charaName)
  {
    return await _repo.GetStrokeFileIdAsync(accessToken, charaName);
  }
}
