namespace CharacomMaui.Domain.Entities;

public class AppTokenResult
{
  public bool Success { get; set; } = false;
  public string AccessToken { get; set; } = string.Empty;
  public string RefreshToken { get; set; } = string.Empty;
  public int ExpiresAt { get; set; } // 秒数
  public string Message { get; set; } = string.Empty;

  public override string ToString()
  {
    return $"Success:{Success},AccessToken:{AccessToken},RefreshToken:{RefreshToken},ExpiresAt:{ExpiresAt},Message:{Message}";
  }
}