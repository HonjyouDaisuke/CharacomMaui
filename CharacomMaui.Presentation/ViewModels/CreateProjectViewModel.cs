using System.ComponentModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Application.Interfaces;
using CommunityToolkit.Maui.Converters;

namespace CharacomMaui.Presentation.ViewModels;

public partial class CreateProjectViewModel : ObservableObject
{
  private readonly GetBoxFolderItemsUseCase _getFolderItemsUsecase;
  private readonly CreateOrUpdateProjectUseCase _createOrUpdateProjectUsecase;
  private readonly GetUserProjectsUseCase _getUserProjectsUseCase;
  private readonly UpdateStrokeMasterUseCase _updateStrokeMasterUseCase;
  private readonly UpdateStandardMasterUseCase _updateStandardMasterUseCase;
  private readonly DeleteProjectUseCase _deleteProjectUseCase;
  private readonly IAppTokenStorageService _tokenStorage;
  private BoxItem _selectedFolder = new();

  public AppStatus _appStatus = new();
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
                                CreateOrUpdateProjectUseCase createOrUpdateProjectUseCase,
                                GetUserProjectsUseCase getUserProjectsUseCase,
                                UpdateStrokeMasterUseCase updateStrokeMasterUseCase,
                                UpdateStandardMasterUseCase updateStandardMasterUseCase,
                                DeleteProjectUseCase deleteProjectUseCase,
                                IAppTokenStorageService tokenStorage)
  {
    _getFolderItemsUsecase = getBoxFolderItemsUseCase;
    _createOrUpdateProjectUsecase = createOrUpdateProjectUseCase;
    _getUserProjectsUseCase = getUserProjectsUseCase;
    _updateStrokeMasterUseCase = updateStrokeMasterUseCase;
    _updateStandardMasterUseCase = updateStandardMasterUseCase;
    _deleteProjectUseCase = deleteProjectUseCase;
    _tokenStorage = tokenStorage;
  }

  public async Task<List<BoxItem>> GetFolderItemsAsync(string? folderId = null)
  {
    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    Debug.WriteLine($"AppAccessToken : {accessToken}");
    if (accessToken == null)
    {
      System.Diagnostics.Debug.WriteLine("AccessTokenの取得に失敗しました。");
      return [];
    }
    return await _getFolderItemsUsecase.ExecuteAsync(accessToken, folderId);
  }

  public async Task<SimpleApiResult> CreateOrUpdateProjectAsync(Project project)
  {
    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    if (accessToken == null)
    {
      System.Diagnostics.Debug.WriteLine("AccessTokenの取得に失敗しました。");
      return new SimpleApiResult
      {
        Success = false,
        Message = "AccessTokenの取得に失敗しました。"
      };
    }
    return await _createOrUpdateProjectUsecase.ExecuteAsync(accessToken, project);
  }

  public async Task<List<Project>?> GetProjectsAsync()
  {
    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    if (accessToken == null)
    {
      System.Diagnostics.Debug.WriteLine("AccessTokenの取得に失敗しました。");
      return null;
    }
    return await _getUserProjectsUseCase.GetProjectsInfoAsync(accessToken);
  }

  public AppStatus AppStatus
  {
    get => _appStatus;
    set
    {
      if (_appStatus != value)
      {
        _appStatus = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppStatus)));
      }
    }
  }

  public void SetUserStatus(AppStatus appStatus)
  {
    AppStatus = appStatus;
  }

  public async Task<SimpleApiResult> UpdateStrokeAsync()
  {
    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    if (accessToken == null)
      return new SimpleApiResult
      {
        Success = false,
        Message = "AccessTokenの取得に失敗しました。"
      };
    return await _updateStrokeMasterUseCase.ExecuteAsync(accessToken);
  }

  public async Task<SimpleApiResult> UpdateStandardAsync()
  {
    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    if (accessToken == null)
      return new SimpleApiResult
      {
        Success = false,
        Message = "AccessTokenの取得に失敗しました。"
      };
    return await _updateStandardMasterUseCase.ExecuteAsync(accessToken);
  }

  public async Task<SimpleApiResult> DeleteProjectAsync(string projectId)
  {
    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    if (accessToken == null)
      return new SimpleApiResult
      {
        Success = false,
        Message = "AccessTokenの取得に失敗しました。"
      };
    return await _deleteProjectUseCase.ExecuteAsync(accessToken, projectId);
  }
}

