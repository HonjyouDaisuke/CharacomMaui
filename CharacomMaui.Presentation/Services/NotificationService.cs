using System.Collections.ObjectModel;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Application.Models;
using CharacomMaui.Presentation.Interfaces;
using CharacomMaui.Domain.Entities;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Layouts;

namespace CharacomMaui.Presentation.Services;

public record NotificationRequest(
    string Id,
    string Title,
    string Message,
    string Icon);

public class NotificationService : INotificationService
{
  public ObservableCollection<NotificationItem> Notifications { get; set; } = new();
  private readonly FetchNotificationsUseCase _fetchNotificationsUseCase;
  private readonly UpdateNotificationReadUseCase _updateNotificationReadUseCase;
  private readonly AppStatus _appStatus;
  public NotificationService(FetchNotificationsUseCase fetchNotificationsUseCase, UpdateNotificationReadUseCase updateNotificationReadUseCase, AppStatus appStatus)
  {
    _fetchNotificationsUseCase = fetchNotificationsUseCase;
    _updateNotificationReadUseCase = updateNotificationReadUseCase;
    _appStatus = appStatus;
  }

  public event EventHandler<NotificationRequest>? NotificationRequested;

  public void RequestOpen(string id, string title, string message, string icon)
  {
    NotificationRequested?.Invoke(
        this,
        new NotificationRequest(id, title, message, icon));
  }
  public async Task MarkAsReadAsync(string accessToken, string id)
  {
    if (_appStatus.IsProxy)
    {
      System.Diagnostics.Debug.WriteLine("代理ログイン中なので、既読フラグは変えません");
      return;
    }
    var item = Notifications.FirstOrDefault(x => x.Id == id);
    if (item == null) return;
    if (item.IsRead) return;

    var res = await _updateNotificationReadUseCase.ExecuteAsync(accessToken, id);
    if (res == false) return;

    item.IsRead = true;
  }

  public async Task AllReadAsync(string accessToken)
  {
    if (Notifications.Count == 0) return;
    foreach (var notification in Notifications)
    {
      MarkAsReadAsync(accessToken, notification.Id);
    }
  }
  public async Task InitNotificationsAsync(string accessToken)
  {
    var res = await _fetchNotificationsUseCase.ExecuteAsync(accessToken);
    if (res == null)
    {
      System.Diagnostics.Debug.WriteLine("通知はありませんでした。");
      return;
    }
    System.Diagnostics.Debug.WriteLine($"通知は{res.Count}です。");
    Notifications.Clear();
    foreach (var item in res)
    {
      Notifications.Add(item);
    }
  }
}