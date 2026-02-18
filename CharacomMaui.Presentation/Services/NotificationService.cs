using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.Models;
using CharacomMaui.Presentation.Interfaces;

namespace CharacomMaui.Presentation.Services;

public record NotificationRequest(
    string Id,
    string Title,
    string Message,
    string Icon);

public class NotificationService : INotificationService
{
  public event EventHandler<NotificationRequest>? NotificationRequested;

  public void RequestOpen(string id, string title, string message, string icon)
  {
    NotificationRequested?.Invoke(
        this,
        new NotificationRequest(id, title, message, icon));
  }
}