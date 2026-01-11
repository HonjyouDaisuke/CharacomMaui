using CharacomMaui.Domain.Entities;
using SkiaSharp;

namespace CharacomMaui.Application.Models;

public sealed class CharaLoadResult : IAsyncDisposable
{
  public SKBitmap? StandardBitmap { get; }
  public SKBitmap? StrokeBitmap { get; }
  public IReadOnlyList<CharaSelectCardData> CharaItems { get; }

  public CharaLoadResult(
    SKBitmap? standardBitmap,
    SKBitmap? strokeBitmap,
    IReadOnlyList<CharaSelectCardData> charaItems)
  {
    StandardBitmap = standardBitmap;
    StrokeBitmap = strokeBitmap;
    CharaItems = charaItems;
  }

  public ValueTask DisposeAsync()
  {
    StandardBitmap?.Dispose();
    StrokeBitmap?.Dispose();
    return ValueTask.CompletedTask;
  }
}