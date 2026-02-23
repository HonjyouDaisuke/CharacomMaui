using System.Collections.ObjectModel;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Application.Models;
using CharacomMaui.Presentation.Interfaces;
using CharacomMaui.Domain.Entities;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Layouts;
using System.ComponentModel;

namespace CharacomMaui.Presentation.Services;

public record NotificationRequest(
    string Id,
    string Title,
    string Message,
    string Icon,
    string CreatedAt);

public class NotificationService : INotificationService, INotifyPropertyChanged
{
  public ObservableCollection<NotificationItem> Notifications { get; set; } = new();
  private readonly FetchNotificationsUseCase _fetchNotificationsUseCase;
  private readonly UpdateNotificationReadUseCase _updateNotificationReadUseCase;
  private readonly AppStatus _appStatus;

  public event EventHandler<NotificationRequest>? NotificationRequested;

  public event PropertyChangedEventHandler? PropertyChanged;
  public event EventHandler<string>? DeleteRequested;

  public int UnreadCount =>
      Notifications.Count(x => !x.IsRead);

  public NotificationService(
    FetchNotificationsUseCase fetchNotificationsUseCase,
    UpdateNotificationReadUseCase updateNotificationReadUseCase,
    AppStatus appStatus)
  {
    _fetchNotificationsUseCase = fetchNotificationsUseCase;
    _updateNotificationReadUseCase = updateNotificationReadUseCase;
    _appStatus = appStatus;

    Notifications.CollectionChanged += (s, e) =>
    {
      OnPropertyChanged(nameof(UnreadCount));
      OnPropertyChanged(nameof(UnreadBadgeText));
      if (e.NewItems != null)
      {
        foreach (NotificationItem item in e.NewItems)
        {
          item.PropertyChanged += (_, __) =>
          {

            OnPropertyChanged(nameof(UnreadCount));
            OnPropertyChanged(nameof(UnreadBadgeText));
          };
        }
      }
    };
  }

  public void RequestOpen(string id, string title, string message, string icon, string createdAt)
  {
    NotificationRequested?.Invoke(
        this,
        new NotificationRequest(id, title, message, icon, createdAt));
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

    OnPropertyChanged(nameof(UnreadCount));
  }

  public async Task MarkAsDeleteAsync(string accessToken, string id)
  {
    if (_appStatus.IsProxy)
    {
      System.Diagnostics.Debug.WriteLine("代理ログイン中なので、既読フラグは変えません");
      return;
    }

    var res = await _updateNotificationReadUseCase.DeleteAsync(accessToken, id);
    if (res == false) return;
    await InitNotificationsAsync(accessToken);

    OnPropertyChanged(nameof(UnreadCount));
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
  public string UnreadBadgeText
  {
    get
    {
      var count = UnreadCount;

      if (count <= 0)
        return string.Empty;

      if (count >= 10)
        return "9+";

      return count.ToString();
    }
  }

  public void RequestDelete(string id)
  {
    DeleteRequested?.Invoke(this, id);
  }
  private void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}