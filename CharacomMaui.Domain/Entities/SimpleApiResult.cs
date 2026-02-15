public class SimpleApiResult
{
  public bool Success { get; set; } = false;
  public string Message { get; set; } = string.Empty;
  public SimpleApiResult(bool success = false, string message = "")
  {
    Success = success;
    Message = message;
  }

  public override string ToString()
  {
    return $"Success:{Success} --- Message:{Message}";
  }
}