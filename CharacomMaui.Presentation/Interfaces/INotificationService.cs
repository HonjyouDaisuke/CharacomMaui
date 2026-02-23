namespace CharacomMaui.Presentation.Interfaces;

using System.Collections.ObjectModel;
using CharacomMaui.Presentation.Services;
using CharacomMaui.Presentation.Models;
using CharacomMaui.Domain.Entities;


public interface INotificationService
{
  event EventHandler<NotificationRequest>? NotificationRequested;
  event EventHandler<string>? DeleteRequested;

  ObservableCollection<NotificationItem> Notifications { get; set; }
  void RequestOpen(string id, string title, string message, string icon, string createdAt);
  void RequestDelete(string id);
  Task MarkAsReadAsync(string accessToken, string id);
  Task MarkAsDeleteAsync(string accessToken, string id);
  Task AllReadAsync(string accessToken);
  Task InitNotificationsAsync(string accessToken);
}