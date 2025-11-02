using Android.App;
using Android.Content;
using Android.Content.PM;
using System.Diagnostics;

namespace CharacomMaui.Presentation;

[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
[IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[]
        {
            Intent.CategoryDefault,
            Intent.CategoryBrowsable
        },
        DataScheme = "myapp",
        DataHost = "callback"
    )]
public class WebAuthenticationCallbackActivity : WebAuthenticatorCallbackActivity
{
    protected override void OnResume()
    {
        base.OnResume();
        System.Diagnostics.Debug.WriteLine("🟢 [OAuthDebug] WebAuthenticationCallbackActivity.OnResume() called.");
    }

    protected override void OnNewIntent(Android.Content.Intent intent)
    {
        base.OnNewIntent(intent);

        System.Diagnostics.Debug.WriteLine("🟢 [OAuthDebug] OnNewIntent() called.");

        if (intent?.Data != null)
        {
            System.Diagnostics.Debug.WriteLine($"🟢 [OAuthDebug] Intent Data: {intent.Data}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("🟠 [OAuthDebug] Intent Data is null.");
        }
    }
}