using SkiaSharp;

namespace CharacomMaui.Application.ImageProcessing;

public class OverlayProcess
{
  public static SKBitmap Overlay(SKBitmap bottom, SKBitmap top)
  {
    var result = new SKBitmap(bottom.Width, bottom.Height);

    using var canvas = new SKCanvas(result);
    canvas.Clear(SKColors.Transparent);

    // 下の画像
    canvas.DrawBitmap(bottom, 0, 0);

    // 白→透明にした上の画像
    var transparentTop = TransparentProcess.WhiteTransparent(top);

    // 上の画像を描画
    canvas.DrawBitmap(transparentTop, 0, 0);

    return result;
  }

}