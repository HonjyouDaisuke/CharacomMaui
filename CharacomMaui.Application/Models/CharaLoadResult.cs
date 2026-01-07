using CharacomMaui.Domain.Entities;
using SkiaSharp;

namespace CharacomMaui.Application.Models;

public sealed record CharaLoadResult(
  SKBitmap? StandardBitmap,
  SKBitmap? StrokeBitmap,
  IReadOnlyList<CharaSelectCardData> CharaItems
);