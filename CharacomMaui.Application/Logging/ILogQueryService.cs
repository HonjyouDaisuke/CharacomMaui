namespace CharacomMaui.Application.Logging;

public interface ILogQueryService
{
  Task<List<LogDto>> GetLogsAsync(DateTime targetDate, int page = 1);
}