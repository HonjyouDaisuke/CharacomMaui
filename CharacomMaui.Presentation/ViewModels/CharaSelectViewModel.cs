
using System.Collections.ObjectModel;
using CharacomMaui.Application.ImageProcessing;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Application.Coordinators;
using CharacomMaui.Application.Models;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Presentation.Models;
using CharacomMaui.Presentation.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkiaSharp;
using UraniumUI.Dialogs;
using MauiControls = Microsoft.Maui.Controls;
using CharacomMaui.Presentation.Dialogs;
using CharacomMaui.Presentation.Services;
using CharacomMaui.Presentation.Interfaces;

namespace CharacomMaui.Presentation.ViewModels;

public partial class CharaSelectViewModel : ObservableObject, IProgressPublisher
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



  readonly FetchBoxItemUseCase _fetchBoxItemUseCase;
  readonly UpdateCharaSelectedUseCase _updateCharaSelectUseCase;
  readonly IAppTokenStorageService _tokenStorage;
  readonly IProgressDialogService _progressDialog;
  readonly ICharaImageOverlayUseCase _overlayUseCase;
  readonly ICharaLoadCoordinator _charaLoadCoordinator;
  readonly IProjectItemsLoadCoordinator _projectItemsLoadCoordinator;
  public event EventHandler<ImageProgress>? ProgressChanged;
  AppStatus _appStatus;

  private List<CharaData> allCharaData = new();

  // ObservableCollection(UI動的追加用)
  public ObservableCollection<SelectBarContents> CharaNames { get; } = [];
  public ObservableCollection<SelectBarContents> MaterialNames { get; } = [];
  public IAsyncRelayCommand<string>? CharaButtonCommand { get; }

  public ObservableCollection<CharaSelectCardData> CurrentCharaItems { get; } = [];
  [ObservableProperty]
  private bool isLoading = false;
  private bool _isBusy;
  public bool IsBusy
  {
    get => _isBusy;
    private set => SetProperty(ref _isBusy, value);
  }

  private CharaLoadResult? _currentResult;

  public CharaSelectViewModel(AppStatus appStatus,
                              FetchBoxItemUseCase fetchBoxItemUseCase,
                              UpdateCharaSelectedUseCase updateCharaSelectedUseCase,
                              IAppTokenStorageService tokenStorage,
                              IProgressDialogService progressDialog,
                              ICharaImageOverlayUseCase overlayUseCase,
                              ICharaLoadCoordinator charaLoadCoordinator,
                              IProjectItemsLoadCoordinator projectItemsLoadCoordinator)
  {
    _appStatus = appStatus;
    _fetchBoxItemUseCase = fetchBoxItemUseCase;
    _updateCharaSelectUseCase = updateCharaSelectedUseCase;
    _tokenStorage = tokenStorage;
    _progressDialog = progressDialog;
    _overlayUseCase = overlayUseCase;
    _charaLoadCoordinator = charaLoadCoordinator;
    _projectItemsLoadCoordinator = projectItemsLoadCoordinator;
  }
  public async Task RunBusyAsync(Func<Task> action)
  {
    if (IsBusy) return;

    try
    {
      IsBusy = true;
      await action();
    }
    finally
    {
      IsBusy = false;
    }
  }
  public async Task GetCharaItemAsync()
  {

    if (IsLoading) return;
    if (_currentResult != null)
      await _currentResult.DisposeAsync();
    try
    {
      IsLoading = true;
      await LoadCharaItemsCoreAsync();
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[GetCharaItem Error] {ex.Message}");
      await SnackBarService.Error("個別画像の取得でエラーが発生しました。");
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
    await LoadProjectItemsAsync(accessToken);
    var progress = new Progress<ImageProgress>(p =>
    {
      ProgressChanged?.Invoke(this, p);
    });

    var result = await _charaLoadCoordinator.LoadAsync(_appStatus, accessToken, progress);
    _currentResult = result;

    StandardBitmap = result.StandardBitmap;
    StrokeBitmap = result.StrokeBitmap;

    CurrentCharaItems.Clear();
    foreach (var item in result.CharaItems)
      CurrentCharaItems.Add(item);
  }

  private async Task<bool> LoadProjectItemsAsync(string accessToken)
  {
    var result = await _projectItemsLoadCoordinator.LoadProjectItemsAsync(accessToken);

    if (result == null) return false;

    allCharaData = result.AllCharaData;

    CharaNames.Clear();
    foreach (var charaItem in result.CharaNames)
      CharaNames.Add(SelectBarContentsConverter.ToSelectedBarContents(charaItem));

    MaterialNames.Clear();
    foreach (var materialItem in result.MaterialNames)
      MaterialNames.Add(SelectBarContentsConverter.ToSelectedBarContents(materialItem));

    InitialMaterialName = _appStatus.MaterialName ?? MaterialNames.FirstOrDefault()?.Name ?? string.Empty;
    InitialCharaName = _appStatus.CharaName ?? CharaNames.FirstOrDefault()?.Name ?? string.Empty;
    return true;
  }

  public async Task OnChangeSelect(string charaName, string materialName)
  {
    await UpdateMasters(charaName, materialName);
  }
  public async Task UpdateMasters(string charaName, string materialName)
  {
    System.Diagnostics.Debug.WriteLine($"UpdateMasters: Chara={charaName}, Material={materialName}");
    if (_appStatus.CharaName == charaName && _appStatus.MaterialName == materialName)
    {
      await SnackBarService.Info("同じ文字種と資料名が選択されました。");
      return;
    }
    _appStatus.CharaName = charaName;
    _appStatus.MaterialName = materialName;
    await GetCharaItemAsync();
  }

  public async Task<byte[]?> LoadImageAsync(string accessToken, string fileId)
  {
    var result = await _fetchBoxItemUseCase.ExecuteAsync(accessToken, fileId);
    if (!result.Success) return null;
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

  public async Task CharaImageUpdateAsync()
  {
    var selectedImages = CurrentCharaItems
      .Where(x => x.IsSelected == true)
      .Select(x => x.RawImageData)
      .ToList();

    if (!selectedImages.Any()) return;

    var progress = new Progress<ImageProgress>(p =>
    {
      ProgressChanged?.Invoke(this, p);
    });

    var old = CharaImageBitmap;
    CharaImageBitmap = await Task.Run(() =>
       _overlayUseCase.Execute(selectedImages, progress));
    old?.Dispose();
  }
}