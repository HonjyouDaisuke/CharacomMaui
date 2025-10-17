using CharacomMaui.Application.Interfaces;

namespace CharacomMaui.Application.UseCases;

public class GetBoxConfigUseCase
{
  private readonly IBoxConfigRepository _repository;

  public GetBoxConfigUseCase(IBoxConfigRepository repository)
  {
    _repository = repository;
  }

  public async Task<(string ClientId, string ClientSecret)> ExecuteAsync()
  {
    return await _repository.GetBoxConfigAsync();
  }
}
