using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Presentation.Components;
using CharacomMaui.Presentation.ViewModels;

namespace CharacomMaui.Presentation.Pages;

public partial class ProjectDetailPage : ContentPage
{
  private CharaDataProgressRow? _selectedRow;
  AppStatus _appStatus;
  GetProjectCharaItemsUseCase _useCase;
  ProjectDetailViewModel _viewModel;
  public ProjectDetailPage(AppStatus appStatus, GetProjectCharaItemsUseCase useCase, ProjectDetailViewModel viewModel)
  {
    InitializeComponent();
    _appStatus = appStatus;
    _useCase = useCase;
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
    await _viewModel.FetchCharaDataAsync(_appStatus.ProjectId);
  }

  private void OnRowClicked(object sender, CharaDataProgressRowEventArgs e)
  {
    System.Diagnostics.Debug.WriteLine("クリックされました");
    if (sender is CharaDataProgressRow clickedRow)
    {
      // 前のカードの選択を解除
      if (_selectedRow != null && _selectedRow != clickedRow)
        _selectedRow.IsSelected = false;

      // 今回のカードを選択
      clickedRow.IsSelected = true;
      _selectedRow = clickedRow;
      // _notifier.ProjectName = _selectedCard.ProjectName;
      LogEditor.Text += $"[{_selectedRow.CharaName}-{_selectedRow.MaterialName}]が選択されました\n";
    }
  }

  private async void OnRowDoubleClicked(object sender, ProjectInfoEventArgs e)
  {
    /***
    if (sender is ProjectInfoCard clickedCard)
    {
      // 前のカードの選択を解除
      if (_selectedCard != null && _selectedCard != clickedCard)
        _selectedCard.IsSelected = false;

      // 今回のカードを選択
      clickedCard.IsSelected = true;
      _selectedCard = clickedCard;

      _notifier.ProjectName = _selectedCard.ProjectName;
      _notifier.ProjectId = _selectedCard.ProjectId;
      LogEditor.Text += $"Status [{_selectedCard.ProjectName} id={_selectedCard.ProjectId}]が選択されました\n";
      LogEditor.Text += $"Project [{_selectedCard.ProjectName}]が選択されました\n";
    }

    await Shell.Current.GoToAsync(
        $"ProjectDetailPage?ProjectId={_selectedCard!.ProjectId}"
    );
    ***/
  }
}