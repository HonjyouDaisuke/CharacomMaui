using SkiaSharp;

namespace CharacomMaui.Application.ImageProcessing;

// 使い方
// var cleaned = SkiaMorphologyService.Opening(binaryBitmap); // kernelSize=3 デフォルト

public static class NoiseCancelingProcess
{
  /// <summary>
  /// 2値画像（白0/黒255）に対し、Opening（収縮→膨張）を実行してノイズ除去
  /// </summary>
  public static SKBitmap Opening(SKBitmap binary, int kernelSize = 3)
  {
    var eroded = Erode(binary, kernelSize);
    var dilated = Dilate(eroded, kernelSize);
    eroded.Dispose();
    return dilated;
  }

  private static SKBitmap Erode(SKBitmap src, int kernelSize)
  {
    int w = src.Width;
    int h = src.Height;
    int radius = kernelSize / 2;

    var dst = new SKBitmap(w, h);

    for (int y = 0; y < h; y++)
    {
      for (int x = 0; x < w; x++)
      {
        bool allWhite = true;

        for (int ky = -radius; ky <= radius; ky++)
        {
          for (int kx = -radius; kx <= radius; kx++)
          {
            int px = x + kx;
            int py = y + ky;

            if (px < 0 || py < 0 || px >= w || py >= h)
              continue;

            var c = src.GetPixel(px, py);
            if (c.Red < 128) // 黒があれば白にできない
            {
              allWhite = false;
              goto END_LOOP;
            }
          }
        }

      END_LOOP:
        dst.SetPixel(x, y, allWhite ? SKColors.White : SKColors.Black);
      }
    }

    return dst;
  }

  public static SKBitmap Dilate(SKBitmap src, int kernelSize)
  {
    int w = src.Width;
    int h = src.Height;
    int radius = kernelSize / 2;

    var dst = new SKBitmap(w, h);

    for (int y = 0; y < h; y++)
    {
      for (int x = 0; x < w; x++)
      {
        bool anyBlack = false;

        for (int ky = -radius; ky <= radius; ky++)
        {
          for (int kx = -radius; kx <= radius; kx++)
          {
            int px = x + kx;
            int py = y + ky;

            if (px < 0 || py < 0 || px >= w || py >= h)
              continue;

            var c = src.GetPixel(px, py);
            if (c.Red < 128) // 黒が1つでもあれば黒
            {
              anyBlack = true;
              goto END_LOOP;
            }
          }
        }

      END_LOOP:
        dst.SetPixel(x, y, anyBlack ? SKColors.Black : SKColors.White);
      }
    }

    return dst;
  }
}