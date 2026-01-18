namespace CharacomMaui.Tests.TestHelpers;

using SkiaSharp;

internal static class BitmapTestHelper
{
  public static SKBitmap CreateBinaryBitmap(
      int width,
      int height,
      Func<int, int, bool> isBlack)
  {
    var bmp = new SKBitmap(width, height);

    for (int y = 0; y < height; y++)
    {
      for (int x = 0; x < width; x++)
      {
        bmp.SetPixel(
          x,
          y,
          isBlack(x, y) ? SKColors.Black : SKColors.White);
      }
    }

    return bmp;
  }

  public static bool IsBlack(SKBitmap bmp, int x, int y)
    => bmp.GetPixel(x, y).Red < 128;
}