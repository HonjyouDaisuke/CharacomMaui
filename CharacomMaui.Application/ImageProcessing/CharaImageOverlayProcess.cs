using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;
using SkiaSharp;

namespace CharacomMaui.Application.ImageProcessing;

public static class CharaImageOverlayProcess
{
  public static SKBitmap CreateOverlayImage(SKBitmap baseBitmap, byte[] image, int width, int height)
  {
    if (image == null)
    {
      throw new ArgumentException("Invalid image data", nameof(image));
    }
    using var src = SKBitmap.Decode(image);
    var processed = Process(src, width, height);
    baseBitmap = OverlayProcess.Overlay(baseBitmap, processed);
    return baseBitmap;
  }

  public static SKBitmap CreateWhiteBitmap(int width, int height)
  {
    var bitmap = new SKBitmap(width, height);
    using var canvas = new SKCanvas(bitmap);
    canvas.Clear(SKColors.White);
    return bitmap;
  }

  private static SKBitmap Process(SKBitmap src, int width, int height)
  {
    using var resized = ResizeProcess.Resize(src, 320, 320);
    using var gray = GrayscaleProcess.ToGrayscale(resized);
    using var binary = BinaryProcess.ToBinaryOtsu(gray);
    using var denoise = NoiseCancelingProcess.Opening(binary);
    using var thin = ThinningProcess.Thinning(denoise);
    var dilated = NoiseCancelingProcess.Dilate(thin, 3);
    return dilated;
  }

}