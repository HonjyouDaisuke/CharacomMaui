
using System.Collections.ObjectModel;
using CharacomMaui.Application.ImageProcessing;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Presentation.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkiaSharp;
using UraniumUI.Dialogs;
using MauiControls = Microsoft.Maui.Controls;
using CharacomMaui.Presentation.Dialogs;
using CharacomMaui.Presentation.Services;

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
  readonly IAppTokenStorageService _tokenStorage;
  AppStatus _appStatus;

  private List<CharaData> allCharaData = new();

  // ObservableCollection(UI動的追加用)
  public ObservableCollection<SelectBarContents> CharaNames { get; } = [];
  public ObservableCollection<SelectBarContents> MaterialNames { get; } = [];
  public IAsyncRelayCommand<string>? CharaButtonCommand { get; }

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
                              IDialogService dialogService,
                              IAppTokenStorageService tokenStorage)
  {
    _useCase = useCase;
    _appStatus = appStatus;
    _fetchBoxItemUseCase = fetchBoxItemUseCase;
    _getStandardFileIdUseCase = getStandardFileIdUseCase;
    _getStrokeFileIdUseCase = getStrokeFileIdUseCase;
    _updateCharaSelectUseCase = updateCharaSelectedUseCase;
    _dialogService = dialogService;
    _tokenStorage = tokenStorage;
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
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[GetCharaItem Error] {ex.Message}");
      await Shell.Current.DisplayAlert("エラー", $"個別画像の取得でエラーが発生しました。", "OK");
    }
    finally
    {
      IsLoading = false;
    }
  }

  private async Task LoadCharaItemsCoreAsync()
  {
    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    if (accessToken == null) return;

    // 1. ダイアログの表示を開始 (表示が完了するのを待たず、表示命令を出したら次へ行く)
    _ = ShowProgressDialogAsync("読み込み中", "データを取得しています...");

    try
    {
      // Projectデータ読み込み
      UpdateProgressMessage("プロジェクトデータを読み込んでいます...", 0.25);
      await LoadProjectItems(accessToken);

      // Standard画像
      UpdateProgressMessage("Standard画像を読み込んでいます...", 0.5);
      await StandardImageUpdateAsync(accessToken);

      // Stroke画像
      UpdateProgressMessage("Stroke画像を読み込んでいます...", 0.75);
      await StrokeImageUpdateAsync(accessToken);

      // Chara選択画像群
      UpdateProgressMessage("Chara選択画像群を読み込んでいます...", 1.00);
      await UpdateCurrentCharaItemsAsync(accessToken);
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
      await CloseProgressDialogAsync();
    }
    finally
    {
      // 4. 確実に閉じる (少し遅延を入れるとUIのチラつきが抑えられ、確実に閉じやすくなります)
      await Task.Delay(100);
      await CloseProgressDialogAsync();
    }
  }

  private void UpdateMaterialNames()
  {
    MaterialNames.Clear();
    var Items = allCharaData
      .Where(x => x.CharaName == _appStatus.CharaName)
      .Select(x => x.MaterialName)
      .Distinct()
      .ToList();

    foreach (var materialItem in Items)
    {
      var count = allCharaData.Count(x => x.MaterialName == materialItem && x.CharaName == _appStatus.CharaName);
      MaterialNames.Add(new SelectBarContents
      {
        Name = materialItem,
        Count = count,
        Title = $"{materialItem} ({count})",
        IsDisabled = count <= 0,
      });
    }
  }

  private async Task LoadProjectItems(string accessToken)
  {
    try
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

      allCharaData = Items;

      CharaNames.Clear();
      foreach (var charaItem in CharaItems)
      {
        var count = CurrentItems.Count(x => x.CharaName == charaItem);
        CharaNames.Add(new SelectBarContents
        {
          Name = charaItem,
          Count = count,
          Title = $"{charaItem} ({count})",
          IsDisabled = count <= 0,
        });
      }

      MaterialNames.Clear();
      foreach (var materialItem in MaterialItems)
      {
        var count = allCharaData.Count(x => x.MaterialName == materialItem && x.CharaName == _appStatus.CharaName);
        MaterialNames.Add(new SelectBarContents
        {
          Name = materialItem,
          Count = count,
          Title = $"{materialItem} ({count})",
          IsDisabled = count <= 0,
        });

      }
      InitialMaterialName = _appStatus.MaterialName ?? MaterialNames.FirstOrDefault()?.Name ?? string.Empty;
      InitialCharaName = _appStatus.CharaName ?? CharaNames.FirstOrDefault()?.Name ?? string.Empty;

    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
    }

  }

  private async Task UpdateCurrentCharaItemsAsync(string accessToken)
  {
    var sw = System.Diagnostics.Stopwatch.StartNew();
    var tempList = new List<CharaSelectCardData>();

    foreach (var currentItem in allCharaData)
    {
      if (currentItem.CharaName != _appStatus.CharaName ||
          currentItem.MaterialName != _appStatus.MaterialName)
      {
        continue;
      }
      System.Diagnostics.Debug.WriteLine($"Loading Image for FileId: {currentItem.FileId}");
      var image = await LoadImageAsync(accessToken, currentItem.FileId) ?? [];
      if (image.Length > 0)
      {
        tempList.Add(new CharaSelectCardData
        {
          CharaId = currentItem.Id,
          FileId = currentItem.FileId,
          CharaName = currentItem.CharaName,
          MaterialName = currentItem.MaterialName,
          IsSelected = currentItem.IsSelected,
          RawImageData = image
        });
      }
    }

    await MainThread.InvokeOnMainThreadAsync(() =>
    {
      CurrentCharaItems.Clear();
      foreach (var item in tempList)
      {
        CurrentCharaItems.Add(item);
      }
    });

    sw.Stop();
    System.Diagnostics.Debug.WriteLine($"UpdateCurrentCharaItemsAsync completed in {sw.ElapsedMilliseconds} ms");
  }

  private async Task StandardImageUpdateAsync(string accessToken)
  {
    if (string.IsNullOrEmpty(_appStatus.CharaName))
    {
      System.Diagnostics.Debug.WriteLine("CharaName is null or empty, skipping standard image update.");
      return;
    }
    var standardFileId = await _getStandardFileIdUseCase.ExecuteAsync(accessToken, _appStatus.CharaName);
    var standardBytes = await LoadImageAsync(accessToken, standardFileId);
    if (standardBytes != null)
    {
      await MainThread.InvokeOnMainThreadAsync(() =>
      {
        using (var ms = new MemoryStream(standardBytes))
        {
          StandardBitmap = SKBitmap.Decode(ms);
        }
      });
    }

  }

  private async Task StrokeImageUpdateAsync(string accessToken)
  {
    if (string.IsNullOrEmpty(_appStatus.CharaName))
    {
      System.Diagnostics.Debug.WriteLine("CharaName is null or empty, skipping stroke image update.");
      return;
    }
    var strokeFileId = await _getStrokeFileIdUseCase.ExecuteAsync(accessToken, _appStatus.CharaName);
    var strokeBytes = await LoadImageAsync(accessToken, strokeFileId);
    if (strokeBytes != null)
    {
      await MainThread.InvokeOnMainThreadAsync(() =>
      {
        using (var ms = new MemoryStream(strokeBytes))
        {
          StrokeBitmap = SKBitmap.Decode(ms);
        }
      });
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

    System.Diagnostics.Debug.WriteLine($"OnChangeSelect: Chara={charaName}, Material={materialName} -- 前回 Chara={_appStatus.CharaName}, Material={_appStatus.MaterialName}  ");
    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    if (accessToken == null) return;
    _appStatus.CharaName = charaName;
    _appStatus.MaterialName = materialName;
    if (previousCharaName != charaName)
    {
      // Standard画像
      await StandardImageUpdateAsync(accessToken);
      // Stroke画像
      await StrokeImageUpdateAsync(accessToken);
      // 資料名の数え直し
      UpdateMaterialNames();
    }
    // Chara選択画像群
    await UpdateCurrentCharaItemsAsync(accessToken);

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
    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    if (accessToken == null) return false;
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
    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    if (accessToken == null) return [];
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
        using (await _dialogService.DisplayProgressAsync($"画像処理中...({num} / {count})", "画面を準備しています。しばらくお待ち下さい。"))
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

  // 現在表示中のダイアログを保持する変数
  private ProgressDialog? _activeDialog;
  // Page側がこれを聞き取って表示する
  public event Func<ProgressDialog, Task>? ShowDialogRequested;
  public event Func<Task>? CloseDialogRequested;
  public async Task ShowProgressDialogAsync(string title, string message)
  {
    _activeDialog = new ProgressDialog(title, message, _dialogService);
    if (ShowDialogRequested != null)
      await ShowDialogRequested.Invoke(_activeDialog);
  }

  // ★追加：メッセージを更新するメソッド
  public void UpdateProgressMessage(string newMessage, double value)
  {
    if (_activeDialog != null)
    {
      // UIスレッドで更新する必要がある
      MainThread.BeginInvokeOnMainThread(async () =>
      {
        _activeDialog.Message = newMessage;
        await _activeDialog.AnimateProgressAsync(value);
      });
    }
  }

  public async Task CloseProgressDialogAsync()
  {
    if (CloseDialogRequested != null)
      await CloseDialogRequested.Invoke();
    _activeDialog = null; // 参照をクリア
  }
}