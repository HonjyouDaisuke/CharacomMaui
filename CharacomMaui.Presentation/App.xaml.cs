using System.Web;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Presentation.Services;

namespace CharacomMaui.Presentation;

public partial class App : Microsoft.Maui.Controls.Application
{
  private readonly IGetUserInfoUseCase _userUseCase;
  private readonly AppStatusUseCase _statusUseCase;
  private readonly IAppTokenStorageService _tokenStorage;
  public App(IGetUserInfoUseCase userUserCase, AppStatusUseCase statusUseCase, IAppTokenStorageService tokenStorage)
  {
    InitializeComponent();
    _userUseCase = userUserCase;
    _statusUseCase = statusUseCase;
    _tokenStorage = tokenStorage;
  }


  protected override async void OnAppLinkRequestReceived(Uri uri)
  {
    base.OnAppLinkRequestReceived(uri);

    if (uri == null)
      return;

    if (uri.Scheme == "myapp" && uri.Host == "callback")
    {
      var query = HttpUtility.ParseQueryString(uri.Query);
      var code = query["code"];

      if (!string.IsNullOrEmpty(code))
      {
        try
        {
          // DIなどで取得する実装に置き換えてOK
          //var boxAuthService = ServiceHelper.GetService<IBoxApiAuthService>();
          //var result = await boxAuthService.ExchangeCodeForTokenAsync(code, "myapp://callback");


        }
        catch (Exception ex)
        {
          await Shell.Current.DisplayAlert("Box Login Error", ex.Message, "OK");
        }
      }
    }
  }

  protected override Window CreateWindow(IActivationState? activationState)
  {
    var window = new Window(new LoadingPage(_userUseCase, _statusUseCase, _tokenStorage));

    window.Title = "CharacomMaui";  // ← ★ここでタイトルを設定！
    return window;
  }
}