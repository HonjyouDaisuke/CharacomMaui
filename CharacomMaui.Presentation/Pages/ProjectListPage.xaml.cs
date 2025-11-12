using System.Threading.Tasks;
using Box.Sdk.Gen.Schemas;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Infrastructure.Services;
using CharacomMaui.Presentation.Components;
using CharacomMaui.Presentation.Dialogs;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using UraniumUI.Dialogs;
using UraniumUI.Dialogs.Mopups;

namespace CharacomMaui.Presentation.Pages;

public partial class ProjectListPage : ContentPage
{
  private readonly IBoxFolderRepository _repository;
  private readonly IDialogService _dialogService;
  private readonly CreateProjectViewModel _viewModel;
  public ProjectListPage(IBoxFolderRepository repository, IDialogService dialogService, CreateProjectViewModel viewModel)
  {
    InitializeComponent();
    _repository = repository;
    _dialogService = dialogService;
    _viewModel = viewModel;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();
    var projects = await _viewModel.GetProjectsAsync();
    if (projects == null) return;

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
}