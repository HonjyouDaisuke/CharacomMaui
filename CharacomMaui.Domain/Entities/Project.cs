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
}
