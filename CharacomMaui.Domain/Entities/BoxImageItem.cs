// CharacomMaui.Application/Entities/BoxItem.cs
namespace CharacomMaui.Domain.Entities
{
  public class BoxImageItem
  {
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public byte[] Image { get; set; }
  }
}
