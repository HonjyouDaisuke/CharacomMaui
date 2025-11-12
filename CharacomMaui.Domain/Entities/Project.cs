using System.Collections.Generic;

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

}


public class ProjectInfoResponse
{
  public bool Success { get; set; }
  public string User_Id { get; set; }
  public List<ProjectItem> Projects { get; set; }
}

public class ProjectItem
{
  public string Project_Id { get; set; }
  public string Name { get; set; }
  public string Description { get; set; }
  public int Chara_Count { get; set; }
  public int User_Count { get; set; }
}
