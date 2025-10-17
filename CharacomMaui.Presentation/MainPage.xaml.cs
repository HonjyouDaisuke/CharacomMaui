using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Text.Json;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Infrastructure.Services;
using CharacomMaui.Presentation.ViewModels;

namespace CharacomMaui.Presentation;

public partial class MainPage : ContentPage
{
  int count = 0;
  private readonly LoginToBoxUseCase _loginUseCase;
  private readonly BoxApiAuthService _boxApiAuthService;
  private readonly GetBoxConfigUseCase _getBoxConfigUseCase;
  private readonly LoginViewModel _loginViewModel;
  private const string RootFolderId = "303046914186";
  public ObservableCollection<BoxItemViewModel> Files { get; } = new();

  public MainPage(GetBoxConfigUseCase getBoxConfigUseCase,
                  LoginToBoxUseCase loginUseCase,
                  LoginViewModel loginViewModel)
  {
    try
    {
      InitializeComponent();

      _getBoxConfigUseCase = getBoxConfigUseCase;
      _loginUseCase = loginUseCase;
      //_boxApiAuthService = boxApiAuthService;
      _loginViewModel = loginViewModel;
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
    await _loginViewModel.LoginAsync();

    StatusLabel.Text = "ログイン成功！";
  }

  private async void OnListFilesClicked(object sender, EventArgs e)
  {
    try
    {
      StatusLabel.Text = "取得中...";
      await FetchAndDisplayFolderItemsAsync(RootFolderId);
    }
    catch (Exception ex)
    {
      StatusLabel.Text = $"取得失敗: {ex.Message}";
    }
  }

  private async Task FetchAndDisplayFolderItemsAsync(string folderId)
  {
    // アクセストークンを Preferences から取得
    var accessToken = Preferences.Get("box_access_token", string.Empty);
    if (string.IsNullOrEmpty(accessToken))
    {
      StatusLabel.Text = "アクセストークンがありません。先にログインしてください。";
      return;
    }

    using var client = new HttpClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    // 取得するフィールドを必要に応じて拡張できます
    var requestUrl = $"https://api.box.com/2.0/folders/{folderId}/items?fields=id,name,size,type,modified_at";
    var response = await client.GetAsync(requestUrl);

    if (!response.IsSuccessStatusCode)
    {
      // 401 等ならトークン期限切れの可能性（将来的に RefreshToken ロジックを追加）
      var body = await response.Content.ReadAsStringAsync();
      throw new Exception($"Box API error: {(int)response.StatusCode} {response.ReasonPhrase} - {body}");
    }

    var json = await response.Content.ReadAsStringAsync();
    using var doc = JsonDocument.Parse(json);
    var root = doc.RootElement;

    if (!root.TryGetProperty("entries", out var entries))
    {
      StatusLabel.Text = "entries が見つかりません";
      return;
    }

    Files.Clear();
    foreach (var entry in entries.EnumerateArray())
    {
      var id = entry.GetProperty("id").GetString() ?? "";
      var name = entry.GetProperty("name").GetString() ?? "";
      var type = entry.GetProperty("type").GetString() ?? "";

      Files.Add(new BoxItemViewModel { Id = id, Name = name, Type = type });
    }

    StatusLabel.Text = $"一覧取得完了: {Files.Count} 件";
  }

  // 簡易表示用VM
  public class BoxItemViewModel
  {
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
  }
}

