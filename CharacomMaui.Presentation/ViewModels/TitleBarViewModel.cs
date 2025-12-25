using System.ComponentModel;
using System.Runtime.CompilerServices;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Presentation.Models;

namespace CharacomMaui.Presentation.ViewModels;

public class TitleBarViewModel : INotifyPropertyChanged
{
  private readonly AppStatusNotifier _notifier;
  private readonly AppStatus _appStatus;

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

  // avaterText
  private string avatarString = string.Empty;
  public string AvatarString
  {
    get => avatarString;
    set
    {
      if (avatarString != value)
      {
        avatarString = value;
        OnPropertyChanged();
      }
    }
  }

  private string avatarUrl = string.Empty;
  public string AvatarUrl
  {
    get => avatarUrl;
    set
    {
      if (avatarUrl != value)
      {
        avatarUrl = value;
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

  public TitleBarViewModel(AppStatusNotifier notifier, AppStatus appStatus)
  {
    System.Diagnostics.Debug.WriteLine($"[VM] TitleBarViewModel created: {GetHashCode()}");
    _notifier = notifier;
    _appStatus = appStatus;

    // AppStatusNotifier の変更を購読
    _notifier.PropertyChanged += (_, e) =>
    {
      System.Diagnostics.Debug.WriteLine($"[VM] PropertyChanged name = '{e.PropertyName}'");
      if (e.PropertyName == nameof(AppStatusNotifier.ProjectName))
      {
        TitleString = MakeTitleString();
      }

      if (e.PropertyName == nameof(AppStatusNotifier.AvatarUrl))
      {
        System.Diagnostics.Debug.WriteLine($"[VM] TitleBarViewModel created: {GetHashCode()}");

        System.Diagnostics.Debug.WriteLine($"[viewmodel] avatar changed = {_notifier.AvatarUrl}");
        AvatarUrl = _notifier.AvatarUrl;
      }
    };


    TitleString = MakeTitleString();
    AvatarString = MakeAvatarString();
    AvatarUrl = MakeAvatarUrl();
  }

  private string MakeAvatarString()
  {
    System.Diagnostics.Debug.WriteLine($"UserId{_appStatus.UserId}");

    return _appStatus.UserId.Substring(0, 2);

  }

  private string MakeAvatarUrl()
  {
    return _notifier.AvatarUrl;
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
