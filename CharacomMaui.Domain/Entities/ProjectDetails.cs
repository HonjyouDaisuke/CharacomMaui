using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CharacomMaui.Domain.Entities;

public class ProjectDetails
{
  [JsonPropertyName("id")]
  public string Id { get; set; } = "";
  [JsonPropertyName("name")]
  public string Name { get; set; } = "";
  [JsonPropertyName("description")]
  public string Description { get; set; } = "";
  [JsonPropertyName("project_folder_id")]
  public string ProjectFolderId { get; set; } = "";
  [JsonPropertyName("chara_folder_id")]
  public string CharaFolderId { get; set; } = "";
  [JsonPropertyName("created_at")]
  public DateTime CreatedAt { get; set; } = DateTime.Now;
  [JsonPropertyName("created_by")]
  public string CreatedBy { get; set; } = "";
  [JsonPropertyName("updated_at")]
  public DateTime UpdatedAt { get; set; } = DateTime.Now;
  [JsonPropertyName("chara_count")]
  public int CharaCount { get; set; }
  [JsonPropertyName("participants")]
  public List<string> Participants { get; set; } = [];
}

public class ProjectDetailsResponse
{
  public bool Success { get; set; }
  public ProjectDetails Data { get; set; } = default!;
}
