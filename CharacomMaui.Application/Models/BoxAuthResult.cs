// CharacomMaui.Application/Models/BoxAuthResult.cs
namespace CharacomMaui.Application.Models;

public class BoxAuthResult
{
  public string AccessToken { get; set; } = string.Empty;
  public string RefreshToken { get; set; } = string.Empty;
  public int ExpiresIn { get; set; } // 秒数
}