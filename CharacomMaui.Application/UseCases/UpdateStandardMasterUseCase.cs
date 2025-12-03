namespace CharacomMaui.Application.UseCases;

using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

public class UpdateStandardMasterUseCase
{
  private readonly IStandardMasterRepository _repo;

  public UpdateStandardMasterUseCase(IStandardMasterRepository repo)
  {
    _repo = repo;
  }

  public async Task<SimpleApiResult> ExecuteAsync(string accessToken)
  {
    return await _repo.UpdateStandardMasterAsync(accessToken);
  }
}
