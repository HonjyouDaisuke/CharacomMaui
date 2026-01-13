using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Presentation.ViewModels;
using CharacomMaui.Presentation.Components;
using CharacomMaui.Presentation.Models;
using CharacomMaui.Presentation.Dialogs;
using UraniumUI.Dialogs;
using UraniumUI.Dialogs.Mopups;
using CommunityToolkit.Maui.Extensions;

namespace CharacomMaui.Presentation.Pages;

public partial class UserListPage : ContentPage
{
  private readonly UserListViewModel _viewModel;
  private readonly IDialogService _dialogService;
  private UserInfoRow? _selectedRow;
  public UserListPage(UserListViewModel viewModel, IDialogService dialogService)
  {
    InitializeComponent();
    _viewModel = viewModel;
    _dialogService = dialogService;
    BindingContext = _viewModel;
    // デバッグ用：セットされたか確認
    System.Diagnostics.Debug.WriteLine($"BindingContext is set: {BindingContext != null}");
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();
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
    LogEditor.Text += "ダブルクリックされました\n";
    var userInfo = _viewModel.GetUserInfoSummaryById(e.UserId);
    if (userInfo == null) return;
    LogEditor.Text += $"ユーザー情報取得: {userInfo.Name} ({userInfo.Id})\n";
    var dialog = new UserRoleEditDialog("ユーザーロールの編集", userInfo, _dialogService);
    await this.ShowPopupAsync(dialog);
  }

}