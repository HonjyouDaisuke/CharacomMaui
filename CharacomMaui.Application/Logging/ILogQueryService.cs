namespace CharacomMaui.Application.Logging;

public interface ILogQueryService
{
  Task<List<LogDto>> GetLogsAsync(int page = 1);
}