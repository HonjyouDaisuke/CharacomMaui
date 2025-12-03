// CharacomMaui.Application/UseCases/GetBoxFolderItemsUseCase.cs
using System.Diagnostics;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Application.UseCases;

public class GetBoxFolderItemsUseCase
{
  private readonly IBoxFolderRepository _repository;

  public GetBoxFolderItemsUseCase(IBoxFolderRepository repository)
  {
    _repository = repository;
  }

  public async Task<List<BoxItem>> ExecuteAsync(string accessToken, string? folderId)
  {
    Debug.WriteLine($"[usecase]AccessToken = {accessToken}");
    Debug.WriteLine($"[usecase]FolderId = {folderId}");
    return await _repository.GetFolderItemsAsync(accessToken, folderId);
  }

  public async Task<int> GetFolderItemCountAsync(string accessToken, string folderId)
  {
    // TODO:作り変えが必要
    return 0;  //await _repository.GetFolderItemCountAsync(accessToken, folderId);
  }
}
