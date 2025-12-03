using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Presentation.Components;
using CharacomMaui.Presentation.Dialogs;
using CharacomMaui.Presentation.Models;
using CharacomMaui.Presentation.ViewModels;
using CommunityToolkit.Maui.Extensions;
using UraniumUI.Dialogs;
using UraniumUI.Dialogs.Mopups;

namespace CharacomMaui.Presentation.Pages;

public partial class ProjectListPage : ContentPage
{
  private ProjectInfoCard? _selectedCard;

  private readonly IBoxFolderRepository _repository;
  private readonly IDialogService _dialogService;
  private readonly CreateProjectViewModel _viewModel;
  private readonly AppStatusUseCase _appStatusUseCase;
  private readonly AppStatusNotifier _notifier;
  public ProjectListPage(IBoxFolderRepository repository, IDialogService dialogService, CreateProjectViewModel viewModel, AppStatusUseCase appStatusUseCase, AppStatusNotifier notifier)
  {
    InitializeComponent();
    _repository = repository;
    _dialogService = dialogService;
    _appStatusUseCase = appStatusUseCase;
    _viewModel = viewModel;
    _notifier = notifier;
    _viewModel.SetUserStatus(_appStatusUseCase.GetAppStatus());
    System.Diagnostics.Debug.WriteLine($"status = {_viewModel._appStatus.ToString()}");
    BindingContext = _viewModel;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();
    var projects = await _viewModel.GetProjectsAsync();
    if (projects == null) return;
    // TODO:いる？
    _viewModel.SetUserStatus(_appStatusUseCase.GetAppStatus());
    ProjectsCollection.ItemsSource = projects;
    foreach (var project in projects)
    {
      var card = new ProjectInfoCard
      {
        ProjectName = project.Name,
        CharaCount = project.CharaCount,
        UserCount = project.UserCount
      };

      //ProjectsContainer.Children.Add(card);

      LogEditor.Text += $"projectName = {project.Name}\n";
      LogEditor.Text += $"projectDescription = {project.Description}\n";
      LogEditor.Text += $"UserCount = {project.UserCount}\n";
      LogEditor.Text += $"CharaCount = {project.CharaCount}\n";
      LogEditor.Text += "---------------------------------\n";
    }
  }
  private async void OnDialogButtonClicked(object sender, EventArgs e)
  {
    var result = await this.DisplayRadioButtonPromptAsync(
            "Pick some of them below",
            new[] { "Option 1", "Option 2", "Option 3" });
  }
  private async void OnCreateProjectBtn(object? sender, EventArgs e)
  {
    LogEditor.Text += "プロジェクトの新規作成\n";
    // var accessToken = Preferences.Get("app_access_token", string.Empty);
    var topFolderItems = await _viewModel.GetFolderItemsAsync();

    var dialog = new CreateProjectDialog(topFolderItems, _dialogService, _viewModel);
    await this.ShowPopupAsync(dialog);
    if (dialog.SelectedTopFolder == null)
    {
      LogEditor.Text += "選択されていない";
      return;
    }

    var projectName = dialog.ProjectName;
    var projectDescription = dialog.ProjectDescription;
    var selectedFolder = dialog.SelectedTopFolder;
    var selectedCharaFolder = dialog.SelectedCharaFolder;

    LogEditor.Text += $"Name: {projectName}, Description: {projectDescription}, Folder: {selectedFolder.Name} CharaFolder: {selectedCharaFolder}\n";
  }

  private async void OnStrokeBtnClicked(object sender, EventArgs e)
  {
    using (await _dialogService.DisplayProgressAsync("筆順書体マスター", "筆順書体マスターを作成しています。少々お待ちください。"))
    {
      // Indicate a long running operation
      // エントリーの作成
      var res = await _viewModel.UpdateStrokeAsync();
      System.Diagnostics.Debug.WriteLine(res.ToString());
    }
  }

  // 標準画像の更新
  private async void OnStandardBtnClicked(object sender, EventArgs e)
  {
    using (await _dialogService.DisplayProgressAsync("標準書体マスター", "標準書体マスターを作成しています。少々お待ちください。"))
    {
      // Indicate a long running operation
      // エントリーの作成
      var res = await _viewModel.UpdateStandardAsync();
      System.Diagnostics.Debug.WriteLine(res.ToString());
    }
  }
  private void OnCardClicked(object sender, ProjectInfoEventArgs e)
  {
    if (sender is ProjectInfoCard clickedCard)
    {
      // 前のカードの選択を解除
      if (_selectedCard != null && _selectedCard != clickedCard)
        _selectedCard.IsSelected = false;

      // 今回のカードを選択
      clickedCard.IsSelected = true;
      _selectedCard = clickedCard;
      _notifier.ProjectName = _selectedCard.ProjectName;
      LogEditor.Text += $"Project [{_selectedCard.ProjectName}]が選択されました\n";
    }

  }

  private async void OnCardDoubleClicked(object sender, ProjectInfoEventArgs e)
  {
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
        $"///ProjectDetailPage?ProjectId={_selectedCard!.ProjectId}"
    );
  }
}