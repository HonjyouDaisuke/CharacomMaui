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

namespace CharacomMaui.Presentation.Pages;

public partial class ProjectDetailPage : ContentPage
{
  private CharaDataProgressRow? _selectedRow;
  private AppStatus _appStatus;
  private GetProjectCharaItemsUseCase _useCase;
  private ProjectDetailViewModel _viewModel;
  private CreateProjectViewModel _createViewModel;
  private IDialogService _dialogService;
  public ProjectDetailPage(AppStatus appStatus, GetProjectCharaItemsUseCase useCase, CreateProjectViewModel createProjectViewModel, ProjectDetailViewModel viewModel, IDialogService dialogService)
  {
    InitializeComponent();
    _appStatus = appStatus;
    _useCase = useCase;
    _dialogService = dialogService;
    _viewModel = viewModel;
    _createViewModel = createProjectViewModel;
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
        return;
      }
      project.Name = dialog.ProjectName;
      project.Description = dialog.ProjectDescription;
      project.FolderId = dialog.SelectedTopFolder.Id;
      project.CharaFolderId = dialog.SelectedCharaFolder.Id;
      LogEditor.Text += $"Name: {project.Name}, Description: {project.Description}, Folder: {project.FolderId} CharaFolder: {project.CharaFolderId}\n";

      using (await _dialogService.DisplayProgressAsync(
               "プロジェクト更新", "更新中..."))
      {
        var result = await _createViewModel.CreateOrUpdateProjectAsync(project);
        if (result.Success)
        {
          await _viewModel.SetProjectDetailsAsync(_appStatus.ProjectId);
          await SnackBarService.Success($"プロジェクトを更新しました。 プロジェクト名：{project.Name}");
        }
        else
        {
          await SnackBarService.Error($"プロジェクト更新中にエラーが発生しました。 プロジェクト名：{project.Name}");
        }
      }

      LogEditor.Text += "プロジェクトを更新しました\n";

    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"Update error: {ex.Message}");
      await SnackBarService.Error($"プロジェクトの更新中に想定外のエラーが発生しました");
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
        using (await _dialogService.DisplayProgressAsync("プロジェクトの削除", "プロジェクトデータ削除中・・・\nしばらくお待ち下さい。"))
        {
          LogEditor.Text += $"削除が確認されました: {e.ProjectName} (ID: {e.ProjectId})\n";
          var result = await _createViewModel.DeleteProjectAsync(e.ProjectId);
          LogEditor.Text += $"削除結果: {result.Success}\n";
          if (!result.Success)
          {
            await SnackBarService.Error($"プロジェクトの削除に失敗しました。 プロジェクト名：{e.ProjectName}");
            return;
          }

          await Shell.Current.GoToAsync("///ProjectListPage");
        }
      }
      else
      {
        LogEditor.Text += $"削除がキャンセルされました: {e.ProjectName} (ID: {e.ProjectId})\n";
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