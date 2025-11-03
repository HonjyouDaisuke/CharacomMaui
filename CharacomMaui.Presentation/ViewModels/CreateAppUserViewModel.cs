using CharacomMaui.Domain.Entities;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Application.Interfaces;

namespace CharacomMaui.Presentation.ViewModels;

public class CreateAppUserViewModel
{
  private readonly CreateUserUseCase _usecase;
  private readonly IAppTokenStorageService _storage;

  public CreateAppUserViewModel(CreateUserUseCase usecase, IAppTokenStorageService storage)
  {
    _usecase = usecase;
    _storage = storage;
  }

  public async Task<bool> CreateUserAsync(AppUser user)
  {
    var res = await _usecase.ExecuteAsync(user);
    System.Diagnostics.Debug.WriteLine($"-----------------res-----------");
    System.Diagnostics.Debug.WriteLine($"res = {res.ToString()}");
    if (!res.Success) return false;
    await _storage.SaveTokensAsync(res);
    return true;
  }
}
