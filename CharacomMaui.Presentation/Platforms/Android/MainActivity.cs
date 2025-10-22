using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Microsoft.Maui;
using System;
using System.Web; // ← コレを使うと簡単にクエリ解析できる！

namespace CharacomMaui.Presentation
{
  [Activity(
      Theme = "@style/Maui.SplashTheme",
      MainLauncher = true,
      LaunchMode = LaunchMode.SingleTop,
      ConfigurationChanges = ConfigChanges.ScreenSize
          | ConfigChanges.Orientation
          | ConfigChanges.UiMode
          | ConfigChanges.ScreenLayout
          | ConfigChanges.SmallestScreenSize
          | ConfigChanges.Density)]
  [IntentFilter(
      new[] { Intent.ActionView },
      Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
      DataScheme = "myapp",        // Box のリダイレクトURIと一致
      DataHost = "callback"        // Box のリダイレクトURIと一致
  )]
  public class MainActivity : MauiAppCompatActivity
  {
    protected override void OnNewIntent(Intent intent)
    {
      base.OnNewIntent(intent);

      if (intent?.Data != null)
      {
        var uri = new Uri(intent.Data.ToString());
        var queryParams = HttpUtility.ParseQueryString(uri.Query);

        var code = queryParams.Get("code");

        if (!string.IsNullOrEmpty(code))
        {
          Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() =>
          {
            // MAUI 全体に通知（例: App.xaml.cs や ViewModel 側で購読）
            MessagingCenter.Send<object, string>(this, "BoxAuthCodeReceived", code);
          });
        }
      }
    }
  }
}
