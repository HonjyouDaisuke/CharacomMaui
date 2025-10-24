using CharacomMaui.Application.UseCases;
using CharacomMaui.Infrastructure.Services;
using CharacomMaui.Presentation.Dialogs;
using CharacomMaui.Presentation.ViewModels;
using CommunityToolkit.Maui.Extensions;
using System.Collections.ObjectModel;
namespace CharacomMaui.Presentation.Pages;

public partial class HomePage : ContentPage
{
    int count = 0;
    private const string BOX_ACCESS_TOKEN = "box_access_token";


    private readonly LoginToBoxUseCase _loginUseCase;
    private readonly BoxApiAuthService _boxApiAuthService;
    private readonly GetBoxConfigUseCase _getBoxConfigUseCase;
    private readonly BoxFolderViewModel _boxFolderViewModel;
    private readonly BoxLoginViewModel _boxLoginViewModel;

    private const string RootFolderId = "303046914186";
    private const string TEST_FOLDER_ID = "303046914186";
    public ObservableCollection<BoxItemViewModel> Files { get; } = new();
    public ObservableCollection<BoxImageItemViewModel> Files2 { get; set; } = new();

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
            BindingContext = _boxFolderViewModel;
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
        await DisplayAlert("Info", $"プラットフォーム：{GetPlatfrom()}", "OK");
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

            var accessToken = Preferences.Get(BOX_ACCESS_TOKEN, string.Empty);
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

        // Popup からのキャンセル通知を受け取る
        popup.CancelRequested += async () =>
        {
            //DisplayAlert("Cancel", "Cancel clecked", "OK");
            await this.ClosePopupAsync();       // ダウンロード処理をキャンセル
        };
        _ = this.ShowPopupAsync(popup);

        var progressHandler = new Progress<double>(p =>
          {
              popup.Dispatcher.Dispatch(() => popup.UpdateProgress(p));
          });

        try
        {
            var accessToken = Preferences.Get(BOX_ACCESS_TOKEN, string.Empty);
            // int fileCount = await _boxFolderViewModel.GetFolderItemCountAsync(accessToken, TEST_FOLDER_ID);
            await _boxFolderViewModel.LoadImageItemsAsync(accessToken, TEST_FOLDER_ID, progressHandler, cts.Token);
            await this.ClosePopupAsync();
            //await addFiles();
            //await DisplayAlert("FileCout", $"フォルダ内のファイル数は{fileCount}個です。", "OK");
        }
        catch (OperationCanceledException)
        {
            // キャンセル時の処理
            await DisplayAlert("Info", $"キャンセルされました", "OK");
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

