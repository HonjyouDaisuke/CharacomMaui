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
  private readonly FetchUserRolesUseCase _userRolesUseCase;
  private readonly IAppLogger _logger;
  public App(IAppLogger logger, IGetUserInfoUseCase userUserCase, AppStatusUseCase statusUseCase, IAppTokenStorageService tokenStorage, FetchUserRolesUseCase userRolesUseCase)
  {
    InitializeComponent();
    _userUseCase = userUserCase;
    _statusUseCase = statusUseCase;
    _tokenStorage = tokenStorage;
    _userRolesUseCase = userRolesUseCase;
    _logger = logger;
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
          await SnackBarService.Error($"Box Login Error: {ex.Message}");
        }
      }
    }
  }

  protected override Window CreateWindow(IActivationState? activationState)
  {
    var window = new Window(new LoadingPage(_logger, _userUseCase, _statusUseCase, _tokenStorage, _userRolesUseCase));

    window.Title = "CharacomMaui";  // ← ★ここでタイトルを設定！
    return window;
  }
}