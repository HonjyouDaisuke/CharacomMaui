using CharacomMaui.Application.Models;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;

using SkiaSharp;

namespace CharacomMaui.Application.Interfaces;

public interface ICharaLoadCoordinator
{
  Task<CharaLoadResult> LoadAsync(
    AppStatus status,
    string accessToken,
    IProgress<ImageProgress> progress);
}
