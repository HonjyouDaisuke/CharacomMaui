// CharacomMaui.Application/Entities/BoxItem.cs
using System.Text.Json.Serialization;
namespace CharacomMaui.Domain.Entities
{
  public class BoxItem
  {
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    //public byte[] Data { get; set; }
  }
}
