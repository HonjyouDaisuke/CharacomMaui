using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CharacomMaui.Domain.Entities;

public class CharaDataSummary
{
  public int Number { get; set; } = 0;
  public string CharaName { get; set; } = string.Empty;
  public string MaterialName { get; set; } = string.Empty;
  public int CharaCount { get; set; } = 0;
  public int SelectedCount { get; set; } = 0;
}
