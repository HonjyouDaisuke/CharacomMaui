namespace CharacomMaui.Application.Logging;

public interface ILogQueryService
{
  Task<LogQueryResult> GetLogsAsync(DateTime targetDate, int limit = 50, int page = 1);
}