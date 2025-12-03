
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Presentation.Helpers;
using MauiApp = Microsoft.Maui.Controls.Application;

namespace CharacomMaui.Presentation;

public partial class LoadingPage : ContentPage
{
  public LoadingPage()
  {
    InitializeComponent();
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();

    var accessToken = Preferences.Get("app_access_token", string.Empty);
    var tokenService = ServiceHelper.GetService<ITokenValidationService>();
    bool isValid = false;
    if (!string.IsNullOrEmpty(accessToken))
    {
      try
      {
        var result = await tokenService.ValidateAsync(accessToken);
        System.Diagnostics.Debug.WriteLine($"tokenRes = {result}");

        isValid = result.Success;
      }
      catch
      {
        isValid = false;
      }
    }
    System.Diagnostics.Debug.WriteLine($"isValid = {isValid}");

    if (isValid)
      MauiApp.Current!.Windows[0].Page = new AppShell();
    else
      MauiApp.Current!.Windows[0].Page = new MainPage();
  }
}
