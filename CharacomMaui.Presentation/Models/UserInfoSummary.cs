using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CharacomMaui.Presentation.Models;

public class UserInfoSummary : INotifyPropertyChanged
{
  public const string DefaultPictureUrl = "https://characom.sakuraweb.com/CharacomMaui/avatars/avatar01.png";

  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string AvatarUrl { get; set; } = DefaultPictureUrl;
  public string RoleId { get; set; } = string.Empty;
  private bool _isOdd;
  public bool IsOdd
  {
    get => _isOdd;
    set
    {
      if (_isOdd != value)
      {
        _isOdd = value;
        OnPropertyChanged();
      }
    }
  }
  private bool _isSelected;
  public bool IsSelected
  {
    get => _isSelected;
    set
    {
      if (_isSelected != value)
      {
        _isSelected = value;
        OnPropertyChanged();
      }
    }
  }
  public event PropertyChangedEventHandler? PropertyChanged;
  private void OnPropertyChanged([CallerMemberName] string? name = null)
  {
    System.Diagnostics.Debug.WriteLine($"UserInfoSummary OnPropertyChanged: {name}");
    MainThread.BeginInvokeOnMainThread(() =>
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    });
  }
}
