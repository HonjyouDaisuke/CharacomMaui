using SkiaSharp;

namespace CharacomMaui.Application.ImageProcessing;

public static class CharaImageOverlayProcess
{
  public static SKBitmap CreateOverlayImage(SKBitmap baseBitmap, byte[] image, int width, int height)
  {
    if (image == null || image.Length == 0)
      throw new ArgumentException("Invalid image data", nameof(image));

    using var src = SKBitmap.Decode(image)
        ?? throw new InvalidOperationException("Failed to decode image");

    using var processed = Process(src, width, height);
    var result = OverlayProcess.Overlay(baseBitmap, processed);
    baseBitmap.Dispose();

    return result;
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
    using var resized = ResizeProcess.Resize(src, width, height);
    using var gray = GrayscaleProcess.ToGrayscale(resized);
    using var binary = BinaryProcess.ToBinaryOtsu(gray);
    using var denoise = NoiseCancelingProcess.Opening(binary);
    using var thin = ThinningProcess.Thinning(denoise);
    var dilated = NoiseCancelingProcess.Dilate(thin, 3);
    return dilated;
  }

}