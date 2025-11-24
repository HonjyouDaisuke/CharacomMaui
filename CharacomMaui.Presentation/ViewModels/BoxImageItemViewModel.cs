// CharacomMaui.Presentation/ViewModels/BoxItemViewModel.cs
using CharacomMaui.Domain.Entities;
using SkiaSharp;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CharacomMaui.Presentation.ViewModels;

public partial class BoxImageItemViewModel : ObservableObject
{
  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty;

  private readonly byte[] _imageBytes; // 元の画像データを保持

  // ⭐ Image プロパティは ObservableProperty を使用する
  [ObservableProperty]
  private ImageSource? image;

  public BoxImageItemViewModel(BoxImageItem item)
  {
    Id = item.Id;
    Name = item.Name;
    Type = item.Type;
    _imageBytes = item.Image; // データだけ保持

    // ⭐ コンストラクタから LoadImageAsync の呼び出しを完全に削除する！
  }

  // ⭐ 外部から呼び出すロードメソッド
  public async Task LoadImageAsync()
  {
    if (Image != null)
    {
      return;
    }

    try
    {
      // バックグラウンドで SKBitmap → ImageSource 変換
      var img = await Task.Run(() =>
      {
        using var ms = new MemoryStream(_imageBytes);
        var bmp = SKBitmap.Decode(ms);
        // 変換ロジックをここに
        return CharacomMaui.Presentation.Helpers.ImageSourceConverter.FromSKBitmap(bmp);
      });

      // ⭐ UIスレッドでの画像セットは、ObservableObject の SetProperty で行われる
      await MainThread.InvokeOnMainThreadAsync(() =>
      {
        Image = img;
      });
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[LoadImageAsync Error] {ex.Message}");
    }
  }
}