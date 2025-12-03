using System.Text.Json.Serialization;

namespace CharacomMaui.Domain.Entities;


public class TokenValidationResult
{
  [JsonPropertyName("success")]
  public bool Success { get; set; }

  [JsonPropertyName("userId")]
  public string? UserId { get; set; }

  [JsonPropertyName("payload")]
  public object? Payload { get; set; }

  [JsonPropertyName("error")]
  public string? Error { get; set; }
}