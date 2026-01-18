using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Presentation.ViewModels;
using CharacomMaui.Presentation.Components;
using CharacomMaui.Presentation.Models;
using CharacomMaui.Presentation.Dialogs;
using CharacomMaui.Presentation.Services;
using CharacomMaui.Application.Sessions;
using UraniumUI.Dialogs;
using UraniumUI.Dialogs.Mopups;
using CommunityToolkit.Maui.Extensions;

namespace CharacomMaui.Presentation.Pages;

public partial class UserListPage : ContentPage
{
  private readonly UserListViewModel _viewModel;
  private readonly IDialogService _dialogService;
  private readonly UserRolesSession _userRolesSession;
  private readonly UpdateUserRoleUseCase _updateUserRoleUseCase;
  private UserInfoRow? _selectedRow;
  private bool _isInitailized = false;
  public UserListPage(UserListViewModel viewModel, IDialogService dialogService, UserRolesSession userRolesSession, UpdateUserRoleUseCase updateUserRoleUseCase)
  {
    InitializeComponent();
    _viewModel = viewModel;
    _dialogService = dialogService;
    _userRolesSession = userRolesSession;
    _updateUserRoleUseCase = updateUserRoleUseCase;
    BindingContext = _viewModel;
    // デバッグ用：セットされたか確認
    System.Diagnostics.Debug.WriteLine($"BindingContext is set: {BindingContext != null}");
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();
    if (_isInitailized) return;

    _isInitailized = true;

    if (_viewModel == null)
    {
      LogEditor.Text += "Error: ViewModel is null\n";
      return;
    }

    LogEditor.Text += "Fetching user list...\n";
    await _viewModel.FetchUserListAsync();

    // UI側の Count Label が反応するか、ViewModel 経由で直接確認
    LogEditor.Text += $"ViewModel Users Count: {_viewModel.Users.Count}\n";

    foreach (var user in _viewModel.Users)
    {
      LogEditor.Text += $"User: {user.Name}\n"; //
    }
  }
  private void OnRowClicked(object sender, UserInfoRowEventArgs e)
  {
    System.Diagnostics.Debug.WriteLine("クリックされました");
    SelectRow(sender);
  }

  private void SelectRow(object sender)
  {
    if (sender is not UserInfoRow clickedRow)
      return;

    if (BindingContext is not UserListViewModel vm)
      return;

    // Row とバインドされている「UserInfoSummary」を取得
    if (clickedRow.BindingContext is not UserInfoSummary user)
      return;

    // 1) 全ての行を未選択にする
    foreach (var item in vm.Users)
    {
      item.IsSelected = false;
    }

    // 2) 今回クリックした行だけ選択
    user.IsSelected = true;

    // 3) 選択情報を更新
    _selectedRow = clickedRow;

    LogEditor.Text += $"[{user.Name}-{user.Id}]が選択されました\n";
  }

  private async void OnRowDoubleClicked(object sender, UserInfoRowEventArgs e)
  {
     var userInfo = _viewModel.GetUserInfoSummaryById(e.UserId);
    if (userInfo == null) return;
    
    var dialog = new UserRoleEditDialog("ユーザーロールの編集", userInfo, _dialogService, _userRolesSession);

    var result = await this.ShowPopupAsync(dialog);
    if (dialog.IsCanceled)
    {
      await SnackBarService.Warning("ユーザー権限の編集中をキャンセルしました。");
      return;
    }
    var userRoleId = _userRolesSession.GetRoleIdFromRoleName(dialog.SelectedRole);
    if (!await _viewModel.UpdateUserRoleAsync(dialog.UserId, userRoleId))
    {
      await SnackBarService.Error("ユーザー権限の編集中にエラーが発生しました。");
      return;
    }
    await SnackBarService.Success("ユーザー権限の変更を保存しました。");
  }

}