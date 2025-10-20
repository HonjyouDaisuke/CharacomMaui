// CharacomMaui.Application/UseCases/GetBoxFolderItemsUseCase.cs
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Application.UseCases;

public class GetBoxImageItemsUseCase
{
  private readonly IBoxApiRepository _boxApiRepository;

  public GetBoxImageItemsUseCase(IBoxApiRepository boxApiRepository)
  {
    _boxApiRepository = boxApiRepository;
  }

  public async Task<List<BoxImageItem>> ExecuteAsync(string accessToken, string folderId)
  {
    return await _boxApiRepository.GetJpgImagesAsync(accessToken, folderId);
  }
}
