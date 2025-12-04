using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Presentation.Components;
using CharacomMaui.Presentation.Models;
using CharacomMaui.Presentation.ViewModels;
using UraniumUI.Dialogs;

namespace CharacomMaui.Presentation.Pages;

public partial class ProjectDetailPage : ContentPage
{
  private CharaDataProgressRow? _selectedRow;
  private AppStatus _appStatus;
  private GetProjectCharaItemsUseCase _useCase;
  private ProjectDetailViewModel _viewModel;
  private IDialogService _dialogService;
  public ProjectDetailPage(AppStatus appStatus, GetProjectCharaItemsUseCase useCase, ProjectDetailViewModel viewModel, IDialogService dialogService)
  {
    InitializeComponent();
    _appStatus = appStatus;
    _useCase = useCase;
    _dialogService = dialogService;
    _viewModel = viewModel;
    BindingContext = _viewModel;
  }

  /// <summary>
  /// Handles the page appearing: logs the event and initiates loading of character items.
  /// </summary>
  /// <remarks>
  /// Appends an entry to LogEditor and starts <see cref="GetCharaItemAsync"/> without awaiting so the fetch runs in the background.
  /// </remarks>
  protected override void OnAppearing()
  {
    base.OnAppearing();
    LogEditor.Text += "OnAppearing()... \n";
    _ = GetCharaItemAsync();
  }

  private async Task GetCharaItemAsync()
  {
    using (await _dialogService.DisplayProgressAsync("プロジェクト詳細ページ準備中", "プロジェクト画面を準備しています。少々お待ちください。"))
    {
      await _viewModel.FetchCharaDataAsync(_appStatus.ProjectId);
    }
  }

  private void OnRowClicked(object sender, CharaDataProgressRowEventArgs e)
  {
    System.Diagnostics.Debug.WriteLine("クリックされました");
    SelectRow(sender);
  }

  private void SelectRow(object sender)
  {
    if (sender is CharaDataProgressRow clickedRow)
    {
      // 前のカードの選択を解除
      if (_selectedRow != null && _selectedRow != clickedRow)
        _selectedRow.IsSelected = false;

      // 今回のカードを選択
      clickedRow.IsSelected = true;
      _selectedRow = clickedRow;
      _appStatus.CharaName = clickedRow.CharaName;
      _appStatus.MaterialName = clickedRow.MaterialName;
      // _notifier.ProjectName = _selectedCard.ProjectName;
      LogEditor.Text += $"[{_selectedRow.CharaName}-{_selectedRow.MaterialName}]が選択されました\n";
    }
  }

  private async void OnRowDoubleClicked(object sender, CharaDataProgressRowEventArgs e)
  {
    System.Diagnostics.Debug.WriteLine("ダブルクリックされました");
    SelectRow(sender);

    if (sender is CharaDataProgressRow clickedRow)
    {
      // 前のカードの選択を解除
      if (_selectedRow != null && _selectedRow != clickedRow)
        _selectedRow.IsSelected = false;

      // 今回のカードを選択
      clickedRow.IsSelected = true;
      _selectedRow = clickedRow;
      _appStatus.CharaName = clickedRow.CharaName;
      _appStatus.MaterialName = clickedRow.MaterialName;
      System.Diagnostics.Debug.WriteLine($"appStatus用意{_appStatus.CharaName}");

    }
    System.Diagnostics.Debug.WriteLine("Go!");
    await Shell.Current.GoToAsync(
        $"///CharaSelectPage"
    );
  }
}