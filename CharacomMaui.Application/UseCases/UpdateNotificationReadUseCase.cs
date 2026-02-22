using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.Sessions;
using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Application.UseCases;

public class UpdateNotificationReadUseCase
{
  private readonly INotificationsRepository _repository;

  public UpdateNotificationReadUseCase(INotificationsRepository repository)
  {
    _repository = repository;
  }

  public async Task<bool> ExecuteAsync(string accessToken, string id)
  {
    return await _repository.UpdateNotificationRead(accessToken, id);
  }
}