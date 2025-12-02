using SkiaSharp;

namespace CharacomMaui.Application.ImageProcessing;

public class TransparentProcess
{
  public static SKBitmap WhiteTransparent(SKBitmap src)
  {
    int width = src.Width;
    int height = src.Height;

    // 出力（グレースケール）用
    var result = new SKBitmap(width, height, src.ColorType, src.AlphaType);
    var transparent = new SKColor(0, 0, 0, 0);
    for (int y = 0; y < height; y++)
    {
      for (int x = 0; x < width; x++)
      {
        var c = src.GetPixel(x, y);
        if (c.Red == 255)
        {
          result.SetPixel(x, y, transparent);
        }
        else
        {
          result.SetPixel(x, y, c);
        }
      }
    }
    return result;
  }

}