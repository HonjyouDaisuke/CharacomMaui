using System.ComponentModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Application.Interfaces;
using CommunityToolkit.Maui.Converters;
using Box.Sdk.Gen.Schemas;

namespace CharacomMaui.Presentation.ViewModels;

public partial class CreateProjectViewModel : ObservableObject
{
  private readonly GetBoxFolderItemsUseCase _getFolderItemsUsecase;
  private readonly CreateOrUpdateProjectUseCase _createOrUpdateProjectUsecase;
  private readonly GetUserProjectsUseCase _getUserProjectsUseCase;
  private readonly UpdateStrokeMasterUseCase _updateStrokeMasterUseCase;
  private readonly UpdateStandardMasterUseCase _updateStandardMasterUseCase;
  private readonly InviteToProjectUseCase _inviteToProjectUseCase;
  private readonly DeleteProjectUseCase _deleteProjectUseCase;
  private readonly IAppTokenStorageService _tokenStorage;
  private BoxItem _selectedFolder = new();

  public AppStatus _appStatus = new();
  private readonly IAppLogger _logger;

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
                                IAppLogger logger,
                                InviteToProjectUseCase inviteToProjectUseCase,
                                IAppTokenStorageService tokenStorage)
  {
    _getFolderItemsUsecase = getBoxFolderItemsUseCase;
    _createOrUpdateProjectUsecase = createOrUpdateProjectUseCase;
    _getUserProjectsUseCase = getUserProjectsUseCase;
    _updateStrokeMasterUseCase = updateStrokeMasterUseCase;
    _updateStandardMasterUseCase = updateStandardMasterUseCase;
    _deleteProjectUseCase = deleteProjectUseCase;
    _inviteToProjectUseCase = inviteToProjectUseCase;
    _logger = logger;
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
  public async Task<string> GetAccessTokenAsync()
  {
    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    if (accessToken == null) return string.Empty;
    return accessToken;
  }
  public async Task<SimpleApiResult> InviteToProjectAsync(string projectId, string toUserId, string toRoleId)
  {
    try
    {
      if (string.IsNullOrWhiteSpace(projectId) ||
        string.IsNullOrWhiteSpace(toUserId) ||
        string.IsNullOrWhiteSpace(toRoleId))
      {
        return new SimpleApiResult(false, "招待パラメータが不正です");
      }
      var tokens = await _tokenStorage.GetTokensAsync();
      var accessToken = tokens?.AccessToken;
      if (accessToken == null) return new SimpleApiResult(false, "accessTokenエラーが発生しました");

      var res = await _inviteToProjectUseCase.ExecuteAsync(accessToken, projectId, toUserId, toRoleId);
      if (res.Success)
      {
        await _logger.SystemInfo(_appStatus.UserId, this.GetType().Name, "プロジェクトへ招待", "プロジェクトに招待しました。", new { projectId, toUserId, toRoleId });
        return res;
      }
      else
      {
        await _logger.SystemWarning(_appStatus.UserId, this.GetType().Name, "プロジェクトへ招待", "プロジェクトへの招待に失敗しました。", new { projectId, toUserId, toRoleId });
        return res;
      }
    }
    catch (Exception ex)
    {
      await _logger.SystemError(ex, _appStatus.UserId, this.GetType().Name, "プロジェクトへ招待", new { projectId, toUserId, toRoleId });
      return new SimpleApiResult(false, "想定外のエラーが発生しました");
    }
  }
}

