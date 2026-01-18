using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CharacomMaui.Presentation.Models;

public partial class UserInfoSummary : ObservableObject
{
  public const string DefaultPictureUrl = "https://characom.sakuraweb.com/CharacomMaui/avatars/avatar01.png";
  [ObservableProperty]
  private string id = string.Empty;

  [ObservableProperty]
  private string name = string.Empty;

  [ObservableProperty]
  private string email = string.Empty;

  [ObservableProperty]
  private string avatarUrl = DefaultPictureUrl;

  [ObservableProperty]
  private string roleId = string.Empty;
  [ObservableProperty]
  private string roleName = string.Empty;

  [ObservableProperty]
  private bool isOdd;

  [ObservableProperty]
  private bool isSelected;
}
