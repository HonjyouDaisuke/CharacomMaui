using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Presentation.Dialogs;
using CharacomMaui.Presentation.ViewModels;
using CommunityToolkit.Maui.Extensions;
using System.Collections.ObjectModel;
namespace CharacomMaui.Presentation.Pages;

public partial class HomePage : ContentPage
{
  int count = 0;
  private const string BOX_ACCESS_TOKEN = "box_access_token";


  private readonly GetBoxConfigUseCase _getBoxConfigUseCase;
  private readonly BoxFolderViewModel _boxFolderViewModel;
  private readonly BoxLoginViewModel _boxLoginViewModel;
  private readonly IAppTokenStorageService _tokenStorage;

  private const string RootFolderId = "303046914186";
  private const string TEST_FOLDER_ID = "303046914186";
  public ObservableCollection<BoxItemViewModel> Files { get; } = new();
  public ObservableCollection<BoxImageItemViewModel> Files2 { get; set; } = new();

  public HomePage(GetBoxConfigUseCase getBoxConfigUseCase,
                  BoxFolderViewModel boxFolderViewModel,
                  BoxLoginViewModel boxLoginViewModel,
                  IAppTokenStorageService tokenStorage)
  {
    try
    {
      InitializeComponent();

      _getBoxConfigUseCase = getBoxConfigUseCase;
      _boxFolderViewModel = boxFolderViewModel;
      _boxLoginViewModel = boxLoginViewModel;
      BindingContext = _boxFolderViewModel;
      _tokenStorage = tokenStorage;
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
  private async void OnPlatformClicked(object sender, EventArgs e)
  {
    System.Diagnostics.Debug.WriteLine("OnStart Clicked");
    Console.WriteLine("Console.Debug");
    await DisplayAlert("Info", $"プラットフォーム：{GetPlatfrom()}", "OK");
  }
  private async void OnLoginClicked(object sender, EventArgs e)
  {

    StatusLabel.Text = "ログイン処理を開始...";
    await _boxLoginViewModel.LoginAsync();
    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    System.Diagnostics.Debug.WriteLine("ユーザ情報取得開始");
    await _boxLoginViewModel.GetUserInfoAsync(accessToken);
    StatusLabel.Text = "ログイン成功！";
  }

  private async void OnListFilesClicked(object sender, EventArgs e)
  {
    try
    {
      StatusLabel.Text = "取得中...";

      var tokens = await _tokenStorage.GetTokensAsync();
      var accessToken = tokens?.AccessToken;
      StatusLabel.Text = $"AccessToken = {accessToken}";
      if (string.IsNullOrEmpty(accessToken))
      {
        StatusLabel.Text = "アクセストークンがありません。先にログインしてください。";
        return;
      }

      // await _boxFolderViewModel.LoadFolderItemsAsync(accessToken);
      await _boxFolderViewModel.LoadImageItemsAsync(accessToken, TEST_FOLDER_ID);
      //addFiles();
      StatusLabel.Text = $"一覧取得完了できたぜ！: {_boxFolderViewModel.Files2.Count} 件";
    }
    catch (Exception ex)
    {
      StatusLabel.Text += $"取得失敗だぜ : {ex.Message}";
    }

  }

  private async void OnStartDownloadClicked(object sender, EventArgs e)
  {
    var cts = new CancellationTokenSource();
    var popup = new DownloadingDialog();

    // (中略: CancelRequested の設定)

    _ = this.ShowPopupAsync(popup);
    await Task.Delay(100); // ⭐ ポップアップの描画を待つ (Mac対策)

    var progressHandler = new Progress<double>(p =>
      {
        popup.Dispatcher.Dispatch(() => popup.UpdateProgress(p));
      });

    try
    {
      var tokens = await _tokenStorage.GetTokensAsync();
      var accessToken = tokens?.AccessToken;

      // 1. ダウンロードとViewModelへのアイテム追加（画像セットは含まない）
      await _boxFolderViewModel.LoadImageItemsAsync(accessToken, TEST_FOLDER_ID, progressHandler, cts.Token);

      // 2. ⭐ 画像ロード処理を順次、制御しながら実行する
      foreach (var item in _boxFolderViewModel.Files2)
      {
        if (cts.Token.IsCancellationRequested) break;

        // 画像ロード（Task.Run でデコード、MainThread.InvokeOnMainThreadAsync でUI更新）
        await item.LoadImageAsync();

        // ⭐ 極めて重要: UIスレッドの負荷を下げるため、各画像ロードの間に短い待ち時間を設ける
        // Mac Catalystではこの短い遅延がフリーズ防止に非常に効果的です
        await Task.Delay(50, cts.Token);
      }

      // 3. 全ての処理が完了した後、ポップアップを閉じる
      await this.ClosePopupAsync();
    }
    catch (OperationCanceledException)
    {
      // ... (中略: キャンセル時の処理)
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(ex.ToString());
      // ... (中略: エラー時の処理)
    }
  }

  /**
  private async Task addFiles()
  {
      var tempList = _boxFolderViewModel.Files2.Take(10)
              .Select(entry => new BoxImageItemViewModel(new BoxImageItem
              {
                  Id = entry.Id ?? "",
                  Name = entry.Name ?? "",
                  Type = entry.Type ?? "",
                  Image = entry.Image
              }))
              .ToList();

      MainThread.BeginInvokeOnMainThread(() =>
      {
          var backup = Files2;
          Files2 = null; // 一旦解除
          OnPropertyChanged(nameof(Files2));

          Files2 = new ObservableCollection<BoxImageItemViewModel>(tempList);
          OnPropertyChanged(nameof(Files2));
      });
      await DisplayAlert("Message", $"ファイル一覧を更新しました。{Files2.Count}", "OK");
  }
  **/
  private string GetPlatfrom()
  {
    var platform = DeviceInfo.Platform;
    if (platform == DevicePlatform.WinUI)
      return "Windows";
    else if (platform == DevicePlatform.Android)
      return "Android";
    else if (platform == DevicePlatform.MacCatalyst)
      return "MacCatalyst";
    else
      return "OtherDevice";
  }
}

