namespace CharacomMaui.Application.Logging;

public class LogDto
{
  public string Id { get; set; } = string.Empty;
  public string UserId { get; set; } = string.Empty;
  public string Level { get; set; } = string.Empty;
  public string Screen { get; set; } = string.Empty;
  public string Action { get; set; } = string.Empty;
  public string Message { get; set; } = string.Empty;
  public string CreatedAt { get; set; } = string.Empty;

}

public class LogQueryResult
{
  public int LogsCount { get; set; }
  public List<LogDto> Logs { get; set; } = new();
}