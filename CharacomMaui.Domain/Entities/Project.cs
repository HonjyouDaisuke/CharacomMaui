using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CharacomMaui.Domain.Entities;
// TODO:notification_emailがnullなので要確認
public class Project
{
  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string FolderId { get; set; } = string.Empty;
  public string CharaFolderId { get; set; } = string.Empty;
  public string CreatedBy { get; set; } = string.Empty;
  public int CharaCount { get; set; } = 0;
  public int UserCount { get; set; } = 0;
  public string OwnerName { get; set; } = string.Empty;
  public string RoleId { get; set; } = string.Empty;

}

public class ProjectInfoResponse
{
  [JsonPropertyName("success")]
  public bool Success { get; set; } = false;
  [JsonPropertyName("user_id")]
  public string UserId { get; set; } = string.Empty;
  public List<ProjectItem> Projects { get; set; } = [];
}

// TODO:`Project`クラスと同じ構造をしているので一緒にする
public class ProjectItem
{
  [JsonPropertyName("project_id")]
  public string ProjectId { get; set; } = string.Empty;
  [JsonPropertyName("name")]

  public string Name { get; set; } = string.Empty;
  [JsonPropertyName("description")]
  public string Description { get; set; } = string.Empty;
  [JsonPropertyName("folder_id")]
  public string FolderId { get; set; } = string.Empty;
  [JsonPropertyName("chara_folder_id")]
  public string CharaFolderId { get; set; } = string.Empty;
  [JsonPropertyName("chara_count")]
  public int CharaCount { get; set; }
  [JsonPropertyName("user_count")]
  public int UserCount { get; set; }
}
