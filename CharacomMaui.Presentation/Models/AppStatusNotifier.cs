using System.ComponentModel;
using System.Runtime.CompilerServices;
using CharacomMaui.Domain.Entities;
using CommunityToolkit.Maui.Core.Platform;

namespace CharacomMaui.Presentation.Models;

public class AppStatusNotifier : INotifyPropertyChanged
{
  private readonly AppStatus _status;

  public AppStatusNotifier(AppStatus status)
  {
    _status = status;
  }

  public string ProjectName
  {
    get => _status.ProjectName;
    set
    {
      if (_status.ProjectName != value)
      {
        _status.ProjectName = value;
        System.Diagnostics.Debug.WriteLine($"value = {value}");
        OnPropertyChanged();
      }
    }
  }

  public string ProjectId
  {
    get => _status.ProjectId;
    set
    {
      if (_status.ProjectId != value)
      {
        _status.ProjectId = value;
        OnPropertyChanged();
      }
    }
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  private void OnPropertyChanged([CallerMemberName] string? name = null)
  {
    MainThread.BeginInvokeOnMainThread(() =>
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    });
  }


  public string? CharaName
  {
    get => _status.CharaName;
    set
    {
      if (_status.CharaName != value)
      {
        _status.CharaName = value;
        OnPropertyChanged();
      }
    }
  }

  public string? MaterialName
  {
    get => _status.MaterialName;
    set
    {
      if (_status.MaterialName != value)
      {
        _status.MaterialName = value;
        OnPropertyChanged();
      }
    }
  }
}
