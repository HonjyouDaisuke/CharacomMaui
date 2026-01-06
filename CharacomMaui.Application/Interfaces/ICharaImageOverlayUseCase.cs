using SkiaSharp;
using CharacomMaui.Application.UseCases;
namespace CharacomMaui.Application.Interfaces;

public interface ICharaImageOverlayUseCase
{
  SKBitmap Execute(List<byte[]> images, IProgress<ImageProgress>? progress = null);
}
