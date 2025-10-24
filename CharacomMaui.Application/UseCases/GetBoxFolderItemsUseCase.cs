// CharacomMaui.Application/UseCases/GetBoxFolderItemsUseCase.cs
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Application.UseCases;

public class GetBoxFolderItemsUseCase
{
  private readonly IBoxApiRepository _boxApiRepository;

  public GetBoxFolderItemsUseCase(IBoxApiRepository boxApiRepository)
  {
    _boxApiRepository = boxApiRepository;
  }

  public async Task<List<BoxItem>> ExecuteAsync(string accessToken, string folderId)
  {
    return await _boxApiRepository.GetFolderItemsAsync(accessToken, folderId);
  }

  public async Task<int> GetFolderItemCountAsync(string accessToken, string folderId)
  {
    return await _boxApiRepository.GetFolderItemCountAsync(accessToken, folderId);
  }
}
