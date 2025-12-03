using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Application.UseCases;

public class GetUserProjectsUseCase
{
  private readonly IProjectRepository _repository;

  public GetUserProjectsUseCase(IProjectRepository repository)
  {
    _repository = repository;
  }

  public async Task<List<string>> ExecuteAsync(string accessToken)
  {
    return await _repository.GetProjectsAsync(accessToken);
    //var img = new ImageData(raw, 0, 0);
    //var filtered = _imageProcessing.ApplyFilter(img, "grayscale");
    //await _cloudStorage.UploadFileAsync(folderId, "filtered.png", filtered.RawData);
  }

  public async Task<List<Project>> GetProjectsInfoAsync(string accessToken)
  {
    return await _repository.GetProjectsInfoAsync(accessToken);
  }
}