using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Presentation.Components;
using CharacomMaui.Presentation.Models;
using CharacomMaui.Presentation.ViewModels;
using CharacomMaui.Presentation.Dialogs;
using CharacomMaui.Presentation.Services;
using CommunityToolkit.Maui.Extensions;
using UraniumUI.Dialogs;
using UraniumUI.Dialogs.Mopups;
using MauiApp = Microsoft.Maui.Controls.Application;
using CharacomMaui.Presentation.Interfaces;

namespace CharacomMaui.Presentation.Pages;

public partial class ProjectDetailPage : ContentPage
{
  private CharaDataProgressRow? _selectedRow;
  private AppStatus _appStatus;
  private GetProjectCharaItemsUseCase _useCase;
  private ProjectDetailViewModel _viewModel;
  private CreateProjectViewModel _createViewModel;
  private IDialogService _dialogService;
  private ISimpleProgressDialogService _simpleDialog;

  public ProjectDetailPage(AppStatus appStatus,
  GetProjectCharaItemsUseCase useCase,
  CreateProjectViewModel createProjectViewModel,
  ProjectDetailViewModel viewModel,
  IDialogService dialogService,
  ISimpleProgressDialogService simpleDialog)
  {
    InitializeComponent();
    _appStatus = appStatus;
    _useCase = useCase;
    _dialogService = dialogService;
    _viewModel = viewModel;
    _createViewModel = createProjectViewModel;
    _simpleDialog = simpleDialog;
    BindingContext = _viewModel;

    ProjectDetailCard.UpdateRequested += OnUpdateRequestedAsync;
    ProjectDetailCard.DeleteRequested += OnDeleteRequestedAsync;
    ProjectDetailCard.InviteRequested += OnInviteRequestedAsync;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();
    try
    {
      if (string.IsNullOrWhiteSpace(_appStatus.ProjectId))
      {
        await SnackBarService.Error("ProjectIdが設定されていません");
        return;
      }
      await GetCharaItemAsync();
      await _viewModel.SetProjectDetailsAsync(_appStatus.ProjectId);
      LogEditor.Text += $"projectId = {_appStatus.ProjectId}\n";
      LogEditor.Text += $"ProjectName = {_appStatus.ProjectName}\n";
      LogEditor.Text += $"ProjectFolder = {_appStatus.ProjectFolderId}\n";
      LogEditor.Text += $"CharaFolder = {_appStatus.CharaFolderId}\n";
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"OnAppearing error: {ex.Message}");
      await SnackBarService.Error("ページの初期化中にエラーが発生しました。");
    }

    // 強制的に ItemsSource を再設定
    CharaDataCollection.ItemsSource = null;
    CharaDataCollection.ItemsSource = _viewModel.CharaDataSummaries;
  }

  private async Task GetCharaItemAsync()
  {

    await _viewModel.FetchCharaDataAsync(_appStatus.ProjectId);
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
      System.Diagnostics.Debug.WriteLine("ダブルクリックされました");
      SelectRow(sender); // 選択状態の更新は ViewModel/CharaDataSummary に委譲

      System.Diagnostics.Debug.WriteLine("Go!");
      await Shell.Current.GoToAsync("///CharaSelectPage");
    }
  }
  private Project MakeProjectFromEventArgs(ProjectInfoEventArgs e)
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
  private async Task OnUpdateRequestedAsync(ProjectInfoEventArgs e)
  {
    try
    {
      // var tcs = new TaskCompletionSource();
      System.Diagnostics.Debug.WriteLine($"編集要求: {e.ProjectName} (ID: {e.ProjectId}) 説明: {e.ProjectDescription} folder{e.ProjectFolderId} chara{e.CharaFolderId})\n");
      LogEditor.Text += $"編集: {e.ProjectName} (ID: {e.ProjectId}) 説明: {e.ProjectDescription} [ {e.ProjectFolderId} ][ {e.CharaFolderId} ]\n";
      LogEditor.Text += "プロジェクトの更新！！\n";

      var topFolderItems = await _createViewModel.GetFolderItemsAsync();
      var project = MakeProjectFromEventArgs(e);

      var dialog = new CreateProjectDialog("プロジェクトの更新", topFolderItems, _dialogService, _createViewModel, project);
      dialog.Closed += (_, __) =>
      {
        System.Diagnostics.Debug.WriteLine("Dialog Closed → unlock");
        this.ProjectDetailCard.NotifyActionCompleted();
      };

      await this.ShowPopupAsync(dialog);
      // await tcs.Task;
      if (dialog.IsCanceled)
      {
        LogEditor.Text += "キャンセルされました\n";
        await SnackBarService.Warning("更新がキャンセルされました。");
        return;
      }
      project.Name = dialog.ProjectName;
      project.Description = dialog.ProjectDescription;
      project.FolderId = dialog.SelectedTopFolder.Id;
      project.CharaFolderId = dialog.SelectedCharaFolder.Id;
      LogEditor.Text += $"Name: {project.Name}, Description: {project.Description}, Folder: {project.FolderId} CharaFolder: {project.CharaFolderId}\n";

      await _simpleDialog.ShowAsync("プロジェクトの更新", "プロジェクトを更新しています。少々お待ちください");

      var result = await _createViewModel.CreateOrUpdateProjectAsync(project);

      await _simpleDialog.CloseAsync();
      if (result.Success)
      {
        await SnackBarService.Success("プロジェクトを更新しました");
      }
      else
      {
        System.Diagnostics.Debug.WriteLine($"[Error]Project create or update error... {result.Message}");
        await SnackBarService.Error("プロジェクトの作成・更新中にエラーが発生しました。");
      }

    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[Error]Project create or update error... {ex.Message}");
      await SnackBarService.Error("プロジェクトの作成・更新中にエラーが発生しました。");
    }
  }
  private async Task OnDeleteRequestedAsync(ProjectInfoEventArgs e)
  {
    try
    {
      LogEditor.Text += $"削除: {e.ProjectName} (ID: {e.ProjectId})\n";
      var project = MakeProjectFromEventArgs(e);

      var dialog = new ConfirmDeleteDialog("プロジェクトの削除確認", _dialogService, project);
      await this.ShowPopupAsync(dialog);

      if (dialog.IsConfirmed)
      {
        await _simpleDialog.ShowAsync("プロジェクトの削除", "プロジェクトデータ削除中・・・しばらくお待ち下さい。");

        LogEditor.Text += $"削除が確認されました: {e.ProjectName} (ID: {e.ProjectId})\n";
        var result = await _createViewModel.DeleteProjectAsync(e.ProjectId);
        LogEditor.Text += $"削除結果: {result.Success}\n";

        await _simpleDialog.CloseAsync();
        if (!result.Success)
        {
          await SnackBarService.Error($"プロジェクトの削除に失敗しました。 プロジェクト名：{e.ProjectName}");
          return;
        }
        await SnackBarService.Success($"プロジェクトを削除しました。");
        await Shell.Current.GoToAsync("///ProjectListPage");

      }
      else
      {
        LogEditor.Text += $"削除がキャンセルされました: {e.ProjectName} (ID: {e.ProjectId})\n";
        await SnackBarService.Warning("削除がキャンセルされました。");
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"Delete error: {ex.Message}");
      await SnackBarService.Error($"プロジェクトの削除中に想定外のエラーが発生しました: {e.ProjectName}");
    }
  }
  private async Task OnInviteRequestedAsync(ProjectInfoEventArgs e)
  {
    LogEditor.Text += $"招待します.{e.ProjectName}\n";
  }
}