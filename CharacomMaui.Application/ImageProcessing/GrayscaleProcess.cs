using SkiaSharp;

namespace CharacomMaui.Application.ImageProcessing;

public class GrayscaleProcess
{
  public static SKBitmap ToGrayscale(SKBitmap src)
  {
    int width = src.Width;
    int height = src.Height;

    // 出力（グレースケール）用
    var grayBitmap = new SKBitmap(width, height, SKColorType.Gray8, SKAlphaType.Opaque);

    using var srcPixmap = src.PeekPixels();
    using var dstPixmap = grayBitmap.PeekPixels();

    int srcRowBytes = srcPixmap.RowBytes;
    int dstRowBytes = dstPixmap.RowBytes;

    // RGBA ビットマップ前提
    for (int y = 0; y < height; y++)
    {
      // 1行分の Span を取得
      var srcRow = srcPixmap.GetPixelSpan().Slice(y * srcRowBytes, width * 4);
      var dstRow = dstPixmap.GetPixelSpan().Slice(y * dstRowBytes, width);

      for (int x = 0; x < width; x++)
      {
        int idx = x * 4;

        byte r = srcRow[idx + 0];
        byte g = srcRow[idx + 1];
        byte b = srcRow[idx + 2];

        // 輝度 = 0.299R + 0.587G + 0.114B
        byte gray = (byte)(0.299 * r + 0.587 * g + 0.114 * b);

        dstRow[x] = gray;
      }
    }

    return grayBitmap;
  }

}