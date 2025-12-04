namespace CharacomMaui.Domain.Entities;

public class AppStatus
{
  public string UserId { get; set; } = string.Empty;
  public string UserName { get; set; } = string.Empty;
  public string UserRole { get; set; } = string.Empty;
  public string ProjectId { get; set; } = string.Empty;
  public string ProjectName { get; set; } = string.Empty;
  public string ProjectFolderId { get; set; } = string.Empty;
  public string CharaFolderId { get; set; } = string.Empty;
  public string MaterialName { get; set; } = string.Empty;
  public string? CharaName { get; set; } = string.Empty;
  public string ProjectRole { get; set; } = string.Empty;
  public string AvatarUrl { get; set; } = string.Empty;
  public string AvatarImgString { get; set; } = string.Empty;

  /// <summary>
  /// Produces a single-line string representation of the app status containing key user, project, and character fields.
  /// </summary>
  /// <returns>A single-line string that includes UserId, UserName, UserRole, AvatarUrl, ProjectName, CharaName, and MaterialName (MaterialName appears labeled as "MatrialName" in the output).</returns>
  public override string ToString()
  {
    return $"userId = {UserId} userName = {UserName} userRole = {UserRole} avatarUrl = {AvatarUrl} projectName = {ProjectName} CharaName = {CharaName} MatrialName={MaterialName}";
  }
}