using SkiaSharp;

namespace CharacomMaui.Application.ImageProcessing;

public static class ThinningProcess
{
  // <summary>
  /// Zhang-Suen 細線化（Thinning）
  /// 入力は 2値化済み（黒=255 / 白=0）の画像
  /// </summary>
  public static SKBitmap Thinning(SKBitmap binary)
  {
    int w = binary.Width;
    int h = binary.Height;

    // ここが重要！黒線=0 → 1、白=255 → 0 に反転する
    byte[,] img = new byte[h, w];
    for (int y = 0; y < h; y++)
    {
      for (int x = 0; x < w; x++)
      {
        var c = binary.GetPixel(x, y);
        img[y, x] = (c.Red < 128) ? (byte)1 : (byte)0;
        // 黒線が1になるように反転
      }
    }

    bool changed;

    do
    {
      changed = false;
      List<(int y, int x)> toRemove = new();

      // Step 1
      for (int y = 1; y < h - 1; y++)
      {
        for (int x = 1; x < w - 1; x++)
        {
          if (img[y, x] != 1) continue; // 前景 pixel=1

          var p2 = img[y - 1, x];
          var p3 = img[y - 1, x + 1];
          var p4 = img[y, x + 1];
          var p5 = img[y + 1, x + 1];
          var p6 = img[y + 1, x];
          var p7 = img[y + 1, x - 1];
          var p8 = img[y, x - 1];
          var p9 = img[y - 1, x - 1];

          int neighbors = p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9;
          if (neighbors < 2 || neighbors > 6) continue;

          int transitions =
              (p2 == 0 && p3 == 1 ? 1 : 0) +
              (p3 == 0 && p4 == 1 ? 1 : 0) +
              (p4 == 0 && p5 == 1 ? 1 : 0) +
              (p5 == 0 && p6 == 1 ? 1 : 0) +
              (p6 == 0 && p7 == 1 ? 1 : 0) +
              (p7 == 0 && p8 == 1 ? 1 : 0) +
              (p8 == 0 && p9 == 1 ? 1 : 0) +
              (p9 == 0 && p2 == 1 ? 1 : 0);

          if (transitions != 1) continue;

          if (p2 * p4 * p6 != 0) continue;
          if (p4 * p6 * p8 != 0) continue;

          toRemove.Add((y, x));
          changed = true;
        }
      }

      foreach (var (y, x) in toRemove)
        img[y, x] = 0;

      toRemove.Clear();

      // Step 2
      for (int y = 1; y < h - 1; y++)
      {
        for (int x = 1; x < w - 1; x++)
        {
          if (img[y, x] != 1) continue;

          var p2 = img[y - 1, x];
          var p3 = img[y - 1, x + 1];
          var p4 = img[y, x + 1];
          var p5 = img[y + 1, x + 1];
          var p6 = img[y + 1, x];
          var p7 = img[y + 1, x - 1];
          var p8 = img[y, x - 1];
          var p9 = img[y - 1, x - 1];

          int neighbors = p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9;
          if (neighbors < 2 || neighbors > 6) continue;

          int transitions =
              (p2 == 0 && p3 == 1 ? 1 : 0) +
              (p3 == 0 && p4 == 1 ? 1 : 0) +
              (p4 == 0 && p5 == 1 ? 1 : 0) +
              (p5 == 0 && p6 == 1 ? 1 : 0) +
              (p6 == 0 && p7 == 1 ? 1 : 0) +
              (p7 == 0 && p8 == 1 ? 1 : 0) +
              (p8 == 0 && p9 == 1 ? 1 : 0) +
              (p9 == 0 && p2 == 1 ? 1 : 0);

          if (transitions != 1) continue;

          if (p2 * p4 * p8 != 0) continue;
          if (p2 * p6 * p8 != 0) continue;

          toRemove.Add((y, x));
          changed = true;
        }
      }

      foreach (var (y, x) in toRemove)
        img[y, x] = 0;

    } while (changed);

    // 結果を SKBitmap に戻す（黒=0→255, 白=1→0 に戻す）
    SKBitmap result = new SKBitmap(w, h);
    for (int y = 0; y < h; y++)
    {
      for (int x = 0; x < w; x++)
      {
        result.SetPixel(x, y, img[y, x] == 1 ? SKColors.Black : SKColors.White);
      }
    }

    return result;
  }
}