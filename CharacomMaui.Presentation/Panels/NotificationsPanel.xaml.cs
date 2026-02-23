using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CharacomMaui.Presentation.Models;
using CharacomMaui.Presentation.Interfaces;
using CharacomMaui.Application.Interfaces;
namespace CharacomMaui.Presentation.Panels;

public partial class NotificationsPanel : ContentView
{
  INotificationService _notificationService;
  IAppTokenStorageService _tokenStorage;
  public NotificationsPanel()
  {
    InitializeComponent();
  }
  protected override void OnHandlerChanged()
  {
    base.OnHandlerChanged();

    if (Handler?.MauiContext?.Services is not IServiceProvider services)
      return;

    _notificationService = services.GetRequiredService<INotificationService>();
    _tokenStorage = services.GetRequiredService<IAppTokenStorageService>();
    BindingContext = _notificationService;

    System.Diagnostics.Debug.WriteLine(
        $"[panel]: {_notificationService?.Notifications.Count}");
  }

  private async void OnAllReadClicked(object sender, EventArgs e)
  {
    if (_tokenStorage == null || _notificationService == null) return;

    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    if (accessToken == null) return;
    await _notificationService.AllReadAsync(accessToken);
  }
}