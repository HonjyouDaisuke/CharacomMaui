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

  public async Task<List<BoxImageItem>> ExecuteAsync(string accessToken, string folderId, IProgress<double>? progress, CancellationToken token)
  {
    var items = await _boxApiRepository.GetFolderItemsAsync(accessToken, folderId);
    int total = items.Count;
    int count = 0;
    var result = new List<BoxImageItem>();

    foreach (var item in items)
    {
      if (item.Type == "file" &&
          (item.Name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
           item.Name.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)))
      {
        var bytes = await _boxApiRepository.DownloadFileAsync(accessToken, item.Id);
        result.Add(new BoxImageItem
        {
          Id = item.Id,
          Name = item.Name,
          Type = item.Type,
          Image = bytes
        });
        count++;
        progress?.Report((double)count / total);
      }
    }

    return result;
  }
}
