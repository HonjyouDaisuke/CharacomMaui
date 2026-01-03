using System.Runtime.InteropServices.Marshalling;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Presentation.Components;
using CharacomMaui.Presentation.Dialogs;
using CharacomMaui.Presentation.Models;
using CharacomMaui.Presentation.ViewModels;
using CharacomMaui.Presentation.Services;
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
    // _viewModel.SetUserStatus(_appStatusUseCase.GetAppStatus());
    BindableLayout.SetItemsSource(ProjectsFlex, projects);
    foreach (var project in projects)
    {
      System.Diagnostics.Debug.WriteLine($"Project: {project.Name} (ID: {project.Id}) FolderId: {project.FolderId} CharaFolderId: {project.CharaFolderId}");
    }
  }
  private Project makeProjectFromEventArgs(ProjectInfoEventArgs e)
  {
    return new Project
    {
      Id = e.ProjectId,
      Name = e.ProjectName,
      Description = e.ProjectDescription,
      FolderId = e.ProjectFolderId,
      CharaFolderId = e.CharaFolderId,
    };
  }

  private async void OnEditRequested(object? sender, ProjectInfoEventArgs e)
  {
    System.Diagnostics.Debug.WriteLine($"編集要求: {e.ProjectName} (ID: {e.ProjectId}) 説明: {e.ProjectDescription})\n");
    LogEditor.Text += $"編集: {e.ProjectName} (ID: {e.ProjectId}) 説明: {e.ProjectDescription} {e.ProjectFolderId} {e.CharaFolderId}\n";
    LogEditor.Text += "プロジェクトの更新！！\n";

    var topFolderItems = await _viewModel.GetFolderItemsAsync();
    var project = makeProjectFromEventArgs(e);

    var dialog = new CreateProjectDialog("プロジェクトの更新", topFolderItems, _dialogService, _viewModel, project);
    await this.ShowPopupAsync(dialog);

    // ダイアログから返ってきてから。。。
    if (dialog.SelectedTopFolder == null)
    {
      LogEditor.Text += "選択されていない";
      return;
    }
    if (dialog.SelectedCharaFolder == null)
    {
      LogEditor.Text += "個別文字フォルダが選択されていない";
      return;
    }
    project.Name = dialog.ProjectName;
    project.Description = dialog.ProjectDescription;
    project.FolderId = dialog.SelectedTopFolder.Id;
    project.CharaFolderId = dialog.SelectedCharaFolder.Id;
    LogEditor.Text += $"Name: {project.Name}, Description: {project.Description}, Folder: {project.FolderId} CharaFolder: {project.CharaFolderId}\n";

    // プロジェクトを更新
    using (await _dialogService.DisplayProgressAsync("プロジェクトの更新", "プロジェクトを更新中・・・\nしばらくお待ち下さい。"))
    {
      var updateResult = await _viewModel.CreateOrUpdateProjectAsync(project);
      if (!updateResult.Success)
      {
        LogEditor.Text += $"プロジェクトの更新に失敗しました。{updateResult.Message}\n";
        return;
      }
      LogEditor.Text += $"プロジェクトを更新しました。ProjectName={project.Name}";
    }
  }

  private async void OnDeleteRequested(object? sender, ProjectInfoEventArgs e)
  {
    LogEditor.Text += $"削除: {e.ProjectName} (ID: {e.ProjectId})\n";
    var project = makeProjectFromEventArgs(e);

    var dialog = new ConfirmDeleteDialog("プロジェクトの削除確認", _dialogService, project);
    await this.ShowPopupAsync(dialog);

    if (dialog.IsConfirmed)
    {
      using (await _dialogService.DisplayProgressAsync("プロジェクトの削除", "プロジェクトデータ削除中・・・\nしばらくお待ち下さい。"))
      {
        LogEditor.Text += $"削除が確認されました: {e.ProjectName} (ID: {e.ProjectId})\n";
        var result = await _viewModel.DeleteProjectAsync(e.ProjectId);
        LogEditor.Text += $"削除結果: {result.Success}\n";
        if (!result.Success)
        {
          await _dialogService.DisplayTextPromptAsync("削除エラー", result.Message, "OK");
          return;
        }
        // プロジェクトリストを再取得
        var projects = await _viewModel.GetProjectsAsync();

        if (projects == null) return;
        BindableLayout.SetItemsSource(ProjectsFlex, projects);
      }
    }
    else
    {
      LogEditor.Text += $"削除がキャンセルされました: {e.ProjectName} (ID: {e.ProjectId})\n";
    }
  }

  private void OnInviteRequested(object? sender, ProjectInfoEventArgs e)
  {
    LogEditor.Text += $"招待: {e.ProjectName} (ID: {e.ProjectId})\n";

    // 例: 招待ダイアログ
    // await Navigation.PushAsync(new InvitePage(e.ProjectId));
  }

  private async void OnCreateProjectBtn(object? sender, EventArgs e)
  {
    LogEditor.Text += "プロジェクトの新規作成\n";

    var topFolderItems = await _viewModel.GetFolderItemsAsync();

    var dialog = new CreateProjectDialog("プロジェクトの新規作成", topFolderItems, _dialogService, _viewModel);
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

  private async Task ResultNotification(SimpleApiResult result, string target)
  {

    if (result.Success)
    {
      await SnackBarService.Success($"{target}を作成・更新しました。");
    }
    else
    {
      await SnackBarService.Error($"{target}の作成・更新に失敗しました。");
    }

  }
  private async void OnStrokeBtnClicked(object sender, EventArgs e)
  {
    SimpleApiResult result = new();
    using (await _dialogService.DisplayProgressAsync("筆順書体マスター", "筆順書体マスターを作成しています。少々お待ちください。"))
    {
      // Indicate a long running operation
      // エントリーの作成
      result = await _viewModel.UpdateStrokeAsync();
      System.Diagnostics.Debug.WriteLine(result.ToString());
      await ResultNotification(result, "筆順書体");

    }
  }

  // 標準画像の更新
  private async void OnStandardBtnClicked(object sender, EventArgs e)
  {
    SimpleApiResult result = new();
    using (await _dialogService.DisplayProgressAsync("標準書体マスター", "標準書体マスターを作成しています。少々お待ちください。"))
    {
      // Indicate a long running operation
      // エントリーの作成
      result = await _viewModel.UpdateStandardAsync();
      System.Diagnostics.Debug.WriteLine(result.ToString());
      await ResultNotification(result, "標準書体");
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
      _notifier.ProjectId = _selectedCard.ProjectId;
      _notifier.ProjectFolderId = _selectedCard.ProjectFolderId;
      _notifier.CharaFolderId = _selectedCard?.CharaFolderId;
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
      _notifier.ProjectFolderId = _selectedCard.ProjectFolderId;
      _notifier.CharaFolderId = _selectedCard?.CharaFolderId;

      LogEditor.Text += $"Status [{_selectedCard.ProjectName} id={_selectedCard.ProjectId}]が選択されました\n";
      LogEditor.Text += $"Project [{_selectedCard.ProjectName}]が選択されました\n";
    }

    await Shell.Current.GoToAsync(
        $"///ProjectDetailPage?ProjectId={_selectedCard!.ProjectId}"
    );
  }
}