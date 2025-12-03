using System.ComponentModel;
using System.Runtime.CompilerServices;
using CharacomMaui.Domain.Entities;

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
      => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
