using CharacomMaui.Application.UseCases;
using CharacomMaui.Presentation.ViewModels;

namespace CharacomMaui.Presentation.Pages;

public partial class UserListPage : ContentPage
{
  private readonly UserListViewModel _viewModel;

  public UserListPage(UserListViewModel viewModel)
  {
    InitializeComponent();
    _viewModel = viewModel;
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


}