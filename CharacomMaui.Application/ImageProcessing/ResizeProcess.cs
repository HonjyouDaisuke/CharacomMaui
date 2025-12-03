using SkiaSharp;

public static class ResizeProcess
{
  /// <summary>
  /// SKBitmapを指定の幅と高さにリサイズ
  /// </summary>
  public static SKBitmap Resize(SKBitmap src, int width, int height)
  {
    var info = new SKImageInfo(width, height);

    // SKSamplingOptions でフィルタ品質を指定
    var sampling = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);

    var resized = src.Resize(info, sampling);
    return resized;
  }
}
