using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Application.UseCases;

public class GetProjectCharaItemsUseCase
{
  private readonly ICharaDataRepository _repository;

  public GetProjectCharaItemsUseCase(ICharaDataRepository repository)
  {
    _repository = repository;
  }

  public async Task<List<CharaData>> ExecuteAsync(string accessToken, string projectId)
  {
    return await _repository.GetCharaDataAsync(accessToken, projectId);
    //var img = new ImageData(raw, 0, 0);
    //var filtered = _imageProcessing.ApplyFilter(img, "grayscale");
    //await _cloudStorage.UploadFileAsync(folderId, "filtered.png", filtered.RawData);
  }
}