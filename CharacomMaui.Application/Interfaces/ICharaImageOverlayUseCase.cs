using SkiaSharp;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Application.Models;

namespace CharacomMaui.Application.Interfaces;

public interface ICharaImageOverlayUseCase
{
  /// <summary>
  /// 返却された SKBitmap の破棄責務は呼び出し側にあります（要確認）。
  /// </summary>
  SKBitmap Execute(List<byte[]> images, IProgress<ImageProgress>? progress = null);
}
