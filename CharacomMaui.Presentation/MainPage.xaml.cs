using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Text.Json;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Infrastructure.Services;

namespace CharacomMaui.Presentation;

public partial class MainPage : ContentPage
{
  int count = 0;
  private readonly LoginToBoxUseCase _loginUseCase;
  private const string RootFolderId = "303046914186";
  public ObservableCollection<BoxItemViewModel> Files { get; } = new();


  public MainPage()
  {
    InitializeComponent();
    // 👇 この2行を実際の値に置き換える
    string clientId = "xt52jorsw8fzbit06h1rbciwl96cywe1";
    string clientSecret = "BQiaeKEhaNY0yn33R4oiEAyyWtswcYCT";

    var authService = new BoxApiAuthService(clientId, clientSecret);
    _loginUseCase = new LoginToBoxUseCase(authService);
    FilesCollection.ItemsSource = Files;
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

  private async void OnLoginClicked(object sender, EventArgs e)
  {
    StatusLabel.Text += "start";
    try
    {
      StatusLabel.Text = "認証画面を開いています...";
      // 認可URL取得
      var authUrl = _loginUseCase.GetAuthorizationUrl();
      StatusLabel.Text += "Login step 1";

      // MAUIのWebAuthenticatorを使ってOAuth認証画面を開く
      var callbackUrl = new Uri("myapp://callback"); // Boxで登録したredirect_uriと一致させる
      StatusLabel.Text += "Login step 2";
      var result = await WebAuthenticator.AuthenticateAsync(
          new Uri(authUrl),
          callbackUrl);
      await Shell.Current.DisplayAlert("Debug", $"Query: {result.Properties["code"]}", "OK");

      if (result?.Properties?.ContainsKey("code") == true)
      {
        string code = result.Properties["code"];
        StatusLabel.Text = "アクセストークン取得中...";

        var tokens = await _loginUseCase.LoginWithCodeAsync(code, "myapp://callback");
        StatusLabel.Text += $"AccessToken = {tokens.AccessToken} RefreshToken={tokens.RefreshToken}";
        Preferences.Set("box_access_token", tokens.AccessToken);
        Preferences.Set("box_refresh_token", tokens.RefreshToken);

        StatusLabel.Text += "ログイン成功！";
      }
      else
      {
        StatusLabel.Text += "認可コードが取得できませんでした。";
      }
    }
    catch (Exception ex)
    {
      StatusLabel.Text += $"ログイン失敗: {ex.Message}";
      await Shell.Current.DisplayAlert("Debug", ex.Message, "OK");

    }
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

