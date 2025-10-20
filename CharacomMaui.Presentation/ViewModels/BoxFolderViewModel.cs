// CharacomMaui.Presentation/ViewModels/BoxFolderViewModel.cs
using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace CharacomMaui.Presentation.ViewModels;

public class BoxFolderViewModel
{
  private readonly GetBoxFolderItemsUseCase _useCase;
  private readonly GetBoxImageItemsUseCase _imageUseCase;
  private readonly string RootFolderId = "303046914186";

  public ObservableCollection<BoxItemViewModel> Files { get; } = new();
  public ObservableCollection<BoxImageItem> Files2 { get; } = new();

  public BoxFolderViewModel(GetBoxFolderItemsUseCase useCase, GetBoxImageItemsUseCase imageUseCase)
  {
    _useCase = useCase;
    _imageUseCase = imageUseCase;
  }

  public async Task LoadFolderItemsAsync(string accessToken)
  {
    var items = await _useCase.ExecuteAsync(accessToken, RootFolderId);

    Files.Clear();
    foreach (var item in items)
    {
      Files.Add(new BoxItemViewModel(item));
    }
  }

  public async Task LoadImageItemsAsync(string accessToken, string folderId)
  {
    var items = await _imageUseCase.ExecuteAsync(accessToken, folderId);
    Files2.Clear();
    foreach (var item in items)
    {
      Files2.Add(item);
    }
  }
}
