namespace CharacomMaui.Application.Logging;

public class LogRequest
{
  public string Level { get; set; } = default!;
  public string Screen { get; set; } = default!;
  public string Action { get; set; } = default!;
  public string? Message { get; set; }
  public object? Data { get; set; }
  public string? CorrelationId { get; set; }
}