using System.ComponentModel;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Infrastructure.Services;

public class CreateProjectViewModel : INotifyPropertyChanged
{
  public List<BoxItem> Folders { get; }
  private readonly IBoxFolderRepository _folderRepository;
  private BoxItem _selectedFolder = new();
  public BoxItem SelectedFolder
  {
    get => _selectedFolder;
    set
    {
      if (_selectedFolder != value)
      {
        _selectedFolder = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedFolder)));
        System.Diagnostics.Debug.WriteLine($"Selected -> {_selectedFolder.Name}");
      }
    }
  }

  public event PropertyChangedEventHandler PropertyChanged;

  public CreateProjectViewModel(IBoxFolderRepository folderRepository)
  {
    _folderRepository = folderRepository;
  }

  public async Task<List<BoxItem>> GetFolderItemsAsync(string? folderId = null)
  {
    var accessToken = Preferences.Get("app_access_token", string.Empty);
    return await _folderRepository.GetFolderItemsAsync(accessToken, folderId);
  }
}

