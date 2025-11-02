// CharacomMaui.Presentation/ViewModels/BoxFolderViewModel.cs
using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
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
    Console.WriteLine("Start LoadImage...");
    var items = await _imageUseCase.ExecuteAsync(accessToken, folderId, progress, token);

    Console.WriteLine($"Image item 個数:{items.Count}...");
    var newFiles = items
        .Select(item => new BoxImageItemViewModel(item))
        .ToList();
    Console.WriteLine($"Image newFiles 個数:{newFiles.Count}...");

    // UIスレッドに戻り、コレクションを一括で更新する
    await MainThread.InvokeOnMainThreadAsync(() =>
    {
      int count = 0;
      Files2.Clear();
      foreach (var item in newFiles)
      {
        Console.WriteLine($"No,{count} file Start");
        Files2.Add(item);
        Console.WriteLine($"No,{count} file End");
        count++;

      }
    });
    Console.WriteLine($"File2 個数:{Files2.Count}...");

    // ⭐ コレクションの更新がUIスレッドで完了した後、
    // ⭐ バックグラウンドで画像のロードを順次/並行して開始する

    // UIスレッドをブロックしないように、このメソッド自体は完了させる
    // 別の Task としてロード処理を起動
    _ = Task.Run(async () =>
    {
      // 処理が完了するまでポップアップを閉じるのを遅延させたい場合は、
      // この処理を HomePage.xaml.cs の try ブロック内に移動する必要があります。

      // すべてのファイルのロードを開始（メモリ消費に注意）
      foreach (var fileViewModel in newFiles)
      {
        if (token.IsCancellationRequested) return;
        // 処理が速すぎる場合は、フリーズを防ぐために間にわずかな遅延を入れることも有効
        // await Task.Delay(10); 
        await fileViewModel.LoadImageAsync();
      }
    });

  }
}
