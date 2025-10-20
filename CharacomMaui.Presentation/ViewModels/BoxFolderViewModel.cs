// CharacomMaui.Presentation/ViewModels/BoxFolderViewModel.cs
using CharacomMaui.Application.UseCases;
using System.Collections.ObjectModel;

namespace CharacomMaui.Presentation.ViewModels;

public class BoxFolderViewModel
{
  private readonly GetBoxFolderItemsUseCase _useCase;
  private readonly string RootFolderId = "303046914186";

  public ObservableCollection<BoxItemViewModel> Files { get; } = new();

  public BoxFolderViewModel(GetBoxFolderItemsUseCase useCase)
  {
    _useCase = useCase;
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
}
