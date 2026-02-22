namespace CharacomMaui.Presentation.Interfaces;

using System.Collections.ObjectModel;
using CharacomMaui.Presentation.Services;
using CharacomMaui.Presentation.Models;
using CharacomMaui.Domain.Entities;


public interface INotificationService
{
  event EventHandler<NotificationRequest>? NotificationRequested;
  ObservableCollection<NotificationItem> Notifications { get; set; }
  void RequestOpen(string id, string title, string message, string icon);
  Task MarkAsReadAsync(string accessToken, string id);
  Task AllReadAsync(string accessToken);
  Task InitNotificationsAsync(string accessToken);
}