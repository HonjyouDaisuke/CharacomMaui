namespace CharacomMaui.Presentation.Interfaces;

using CharacomMaui.Presentation.Services;


public interface INotificationService
{
  event EventHandler<NotificationRequest>? NotificationRequested;
  void RequestOpen(string id, string title, string message, string icon);
}