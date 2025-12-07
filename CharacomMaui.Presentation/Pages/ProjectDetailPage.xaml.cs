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

  protected override void OnAppearing()
  {
    base.OnAppearing();
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
    if (sender is not CharaDataProgressRow clickedRow)
      return;

    if (BindingContext is not ProjectDetailViewModel vm)
      return;

    // Row とバインドされている「CharaDataSummary」を取得
    if (clickedRow.BindingContext is not CharaDataSummary summary)
      return;

    // 1) 全ての行を未選択にする
    foreach (var item in vm.CharaDataSummaries)
    {
      item.IsSelected = false;
    }

    // 2) 今回クリックした行だけ選択
    summary.IsSelected = true;

    // 3) 選択情報を更新
    _selectedRow = clickedRow;
    _appStatus.CharaName = summary.CharaName;
    _appStatus.MaterialName = summary.MaterialName;

    LogEditor.Text += $"[{summary.CharaName}-{summary.MaterialName}]が選択されました\n";
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