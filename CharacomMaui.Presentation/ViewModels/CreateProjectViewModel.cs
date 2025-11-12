using System.ComponentModel;
using System.Diagnostics;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Infrastructure.Services;
using CharacomMaui.Presentation;

public class CreateProjectViewModel : INotifyPropertyChanged
{
  public List<BoxItem> Folders { get; }
  private readonly GetBoxFolderItemsUseCase _getFolderItemsUsecase;
  private readonly CreateProjectUseCase _createProjectUsecase;
  private readonly GetUserProjectsUseCase _getUserProjectsUseCase;
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

  public CreateProjectViewModel(GetBoxFolderItemsUseCase getBoxFolderItemsUseCase,
                                CreateProjectUseCase createProjectUseCase,
                                GetUserProjectsUseCase getUserProjectsUseCase)
  {
    _getFolderItemsUsecase = getBoxFolderItemsUseCase;
    _createProjectUsecase = createProjectUseCase;
    _getUserProjectsUseCase = getUserProjectsUseCase;
  }

  public async Task<List<BoxItem>> GetFolderItemsAsync(string? folderId = null)
  {
    var accessToken = Preferences.Get("app_access_token", string.Empty);
    Debug.WriteLine($"AppAccessToken : {accessToken}");
    return await _getFolderItemsUsecase.ExecuteAsync(accessToken, folderId);
  }

  public async Task<SimpleApiResult> CreateProjectAsync(Project project)
  {
    var access_token = Preferences.Get("app_access_token", string.Empty);
    return await _createProjectUsecase.ExecuteAsync(access_token, project);
  }

  public async Task<List<Project>?> GetProjectsAsync()
  {
    var access_token = Preferences.Get("app_access_token", string.Empty);
    return await _getUserProjectsUseCase.GetProjectsInfoAsync(access_token);
  }
}

