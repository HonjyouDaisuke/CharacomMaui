// CharacomMaui.Presentation/ViewModels/BoxFolderViewModel.cs
using CharacomMaui.Application.UseCases;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace CharacomMaui.Presentation.ViewModels;

public partial class BoxFolderViewModel : ObservableObject
{
    private readonly GetBoxFolderItemsUseCase _useCase;
    private readonly GetBoxImageItemsUseCase _imageUseCase;
    private readonly string RootFolderId = "303046914186";

    public ObservableCollection<BoxItemViewModel> Files { get; } = new();
    // public ObservableCollection<BoxImageItem> Files2 { get; } = new();
    [ObservableProperty]
    public ObservableCollection<BoxImageItemViewModel> files2 = new();

    public BoxFolderViewModel(GetBoxFolderItemsUseCase useCase, GetBoxImageItemsUseCase imageUseCase)
    {
        _useCase = useCase;
        _imageUseCase = imageUseCase;
    }

    public async Task<int> GetFolderItemCountAsync(string accessToken, string folderId)
    {
        var items = await _useCase.GetFolderItemCountAsync(accessToken, folderId);
        return items;
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

    public async Task LoadImageItemsAsync(string accessToken, string folderId, IProgress<double>? progress = null, CancellationToken token = default)
    {
        var items = await _imageUseCase.ExecuteAsync(accessToken, folderId, progress, token);
        files2.Clear();
        foreach (var item in items)
        {
            files2.Add(new BoxImageItemViewModel(item));
        }


    }
}
