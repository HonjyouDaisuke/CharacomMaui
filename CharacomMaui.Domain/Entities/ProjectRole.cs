using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CharacomMaui.Domain.Entities;

public class ProjectRole
{
  [JsonPropertyName("id")]
  public string Id { get; set; } = "";
  [JsonPropertyName("name")]
  public string Name { get; set; } = "";
  [JsonPropertyName("description")]
  public string Description { get; set; } = "";
  [JsonPropertyName("level")]
  public int Level { get; set; } = 0;
  [JsonPropertyName("created_at")]
  public DateTime CreatedAt { get; set; } = DateTime.Now;

  [JsonPropertyName("updated_at")]
  public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

