namespace CharacomMaui.Application.Interfaces;

using CharacomMaui.Domain.Entities;

public interface INotificationsRepository
{
  Task<List<NotificationItem>?> GetNotifications(string accessToken);
  Task<bool> UpdateNotificationRead(string accessToken, string id);
  Task<bool> UpdateNotificationDeleted(string accessToken, string id);
}