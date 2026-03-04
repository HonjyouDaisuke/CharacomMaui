using System.Collections.ObjectModel;
using CharacomMaui.Application.Logging;
using CharacomMaui.Infrastructure.Logging;
namespace CharacomMaui.Presentation.ViewModels;

public class LogListViewModel
{
  private readonly ILogQueryService _logQueryService;

  public ObservableCollection<LogDto> Logs { get; } = new();

  public LogListViewModel(ILogQueryService logQueryService)
  {
    _logQueryService = logQueryService;
  }

  public async Task LoadAsync()
  {
    var result = await _logQueryService.GetLogsAsync();

    Logs.Clear();

    if (result == null) return;

    foreach (var log in result)
    {
      Logs.Add(log);
    }
  }
}