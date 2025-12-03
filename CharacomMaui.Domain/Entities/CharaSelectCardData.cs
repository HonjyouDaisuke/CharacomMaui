using System;

namespace CharacomMaui.Domain.Entities;

public class CharaSelectCardData
{
  public string CharaId { get; set; } = string.Empty;
  public string FileId { get; set; } = string.Empty;
  public string CharaName { get; set; } = string.Empty;
  public string MaterialName { get; set; } = string.Empty;
  public string TimesName { get; set; } = string.Empty;
  public byte[] RawImageData { get; set; } = [];
  public bool? IsSelected { get; set; } = false;
}