public class SimpleApiResult
{
  public bool Success { get; set; } = false;
  public string Message { get; set; } = string.Empty;

  public override string ToString()
  {
    return $"Success:{Success} --- Message:{Message}";
  }
}