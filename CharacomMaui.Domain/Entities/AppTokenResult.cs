namespace CharacomMaui.Domain.Entities;

using System.Text.Json.Serialization;
public class AppTokenResult
{
  [JsonPropertyName("success")]
  public bool Success { get; set; }

  [JsonPropertyName("access_token")]
  public string AccessToken { get; set; } = string.Empty;

  [JsonPropertyName("refresh_token")]
  public string RefreshToken { get; set; } = string.Empty;

  [JsonPropertyName("expire_at")]
  public int ExpiresAt { get; set; }

  [JsonPropertyName("message")]
  public string Message { get; set; } = string.Empty;

  public override string ToString()
  {
    return $"Success:{Success},AccessToken:{AccessToken},RefreshToken:{RefreshToken},ExpiresAt:{ExpiresAt},Message:{Message}";
  }
}