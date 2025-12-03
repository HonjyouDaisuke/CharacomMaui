using SkiaSharp;

namespace CharacomMaui.Presentation.Helpers;

public static class ImageSourceConverter
{
  /// <summary>
  /// SKBitmap を MAUI の ImageSource (FromStream) に変換して返す。
  /// 戻り値は null になり得るので呼び出し側でチェックしてください。
  /// </summary>
  public static ImageSource? FromSKBitmap(SKBitmap? bitmap)
  {
    if (bitmap == null)
      return null;

    // SKBitmap -> SKImage -> バイト配列(PNG) -> MemoryStream -> ImageSource.FromStream
    using var image = SKImage.FromBitmap(bitmap);
    using var data = image.Encode(SKEncodedImageFormat.Png, 90);
    if (data == null)
      return null;

    var bytes = data.ToArray();
    // 注意: FromStream に渡すラムダは毎回新しいストリームを作る必要がある
    return ImageSource.FromStream(() => new MemoryStream(bytes));
  }
}

