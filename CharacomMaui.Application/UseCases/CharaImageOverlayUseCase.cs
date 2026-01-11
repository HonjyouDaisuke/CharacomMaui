using CharacomMaui.Application.ImageProcessing;
using CharacomMaui.Application.Interfaces;

using SkiaSharp;

namespace CharacomMaui.Application.UseCases;

public class CharaImageOverlayUseCase : ICharaImageOverlayUseCase
{
  public CharaImageOverlayUseCase()
  {

  }

  public SKBitmap Execute(List<byte[]> images, IProgress<ImageProgress>? progress = null)
  {
    if (images == null) throw new ArgumentNullException(nameof(images));
    if (images.Count == 0) return CharaImageOverlayProcess.CreateWhiteBitmap(320, 320);

    var total = images.Count;
    var baseBitmap = CharaImageOverlayProcess.CreateWhiteBitmap(320, 320);
    int i = 0;
    foreach (var image in images)
    {
      System.Diagnostics.Debug.WriteLine($"画像処理中。。。({i + 1}/{total})");
      progress?.Report(new ImageProgress(i + 1, total, $"画像処理中... "));
      var prev = baseBitmap;
      baseBitmap = CharaImageOverlayProcess.CreateOverlayImage(baseBitmap, image, 320, 320);
      if (!ReferenceEquals(prev, baseBitmap)) prev.Dispose();
      i++;
    }
    return baseBitmap;
  }
}