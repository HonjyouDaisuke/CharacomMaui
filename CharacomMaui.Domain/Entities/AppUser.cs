namespace CharacomMaui.Domain.Entities;
// TODO:notification_emailがnullなので要確認
public class AppUser
{
  public const string DefaultPictureUrl = "https://characom.sakuraweb.com/CharacomMaui/avatars/avatar01.png";

  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string PictureUrl { get; set; } = DefaultPictureUrl;
  public string BoxAccessToken { get; set; } = string.Empty;
  public string BoxRefreshToken { get; set; } = string.Empty;
  public string TokenExpiresAt { get; set; } = string.Empty;
  public string RoleId { get; set; } = string.Empty;
  public string AvatarImgString { get; set; } = string.Empty;
}
