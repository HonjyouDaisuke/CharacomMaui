using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.Sessions;
using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Application.UseCases;

public class FetchNotificationsUseCase
{
  private readonly INotificationsRepository _repository;

  public FetchNotificationsUseCase(INotificationsRepository repository)
  {
    _repository = repository;
  }

  public async Task<List<NotificationItem>> ExecuteAsync(string accessToken)
  {
    var notifications = await _repository.GetNotificationsAsync(accessToken);
    if (notifications == null) return new List<NotificationItem>();
    return notifications;
  }
}