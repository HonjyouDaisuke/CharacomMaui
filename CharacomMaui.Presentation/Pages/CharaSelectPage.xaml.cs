using System.ComponentModel;
using System.Threading.Tasks;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Application.Models;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Presentation.Components;
using CharacomMaui.Presentation.Dialogs;
using CharacomMaui.Presentation.Models;
using CharacomMaui.Presentation.ViewModels;
using CharacomMaui.Presentation.Interfaces;
using CharacomMaui.Presentation.Services;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace CharacomMaui.Presentation.Pages;

public partial class CharaSelectPage : ContentPage
{
  private bool _isChanging = false;
  private readonly AppStatus _appStatus;
  private readonly AppStatusNotifier _notifier;
  private readonly CharaSelectViewModel _viewModel;
  readonly IProgressDialogService _progressDialog;
  private string _pageMaterialName = string.Empty;
  private string _pageCharaName = string.Empty;
  private bool _isFirstLoaded = false;
  private readonly SemaphoreSlim _progressUpdateLock = new(1, 1);
  private IProgressDialogSession? _currentSession = null;
  private IProgressPublisher ProgressPublisher
    => (IProgressPublisher)_viewModel;
  public SKBitmap? LoadedBitmap { get; set; }
  public CharaSelectPage(AppStatus appStatus,
    CharaSelectViewModel viewModel,
    AppStatusNotifier notifier,
    IProgressDialogService progressDialog)
  {
    InitializeComponent();
    _appStatus = appStatus;
    _viewModel = viewModel;
    _notifier = notifier;
    _progressDialog = progressDialog;

    // AppStatusNotifier の変更を購読
    _notifier.PropertyChanged += OnAppStatusChanged;

    BindingContext = _viewModel;
  }
  protected override async void OnDisappearing()
  {
    base.OnDisappearing();
    _isFirstLoaded = false;
    _notifier.PropertyChanged -= OnAppStatusChanged;
    // クリーンアップ処理中のセッション
    if (_currentSession != null)
    {
      await _currentSession.DisposeAsync();
    }

    _currentSession = null;
  }
  protected override async void OnAppearing()
  {
    base.OnAppearing();
    LogEditor.Text += "✳️✡️☆表示！\n";
    LogEditor.Text += "--------------------------\n";
    LogEditor.Text += $"_pageCharaName: {_pageCharaName} vs _appStatus.CharaNam: {_appStatus.CharaName} \n";
    LogEditor.Text += $"_pageMaterialName: {_pageMaterialName} vs _appStatus.MaterialName: {_appStatus.MaterialName} \n";
    LogEditor.Text += $"IsFirstLoaded: {_isFirstLoaded}\n";
    LogEditor.Text += "--------------------------\n";

    //LogEditor.Text += $"ProjectName = {_appStatus.ProjectName} id:{_appStatus.ProjectId}\n";
    //LogEditor.Text += $"CharaName = {_appStatus.CharaName} pageCharaName={_pageCharaName}\n";
    //LogEditor.Text += $"MaterialName = {_appStatus.MaterialName} pageMaterialName={_pageMaterialName}\n";

    if (_pageCharaName != _appStatus.CharaName || _pageMaterialName != _appStatus.MaterialName)
    {
      try
      {
        if (!_isFirstLoaded) await CreatePageViewAsync();
        _pageCharaName = _appStatus.CharaName!;
        _pageMaterialName = _appStatus.MaterialName;
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"Error loading chara items: {ex.Message}");
        await SnackBarService.Error("個別画像を表示中にエラーが発生しました");
      }
    }

    if (!_isFirstLoaded)
    {
      BindingContext = null;
      BindingContext = _viewModel;
      _isFirstLoaded = true;
    }
    System.Diagnostics.Debug.WriteLine($"ViewModel Items Count: {_viewModel.CurrentCharaItems.Count}");
  }
  private readonly SemaphoreSlim _sessionLock = new(1, 1);

  private async Task RunWithProgressAsync(string title, string message, Func<Task> action)
  {
    LogEditor.Text += $"RunWithProgressAsync: {title} - {message} _currentSession?={_currentSession != null}\n";

    await _sessionLock.WaitAsync();
    try
    {
      if (_currentSession != null)
        throw new InvalidOperationException("Progress session is already running.");

      await using var session = await _progressDialog.ShowAsync(title, message);
      _currentSession = session;
      ProgressPublisher.ProgressChanged += OnProgressChanged;

      try
      {
        await action();
      }
      finally
      {
        ProgressPublisher.ProgressChanged -= OnProgressChanged;
        _currentSession = null;
      }
    }
    finally
    {
      _sessionLock.Release();
    }
  }

  private async Task CreatePageViewAsync()
  {
    await _viewModel.RunBusyAsync(async () =>
    {
      await RunWithProgressAsync(
        "画面を準備",
        "ページ表示準備中...",
        () => _viewModel.GetCharaItemAsync()
      );
    });

  }

  private async void OnAppStatusChanged(object sender, PropertyChangedEventArgs e)
  {
    System.Diagnostics.Debug.WriteLine("---発火--- ★★★★★★★");
    if (e.PropertyName == nameof(AppStatusNotifier.CharaName) ||
        e.PropertyName == nameof(AppStatusNotifier.MaterialName))
    {
      System.Diagnostics.Debug.WriteLine($"c {_notifier.CharaName} m {_notifier.MaterialName}");
      // 変更された → 再取得する
      await _viewModel.OnChangeSelect(
          _notifier.CharaName!,
          _notifier.MaterialName!
      );
    }
  }

  private async void OnMaterialSelectBarItemSelected(object? sender, SelectBarEventArgs item)
  {

    if (_viewModel.IsLoading) return;
    System.Diagnostics.Debug.WriteLine($"OnMaterialSelectBarItemSelected isChanging = {_isChanging}");
    LogEditor.Text += $"選択された資料名: {item.SelectedName} pageMaterialName={_pageMaterialName}\n";
    if (string.IsNullOrEmpty(item.SelectedName)) return;
    if (string.IsNullOrEmpty(_appStatus.CharaName)) return;
    if (_appStatus.MaterialName == item.SelectedName)
    {
      System.Diagnostics.Debug.WriteLine($"同じ資料名が選択されました: {_appStatus.MaterialName}");
      return;
    }
    else
    {
      System.Diagnostics.Debug.WriteLine($"異なる資料名が選択されました: {_appStatus.MaterialName} -> {item.SelectedName}");
    }
    // TODO: エラーメッセージを出す
    if (_isChanging) return;
    _isChanging = true;
    try
    {
      await _viewModel.RunBusyAsync(async () =>
      {
        await RunWithProgressAsync(
          "資料変更",
          "ページ表示準備中...",
          () => _viewModel.OnChangeSelect(_appStatus.CharaName!, item.SelectedName)
        );
      });
      _pageMaterialName = _appStatus.MaterialName;
    }
    finally
    {
      _isChanging = false;
    }
  }

  private async void OnCharaSelectBarItemSelected(object? sender, SelectBarEventArgs item)
  {
    if (_viewModel.IsLoading) return;
    if (string.IsNullOrEmpty(item.SelectedName)) return;
    if (string.IsNullOrEmpty(_appStatus.MaterialName)) return;
    if (_appStatus.CharaName == item.SelectedName) return;

    if (_isChanging) return;
    _isChanging = true;

    try
    {
      await _viewModel.RunBusyAsync(async () =>
      {
        await RunWithProgressAsync(
          "個別文字変更",
          "ページ表示準備中...",
          () => _viewModel.OnChangeSelect(item.SelectedName, _appStatus.MaterialName)
        );
      });
      _pageCharaName = _appStatus.CharaName!;
    }
    finally
    {
      _isChanging = false;
    }
  }

  // Clickedイベントハンドラ
  private void OnCardClicked(object sender, CharaThumnailCardEventArgs e)
  {
    System.Diagnostics.Debug.WriteLine($"クリックされました {e.CharaId}-{e.Title} = {e.IsSelected}]");
    _ = _viewModel.UpdateCharaSelected(e.CharaId, e.IsSelected);
  }

  private void OnCardDoubleClicked(object sender, CharaThumnailCardEventArgs e)
  {
    System.Diagnostics.Debug.WriteLine($"ダブルクリックされました {e.CharaId}-{e.Title} = {e.IsSelected}]");
    OpenImageDialog(e.Title);
  }


  private async void OnDrawCanvas(object sender, EventArgs e)
  {
    await _viewModel.RunBusyAsync(async () =>
    {
      await RunWithProgressAsync(
        "処理中",
        "個別画像作成中...",
        () => _viewModel.CharaImageUpdateAsync()
      );
    });
  }
  private async void OpenImageDialog(string fileId)
  {
    var byteArray = await _viewModel.GetImageFromFileIdAsync(fileId);
    if (byteArray == null) return;
    var dialog = new ImageViewDialog(ImageSource.FromStream(() => new MemoryStream(byteArray)));
    var options = new PopupOptions
    {
      PageOverlayColor = Color.FromArgb("#88000000")
    };
    await this.ShowPopupAsync(dialog, options);
  }

  private async void OnProgressChanged(object sender, ImageProgress e)
  {
    try
    {
      if (_currentSession == null) return;
      if (e.Total == 0) return;

      var raw = (double)e.Current / (double)e.Total;
      var value = Math.Clamp(raw, 0d, 1d);

      await _progressUpdateLock.WaitAsync();
      try
      {
        await _currentSession.UpdateAsync(
          $"{e.Message} ({e.Current}/{e.Total})",
          value
        );
      }
      finally
      {
        _progressUpdateLock.Release();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(ex);
    }
  }
}