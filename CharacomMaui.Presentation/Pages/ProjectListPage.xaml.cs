using System.Runtime.InteropServices.Marshalling;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Presentation.Components;
using CharacomMaui.Presentation.Dialogs;
using CharacomMaui.Presentation.Models;
using CharacomMaui.Presentation.ViewModels;
using CharacomMaui.Presentation.Services;
using CharacomMaui.Presentation.Interfaces;
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
  private readonly ISimpleProgressDialogService _simpleDialog;
  public ProjectListPage(IBoxFolderRepository repository,
    IDialogService dialogService,
    CreateProjectViewModel viewModel,
    AppStatusUseCase appStatusUseCase,
    AppStatusNotifier notifier,
    ISimpleProgressDialogService simpleDialog)
  {
    InitializeComponent();
    _repository = repository;
    _dialogService = dialogService;
    _appStatusUseCase = appStatusUseCase;
    _viewModel = viewModel;
    _notifier = notifier;
    _simpleDialog = simpleDialog;
    _viewModel.SetUserStatus(_appStatusUseCase.GetAppStatus());
    System.Diagnostics.Debug.WriteLine($"status = {_viewModel._appStatus.ToString()}");
    BindingContext = _viewModel;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();
    var projects = await _viewModel.GetProjectsAsync();
    if (projects == null) return;
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
    await _simpleDialog.ShowAsync("プロジェクトの更新", "プロジェクトを更新中・・・\nしばらくお待ち下さい。");
    try
    {
      var updateResult = await _viewModel.CreateOrUpdateProjectAsync(project);
      if (updateResult.Success)
      {
        await SnackBarService.Success("プロジェクトを更新しました");
      }
      else
      {
        System.Diagnostics.Debug.WriteLine($"[Error]Project create or update error... {updateResult.Message}");
        await SnackBarService.Error("プロジェクトの作成・更新中にエラーが発生しました。");
      }
    }
    finally
    {
      await Task.Delay(100);
      await _simpleDialog.CloseAsync();
    }
  }

  private async void OnDeleteRequested(object? sender, ProjectInfoEventArgs e)
  {
    LogEditor.Text += $"削除: {e.ProjectName} (ID: {e.ProjectId})\n";
    var project = makeProjectFromEventArgs(e);

    var dialog = new ConfirmDeleteDialog("プロジェクトの削除確認", _dialogService, project);
    await this.ShowPopupAsync(dialog);

    if (!dialog.IsConfirmed)
    {
      LogEditor.Text += $"削除がキャンセルされました: {e.ProjectName} (ID: {e.ProjectId})\n";
      await SnackBarService.Warning($"削除がキャンセルされました: {e.ProjectName} (ID: {e.ProjectId})");
      return;
    }

    await _simpleDialog.ShowAsync("プロジェクトの削除", "プロジェクトを削除中・・・\nしばらくお待ち下さい。");

    try
    {
      var result = await _viewModel.DeleteProjectAsync(e.ProjectId);
      if (!result.Success)
      {
        await SnackBarService.Error($"削除中にエラーが発生しました。: {e.ProjectName} (ID: {e.ProjectId})");
      }
      // プロジェクトリストを再取得
      var projects = await _viewModel.GetProjectsAsync();

      if (projects == null)
      {
        await _simpleDialog.CloseAsync();
        return;
      }
      BindableLayout.SetItemsSource(ProjectsFlex, projects);
    }
    finally
    {
      await Task.Delay(100);
      await _simpleDialog.CloseAsync();
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

    Project project = new()
    {
      Name = dialog.ProjectName,
      Description = dialog.ProjectDescription,
      FolderId = dialog.SelectedTopFolder.Id,
      CharaFolderId = dialog.SelectedCharaFolder.Id
    };
    LogEditor.Text += $"Name: {project.Name}, Description: {project.Description}, Folder: {project.FolderId} CharaFolder: {project.CharaFolderId}\n";

    await _simpleDialog.ShowAsync("プロジェクトの作成", "プロジェクトを作成しています。少々お待ちください");
    // ダイアログ表示待ち
    await Task.Delay(1000);
    try
    {
      var result = await _viewModel.CreateOrUpdateProjectAsync(project);
      if (result.Success)
      {
        var projects = await _viewModel.GetProjectsAsync();
        BindableLayout.SetItemsSource(ProjectsFlex, projects);

        await SnackBarService.Success("プロジェクトを作成しました");
      }
      else
      {
        System.Diagnostics.Debug.WriteLine($"[Error]Project create or update error... {result.Message}");
        await SnackBarService.Error("プロジェクトの作成中にエラーが発生しました。");
      }
    }
    finally
    {
      System.Diagnostics.Debug.WriteLine("KOKO怪しい");
      await _simpleDialog.CloseAsync();
    }

    //LogEditor.Text += $"Name: {projectName}, Description: {projectDescription}, Folder: {selectedFolder.Name} CharaFolder: {selectedCharaFolder}\n";
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
    await _simpleDialog.ShowAsync("筆順書体マスター作成中", "筆順書体マスターを作成しています。少々お待ちください。");
    var result = await _viewModel.UpdateStrokeAsync();
    System.Diagnostics.Debug.WriteLine(result.ToString());
    await _simpleDialog.CloseAsync();
    await ResultNotification(result, "筆順書体");
  }

  // 標準画像の更新
  private async void OnStandardBtnClicked(object sender, EventArgs e)
  {
    await _simpleDialog.ShowAsync("標準書体マスター作成中", "標準書体マスターを作成しています。少々お待ちください。");

    var result = await _viewModel.UpdateStandardAsync();
    System.Diagnostics.Debug.WriteLine(result.ToString());
    await Task.Delay(100);
    await _simpleDialog.CloseAsync();
    await Task.Delay(100);
    await ResultNotification(result, "標準書体");
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