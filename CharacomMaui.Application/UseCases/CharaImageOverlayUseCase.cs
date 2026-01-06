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
    var total = images.Count;
    var baseBitmap = CharaImageOverlayProcess.CreateWhiteBitmap(320, 320);
    int i = 0;
    foreach (var image in images)
    {
      System.Diagnostics.Debug.WriteLine($"画像処理中。。。({i + 1}/{total})");
      progress?.Report(new ImageProgress(i + 1, total, $"画像処理中... ({i + 1}/{total})"));
      baseBitmap = CharaImageOverlayProcess.CreateOverlayImage(baseBitmap, image, 320, 320);
      i++;
    }
    return baseBitmap;
  }
}