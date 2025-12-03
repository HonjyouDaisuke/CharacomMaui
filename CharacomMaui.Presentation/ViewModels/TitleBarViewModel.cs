using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;
using CharacomMaui.Presentation.Models;

namespace CharacomMaui.Presentation.ViewModels;

public class TitleBarViewModel : INotifyPropertyChanged
{
  private readonly AppStatusNotifier _notifier;

  // バインディング用プロパティ
  private string titleString = string.Empty;
  public string TitleString
  {
    get => titleString;
    set
    {
      if (titleString != value)
      {
        titleString = value;
        OnPropertyChanged();
      }
    }
  }

  private ImageSource? avatarImageSource;
  public ImageSource? AvatarImageSource
  {
    get => avatarImageSource;
    set
    {
      if (avatarImageSource != value)
      {
        avatarImageSource = value;
        OnPropertyChanged();
      }
    }
  }

  public string ProjectName => _notifier.ProjectName;

  public TitleBarViewModel(AppStatusNotifier notifier)
  {
    _notifier = notifier;

    // AppStatusNotifier の変更を購読
    _notifier.PropertyChanged += (_, __) =>
    {
      OnPropertyChanged(nameof(ProjectName));
      System.Diagnostics.Debug.WriteLine($"Notifier!!{_notifier.ProjectName}");
      TitleString = MakeTitleString();
    };

    TitleString = MakeTitleString();
  }

  private string MakeTitleString()
  {
    System.Diagnostics.Debug.WriteLine($"Make String !!{_notifier.ProjectName}");
    string projectTitle = string.IsNullOrEmpty(_notifier.ProjectName)
        ? ""
        : " - " + _notifier.ProjectName;
    return "CharacomMaui" + projectTitle;
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  protected void OnPropertyChanged([CallerMemberName] string? name = null)
      => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
