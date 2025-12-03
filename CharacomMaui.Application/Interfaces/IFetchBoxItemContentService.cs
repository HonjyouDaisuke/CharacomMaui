namespace CharacomMaui.Application.Interfaces;

using CharacomMaui.Domain.Entities;

public interface IFetchBoxItemContentService
{
  Task<FetchImageResult> FetchImageData(string accessToken, string fileId);
  Task<FetchImageResult> FetchThumbnailData(string accessToken, string fileId, int width, int height);
}
