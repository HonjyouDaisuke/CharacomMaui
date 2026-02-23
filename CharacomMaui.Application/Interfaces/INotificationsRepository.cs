namespace CharacomMaui.Application.Interfaces;

using CharacomMaui.Domain.Entities;

public interface INotificationsRepository
{
  Task<List<NotificationItem>?> GetNotificationsAsync(string accessToken);
  Task<bool> UpdateNotificationReadAsync(string accessToken, string id);
  Task<bool> UpdateNotificationDeletedAsync(string accessToken, string id);
}