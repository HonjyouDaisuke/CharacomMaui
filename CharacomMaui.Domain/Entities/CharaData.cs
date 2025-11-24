using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CharacomMaui.Domain.Entities;
// TODO:notification_emailがnullなので要確認
public class CharaData
{
  [JsonPropertyName("id")]
  public string Id { get; set; } = string.Empty;

  [JsonPropertyName("project_id")]
  public string ProjectId { get; set; } = string.Empty;

  [JsonPropertyName("file_id")]
  public string FileId { get; set; } = string.Empty;

  [JsonPropertyName("material_name")]
  public string MaterialName { get; set; } = string.Empty;

  [JsonPropertyName("chara_name")]
  public string CharaName { get; set; } = string.Empty;

  [JsonPropertyName("items_name")]
  public string TimesName { get; set; } = string.Empty;

  [JsonPropertyName("is_selected")]
  public bool? IsSelected { get; set; } = false;
}

public class CharaDataResponse
{
  [JsonPropertyName("success")]
  public bool Success { get; set; }

  [JsonPropertyName("items")]
  public List<CharaData> Items { get; set; } = new();
}