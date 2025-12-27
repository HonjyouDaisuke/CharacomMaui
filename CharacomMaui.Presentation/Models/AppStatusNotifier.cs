using System.ComponentModel;
using System.Runtime.CompilerServices;
using CharacomMaui.Domain.Entities;
using CommunityToolkit.Maui.Core.Platform;
using CommunityToolkit.Mvvm.ComponentModel;

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
  public string ProjectFolderId
  {
    get => _status.ProjectFolderId;
    set
    {
      if (_status.ProjectFolderId != value)
      {
        _status.ProjectFolderId = value;
        OnPropertyChanged();
      }
    }
  }
  public string CharaFolderId
  {
    get => _status.CharaFolderId;
    set
    {
      if (_status.CharaFolderId != value)
      {
        _status.CharaFolderId = value;
        OnPropertyChanged();
      }
    }
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
        _status.MaterialName = value ?? string.Empty;
        OnPropertyChanged();
      }
    }
  }

  public string? AvatarUrl
  {
    get => _status.AvatarUrl;
    set
    {
      if (_status.AvatarUrl != value)
      {
        _status.AvatarUrl = value ?? string.Empty;
        OnPropertyChanged();
      }
    }
  }
  public string? UserEmail
  {
    get => _status.UserEmail;
    set
    {
      if (_status.UserEmail != value)
      {
        _status.UserEmail = value ?? string.Empty;
        OnPropertyChanged();
      }
    }
  }
  public string? UserName
  {
    get => _status.UserName;
    set
    {
      if (_status.UserName != value)
      {
        _status.UserName = value ?? string.Empty;
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

}
/**
public string UserId { get; set; } = string.Empty;
  public string UserName { get; set; } = string.Empty;
  public string UserRole { get; set; } = string.Empty;
  public string UserEmail { get; set; } = string.Empty;
  public string ProjectId { get; set; } = string.Empty;
  public string ProjectName { get; set; } = string.Empty;
  public string ProjectFolderId { get; set; } = string.Empty;
  public string CharaFolderId { get; set; } = string.Empty;
  public string MaterialName { get; set; } = string.Empty;
  public string? CharaName { get; set; } = string.Empty;
  public string ProjectRole { get; set; } = string.Empty;
  public string AvatarUrl { get; set; } = string.Empty;
**/