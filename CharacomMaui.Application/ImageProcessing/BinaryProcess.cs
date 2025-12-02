using SkiaSharp;

namespace CharacomMaui.Application.ImageProcessing;

public class BinaryProcess
{
  public static SKBitmap ToBinaryOtsu(SKBitmap gray)
  {
    int width = gray.Width;
    int height = gray.Height;

    using var pix = gray.PeekPixels();

    // --- Step1: ヒストグラム作成 ---
    Span<byte> graySpan = pix.GetPixelSpan();
    int[] hist = new int[256];

    for (int i = 0; i < graySpan.Length; i++)
      hist[graySpan[i]]++;

    // --- Step2: Otsu で閾値計算 ---
    int total = width * height;
    long sum = 0;
    for (int i = 0; i < 256; i++)
      sum += i * hist[i];

    long sumB = 0;
    int wB = 0;
    int wF = 0;

    double maxVar = 0;
    int threshold = 0;

    for (int t = 0; t < 256; t++)
    {
      wB += hist[t];
      if (wB == 0) continue;

      wF = total - wB;
      if (wF == 0) break;

      sumB += t * hist[t];

      double mB = (double)sumB / wB;
      double mF = (double)(sum - sumB) / wF;

      // クラス間分散
      double varBetween = (double)wB * wF * (mB - mF) * (mB - mF);

      if (varBetween > maxVar)
      {
        maxVar = varBetween;
        threshold = t;
      }
    }

    // --- Step3: 二値化 ---
    var binBitmap = new SKBitmap(width, height, SKColorType.Gray8, SKAlphaType.Opaque);
    using var dstPix = binBitmap.PeekPixels();

    Span<byte> dstSpan = dstPix.GetPixelSpan();

    for (int i = 0; i < graySpan.Length; i++)
      dstSpan[i] = graySpan[i] < threshold ? (byte)0 : (byte)255;

    return binBitmap;
  }

}