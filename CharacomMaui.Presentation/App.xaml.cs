using Microsoft.Maui.ApplicationModel;
using CharacomMaui.Presentation.Helpers;
using System.Web;
using CharacomMaui.Application.Interfaces;
using MauiApp = Microsoft.Maui.Controls.Application;
using Microsoft.Extensions.DependencyInjection;
using UraniumUI;
using System.Threading.Tasks;

namespace CharacomMaui.Presentation;

public partial class App : Microsoft.Maui.Controls.Application
{
  public App()
  {
    InitializeComponent();
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

          //await Shell.Current.DisplayAlert("Box Login", $"Access Token: {result.AccessToken}", "OK");
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
    return new Window(new LoadingPage());
  }
}