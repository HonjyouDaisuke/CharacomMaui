
using System.Collections.ObjectModel;
using CharacomMaui.Application.ImageProcessing;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkiaSharp;
using UraniumUI.Dialogs;

namespace CharacomMaui.Presentation.ViewModels;

public partial class CharaSelectViewModel : ObservableObject
{
  [ObservableProperty]
  private SKBitmap? strokeBitmap;
  [ObservableProperty]
  private SKBitmap? standardBitmap;
  [ObservableProperty]
  private SKBitmap? charaImageBitmap;

  [ObservableProperty]
  private string initialMaterialName = string.Empty;

  [ObservableProperty]
  private string initialCharaName = string.Empty;


  readonly GetProjectCharaItemsUseCase _useCase;
  readonly GetStandardFileIdUseCase _getStandardFileIdUseCase;
  readonly GetStrokeFileIdUseCase _getStrokeFileIdUseCase;
  readonly FetchBoxItemUseCase _fetchBoxItemUseCase;
  readonly UpdateCharaSelectedUseCase _updateCharaSelectUseCase;
  AppStatus _appStatus;

  private List<CharaData> allCharaData = new();

  // ObservableCollection(UI動的追加用)
  public ObservableCollection<string> CharaNames { get; } = [];
  public ObservableCollection<string> MaterialNames { get; } = [];
  public IAsyncRelayCommand<string> CharaButtonCommand { get; }

  public ObservableCollection<CharaSelectCardData> CurrentCharaItems { get; } = [];
  private readonly IDialogService _dialogService;
  [ObservableProperty]
  private bool isLoading = false;
  public CharaSelectViewModel(AppStatus appStatus,
                              GetProjectCharaItemsUseCase useCase,
                              FetchBoxItemUseCase fetchBoxItemUseCase,
                              GetStandardFileIdUseCase getStandardFileIdUseCase,
                              GetStrokeFileIdUseCase getStrokeFileIdUseCase,
                              UpdateCharaSelectedUseCase updateCharaSelectedUseCase,
                              IDialogService dialogService)
  {
    _useCase = useCase;
    _appStatus = appStatus;
    _fetchBoxItemUseCase = fetchBoxItemUseCase;
    _getStandardFileIdUseCase = getStandardFileIdUseCase;
    _getStrokeFileIdUseCase = getStrokeFileIdUseCase;
    _updateCharaSelectUseCase = updateCharaSelectedUseCase;
    _dialogService = dialogService;
    // CharaButtonCommand = new AsyncRelayCommand<string>(OnCharaTapped);
  }

  public async Task GetCharaItemAsync()
  {
    if (IsLoading) return;
    try
    {
      IsLoading = true;
      await LoadCharaItemsCoreAsync();
    }
    finally
    {
      IsLoading = false;
    }
  }

  public async Task LoadCharaItemsCoreAsync()
  {
    var accessToken = Preferences.Get("app_access_token", string.Empty);

    // Projectデータ読み込み
    await LoadProjectItems(accessToken);

    // Standard画像
    await StandardImageUpdateAsync(accessToken);

    // Stroke画像
    await StrokeImageUpdateAsync(accessToken);

    // Chara選択画像群
    await UpdateCurrentCharaItemsAsync(accessToken);
  }

  private async Task LoadProjectItems(string accessToken)
  {
    using (await _dialogService.DisplayProgressAsync("文字選択画面準備中", "プロジェクトデータ取得中・・・\nしばらくお待ち下さい"))
    {
      var Items = await _useCase.ExecuteAsync(accessToken, _appStatus.ProjectId);
      var CharaItems = Items
        .Select(x => x.CharaName)
        .Distinct()
        .ToList();

      var MaterialItems = Items
        .Select(x => x.MaterialName)
        .Distinct()
        .ToList();

      var CurrentItems = Items
        .Select(x => new
        {
          x.FileId,
          x.CharaName,
          x.MaterialName,
          x.TimesName
        })
        .Distinct()
        .ToList();

      allCharaData.Clear();
      allCharaData = Items;

      CharaNames.Clear();
      foreach (var charaItem in CharaItems)
      {
        CharaNames.Add(charaItem);
      }

      MaterialNames.Clear();
      foreach (var materialItem in MaterialItems)
      {
        MaterialNames.Add(materialItem);
      }

      InitialMaterialName = _appStatus.MaterialName ?? MaterialNames.FirstOrDefault() ?? string.Empty;
      InitialCharaName = _appStatus.CharaName ?? CharaNames.FirstOrDefault() ?? string.Empty;
    }
  }

  private async Task UpdateCurrentCharaItemsAsync(string accessToken)
  {
    CurrentCharaItems.Clear();
    var sw = System.Diagnostics.Stopwatch.StartNew();
    int CharaCount = allCharaData
        .Count(x => x.CharaName == _appStatus.CharaName
                 && x.MaterialName == _appStatus.MaterialName);
    int count = 1;
    foreach (var currentItem in allCharaData)
    {
      if (currentItem.CharaName != _appStatus.CharaName || currentItem.MaterialName != _appStatus.MaterialName)
      {
        continue;
      }
      using (await _dialogService.DisplayProgressAsync("文字選択画面準備中", $"画面一覧を準備しています。({count}/{CharaCount})\nしばらくお待ち下さい。"))
      {
        System.Diagnostics.Debug.WriteLine($"Loading Image for FileId: {currentItem.FileId}");
        var image = await LoadImageAsync(accessToken, currentItem.FileId) ?? [];
        /**
        var image = await LoadThumbnailAsync(accessToken, currentItem.FileId, 32, 32) ?? [];
        if (image.Length == 0)
        {
          System.Diagnostics.Debug.WriteLine($"サムネイル取得エラー FileId: {currentItem.FileId}");
          image = await LoadImageAsync(accessToken, currentItem.FileId) ?? [];
        }
        **/
        if (image.Length == 0)
        {
          System.Diagnostics.Debug.WriteLine($"画像取得エラー FileId: {currentItem.FileId}");
          continue;
        }
        System.Diagnostics.Debug.WriteLine($"CharaId = {currentItem.Id}, FileId = {currentItem.FileId}");
        CurrentCharaItems.Add(new CharaSelectCardData
        {
          CharaId = currentItem.Id,
          FileId = currentItem.FileId,
          CharaName = currentItem.CharaName,
          MaterialName = currentItem.MaterialName,
          TimesName = currentItem.TimesName,
          IsSelected = currentItem.IsSelected,
          RawImageData = image
        });
        count++;
      }
    }
    sw.Stop();
    System.Diagnostics.Debug.WriteLine($"UpdateCurrentCharaItemsAsync completed in {sw.ElapsedMilliseconds} ms");
  }

  private async Task StandardImageUpdateAsync(string accessToken)
  {
    using (await _dialogService.DisplayProgressAsync("文字選択画面準備中", "標準字体準備中・・・\nしばらくお待ち下さい。"))
    {
      var standardFileId = await _getStandardFileIdUseCase.ExecuteAsync(accessToken, _appStatus.CharaName!);
      var standardBytes = await LoadImageAsync(accessToken, standardFileId);
      if (standardBytes != null)
      {
        using (var ms = new MemoryStream(standardBytes))
        {
          StandardBitmap = SKBitmap.Decode(ms);
        }
      }
    }
  }

  private async Task StrokeImageUpdateAsync(string accessToken)
  {
    using (await _dialogService.DisplayProgressAsync("文字選択画面準備中", "筆順画像準備中・・・\nしばらくお待ち下さい。"))
    {
      var strokeFileId = await _getStrokeFileIdUseCase.ExecuteAsync(accessToken, _appStatus.CharaName!);
      var strokeBytes = await LoadImageAsync(accessToken, strokeFileId);
      if (strokeBytes != null)
      {
        using (var ms = new MemoryStream(strokeBytes))
        {
          StrokeBitmap = SKBitmap.Decode(ms);
        }
      }
    }
  }


  public async Task OnChangeSelect(string charaName, string materialName)
  {
    await UpdateMasters(charaName, materialName);
  }
  public async Task UpdateMasters(string charaName, string materialName)
  {
    if (_appStatus.CharaName == charaName && _appStatus.MaterialName == materialName)
    {
      System.Diagnostics.Debug.WriteLine($"同じです。");
      return;
    }

    var previousCharaName = _appStatus.CharaName;

    using (await _dialogService.DisplayProgressAsync("文字選択画面準備中", "画面を準備しています。少々お待ちください。"))
    {
      System.Diagnostics.Debug.WriteLine($"OnChangeSelect: Chara={charaName}, Material={materialName} -- 前回 Chara={_appStatus.CharaName}, Material={_appStatus.MaterialName}  ");
      var accessToken = Preferences.Get("app_access_token", string.Empty);
      _appStatus.CharaName = charaName;
      _appStatus.MaterialName = materialName;
      if (previousCharaName != charaName)
      {
        // Standard画像
        await StandardImageUpdateAsync(accessToken);
        // Stroke画像
        await StrokeImageUpdateAsync(accessToken);
      }
      // Chara選択画像群
      await UpdateCurrentCharaItemsAsync(accessToken);
    }
  }

  public async Task<byte[]?> LoadImageAsync(string accessToken, string fileId)
  {
    var result = await _fetchBoxItemUseCase.ExecuteAsync(accessToken, fileId);

    if (!result.Success)
    {
      return null;
    }
    // using var ms = new MemoryStream(result.BinaryData!);
    return result.BinaryData;
  }

  public async Task<byte[]?> LoadThumbnailAsync(string accessToken, string fileId, int width, int height)
  {
    var result = await _fetchBoxItemUseCase.FetchThumbNailAsync(accessToken, fileId, width, height);

    if (!result.Success)
    {
      return null;
    }
    // using var ms = new MemoryStream(result.BinaryData!);
    return result.BinaryData;
  }

  public async Task<bool> UpdateCharaSelected(string charaId, bool isSelected)
  {
    var accessToken = Preferences.Get("app_access_token", string.Empty);
    var result = await _updateCharaSelectUseCase.ExecuteAsync(accessToken, charaId, isSelected);

    foreach (var item in CurrentCharaItems)
    {
      if (item.CharaId == charaId) item.IsSelected = isSelected;
    }
    // await CharaImageUpdateAsync();

    return result.Success;
  }

  public async Task<byte[]?> GetImageFromFileIdAsync(string fileId)
  {
    var accessToken = Preferences.Get("app_access_token", string.Empty);
    return await LoadImageAsync(accessToken, fileId);
  }

  private async Task<SKBitmap> CreateWhiteBitmap(int width, int height)
  {
    var bitmap = new SKBitmap(width, height);
    using var canvas = new SKCanvas(bitmap);
    canvas.Clear(SKColors.White);
    return bitmap;
  }

  public async Task CharaImageUpdateAsync()
  {
    CharaImageBitmap = await CreateWhiteBitmap(320, 320);
    int count = CurrentCharaItems.Count(x => x.IsSelected == true);
    int num = 1;
    foreach (var item in CurrentCharaItems)
    {
      if (item == null) continue;
      if (item.IsSelected == true)
      {
        using (await _dialogService.DisplayProgressAsync($"画像処理中...({num} / {count})", "画面を準備しています。少々お待ちください。"))
        {
          System.Diagnostics.Debug.WriteLine($"item.fileId = {item.FileId}");
          SKBitmap src = SKBitmap.Decode(item.RawImageData);
          await UpdateCharaImageAsync(src);
          num++;
        }
      }
    }
  }

  private async Task UpdateCharaImageAsync(SKBitmap src)
  {
    var result = await Task.Run(() =>
    {
      var resized = ResizeProcess.Resize(src, 320, 320);
      var gray = GrayscaleProcess.ToGrayscale(resized);
      var binary = BinaryProcess.ToBinaryOtsu(gray);
      var denoise = NoiseCancelingProcess.Opening(binary);
      var thin = ThinningProcess.Thinning(denoise);
      var dilated = NoiseCancelingProcess.Dilate(thin, 3);
      return dilated;
    });

    CharaImageBitmap ??= await CreateWhiteBitmap(320, 320);

    CharaImageBitmap = OverlayProcess.Overlay(CharaImageBitmap, result);
  }
}