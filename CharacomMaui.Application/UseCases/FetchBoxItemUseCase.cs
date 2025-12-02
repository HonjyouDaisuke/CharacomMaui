namespace CharacomMaui.Application.UseCases;

using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

public class FetchBoxItemUseCase
{
  private readonly IFetchBoxItemContentService _service;

  public FetchBoxItemUseCase(IFetchBoxItemContentService service)
  {
    _service = service;
  }

  public async Task<FetchImageResult> ExecuteAsync(string accessToken, string fileId)
  {
    return await _service.FetchImageData(accessToken, fileId);
  }

  public async Task<FetchImageResult> FetchThumbNailAsync(string accessToken, string fileId, int width, int height)
  {
    return await _service.FetchThumbnailData(accessToken, fileId, width, height);
  }
}
