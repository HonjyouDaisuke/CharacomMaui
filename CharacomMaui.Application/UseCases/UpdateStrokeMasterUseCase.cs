namespace CharacomMaui.Application.UseCases;

using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

public class UpdateStrokeMasterUseCase
{
  private readonly IStrokeMasterRepository _repo;

  public UpdateStrokeMasterUseCase(IStrokeMasterRepository repo)
  {
    _repo = repo;
  }

  public async Task<SimpleApiResult> ExecuteAsync(string accessToken)
  {
    return await _repo.UpdateStrokeMasterAsync(accessToken);
  }
}
