using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Infrastructure.Services;
using CharacomMaui.Presentation.ViewModels;
using System.Collections.ObjectModel;

namespace CharacomMaui.Presentation.Pages;

public partial class HomePage : ContentPage
{
  int count = 0;
  private readonly LoginToBoxUseCase _loginUseCase;
  private readonly BoxApiAuthService _boxApiAuthService;
  private readonly GetBoxConfigUseCase _getBoxConfigUseCase;
  private readonly BoxFolderViewModel _boxFolderViewModel;
  private readonly BoxLoginViewModel _boxLoginViewModel;

  private const string RootFolderId = "303046914186";
  public ObservableCollection<BoxItemViewModel> Files { get; } = new();
  public ObservableCollection<BoxImageItemViewModel> Files2 { get; } = new();

  public HomePage(GetBoxConfigUseCase getBoxConfigUseCase,
                  LoginToBoxUseCase loginUseCase,
                  BoxFolderViewModel boxFolderViewModel,
                  BoxLoginViewModel boxLoginViewModel)
  {
    try
    {
      InitializeComponent();

      _getBoxConfigUseCase = getBoxConfigUseCase;
      _loginUseCase = loginUseCase;
      _boxFolderViewModel = boxFolderViewModel;
      //_boxApiAuthService = boxApiAuthService;
      _boxLoginViewModel = boxLoginViewModel;
      FilesCollection.ItemsSource = Files;
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[MainPage ctor] {ex}");
      throw;
    }
  }

  private void OnCounterClicked(object? sender, EventArgs e)
  {
    count++;

    if (count == 1)
      CounterBtn.Text = $"Clicked {count} time";
    else
      CounterBtn.Text = $"Clicked {count} times";

    SemanticScreenReader.Announce(CounterBtn.Text);
  }
  private async void OnConfigClicked(object sender, EventArgs e)
  {

    try
    {
      var (clientId, clientSecret) = await _getBoxConfigUseCase.ExecuteAsync();
      await DisplayAlert("取得成功", $"ClientId: {clientId}\nSecret: {clientSecret}", "OK");
    }
    catch (Exception ex)
    {
      await DisplayAlert("エラー", ex.Message, "OK");
    }
  }

  private async void OnLoginClicked(object sender, EventArgs e)
  {

    StatusLabel.Text = "ログイン処理を開始...";
    await _boxLoginViewModel.LoginAsync();

    StatusLabel.Text = "ログイン成功！";
  }

  private async void OnListFilesClicked(object sender, EventArgs e)
  {
    try
    {
      StatusLabel.Text = "取得中...";

      var accessToken = Preferences.Get("box_access_token", string.Empty);
      StatusLabel.Text = $"AccessToken = {accessToken}";
      if (string.IsNullOrEmpty(accessToken))
      {
        StatusLabel.Text = "アクセストークンがありません。先にログインしてください。";
        return;
      }

      // await _boxFolderViewModel.LoadFolderItemsAsync(accessToken);
      await _boxFolderViewModel.LoadImageItemsAsync(accessToken, "303046914186");
      addFiles();
      StatusLabel.Text = $"一覧取得完了できたぜ！: {_boxFolderViewModel.Files2.Count} 件";
    }
    catch (Exception ex)
    {
      StatusLabel.Text += $"取得失敗だぜ : {ex.Message}";
    }

  }

  private void addFiles()
  {
    Files.Clear();
    foreach (var entry in _boxFolderViewModel.Files)
    {
      BoxItem item = new BoxItem
      {
        Id = entry.Id ?? "",
        Name = entry.Name ?? "",
        Type = entry.Type ?? ""
      };

      Files.Add(new BoxItemViewModel(item));
    }
  }
}

