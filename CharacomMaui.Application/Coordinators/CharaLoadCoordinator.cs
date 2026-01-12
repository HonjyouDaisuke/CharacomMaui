
using System.Transactions;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Application.Models;
using CharacomMaui.Domain.Entities;
using SkiaSharp;
using System.Runtime.CompilerServices;

namespace CharacomMaui.Application.Coordinators;

public sealed class CharaLoadCoordinator : ICharaLoadCoordinator
{
  private readonly GetProjectCharaItemsUseCase _getProjectItems;
  private readonly GetStandardFileIdUseCase _getStandardFileId;
  private readonly GetStrokeFileIdUseCase _getStrokeFileId;
  private readonly FetchBoxItemUseCase _fetchBoxItem;

  public CharaLoadCoordinator(
    GetProjectCharaItemsUseCase getProjectItems,
    GetStandardFileIdUseCase getStandardFileId,
    GetStrokeFileIdUseCase getStrokeFileId,
    FetchBoxItemUseCase fetchBoxItem)
  {
    _getProjectItems = getProjectItems;
    _getStandardFileId = getStandardFileId;
    _getStrokeFileId = getStrokeFileId;
    _fetchBoxItem = fetchBoxItem;
  }

  public async Task<CharaLoadResult> LoadAsync(AppStatus status, string accessToken, IProgress<ImageProgress> progress)
  {
    // ProjectItem読み込み
    var items = await _getProjectItems.ExecuteAsync(accessToken, status.ProjectId);

    var targetItems = items
      .Where(x => x.CharaName == status.CharaName && x.MaterialName == status.MaterialName)
      .ToList();

    var totalSteps =
      1 // 標準画像
      + 1 // 筆順画像
      + targetItems.Count; // 個別画像

    var current = 0;

    // 進捗報告用ローカル関数
    // Reportが呼ばれたら、currentをインクリメントして進捗を報告する
    void Report(string message)
     => progress.Report(new ImageProgress(++current, totalSteps, message));

    // Standard画像
    SKBitmap? standardBitmap = null;
    Report("標準画像を読み込んでいます");

    var standardFileId = await _getStandardFileId.ExecuteAsync(accessToken, status.CharaName);
    var standardBytes = await LoadBinaryAsync(accessToken, standardFileId);
    if (standardBytes != null)
    {
      using var ms = new MemoryStream(standardBytes);
      standardBitmap = SKBitmap.Decode(ms);
    }

    // Stroke画像
    SKBitmap? strokeBitmap = null;
    Report("筆順画像を読み込んでいます");

    var strokeFileId = await _getStrokeFileId.ExecuteAsync(accessToken, status.CharaName);
    var strokeBytes = await LoadBinaryAsync(accessToken, strokeFileId);
    if (strokeBytes != null)
    {
      using var ms = new MemoryStream(strokeBytes);
      strokeBitmap = SKBitmap.Decode(ms);
    }
    // 個別画像
    var charaItems = new List<CharaSelectCardData>();
    if (targetItems.Count > 0)
    {
      foreach (var item in targetItems)
      {
        Report("個別画像を読み込んでいます");

        var bytes = await LoadBinaryAsync(accessToken, item.FileId);
        if (bytes == null) continue;

        charaItems.Add(new CharaSelectCardData
        {
          CharaId = item.Id,
          FileId = item.FileId,
          CharaName = item.CharaName,
          MaterialName = item.MaterialName,
          IsSelected = item.IsSelected,
          RawImageData = bytes
        });
      }
    }

    return new CharaLoadResult(
      standardBitmap,
      strokeBitmap,
      charaItems);
  }

  private async Task<byte[]?> LoadBinaryAsync(
    string accessToken,
    string fileId)
  {
    var result = await _fetchBoxItem.ExecuteAsync(accessToken, fileId);
    return result.Success ? result.BinaryData : null;
  }
}