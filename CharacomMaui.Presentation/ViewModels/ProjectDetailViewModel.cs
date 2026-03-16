using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CharacomMaui.Presentation.Models;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Presentation.Services;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Application.Interfaces;
namespace CharacomMaui.Presentation.ViewModels;

public class ProjectDetailViewModel : INotifyPropertyChanged
{
  public ObservableCollection<CharaDataSummary> CharaDataSummaries { get; } = [];
  private readonly AppStatus _appStatus;
  IAppLogger _logger;
  GetProjectCharaItemsUseCase _useCase;
  GetProjectDetailsUseCase _getProjectDetailsUseCase;
  private readonly InviteToProjectUseCase _inviteToProjectUseCase;
  private readonly IAppTokenStorageService _tokenStorage;
  private string _projectId = string.Empty;
  public string ProjectId
  {
    get => _projectId;
    set => SetProperty(ref _projectId, value);
  }
  private string _projectName = string.Empty;
  public string ProjectName
  {
    get => _projectName;
    set => SetProperty(ref _projectName, value);
  }

  private string _projectDescription = string.Empty;
  public string ProjectDescription
  {
    get => _projectDescription;
    set => SetProperty(ref _projectDescription, value);
  }
  private string _projectFolderId = string.Empty;
  public string ProjectFolderId
  {
    get => _projectFolderId;
    set => SetProperty(ref _projectFolderId, value);
  }
  private string _charaFolderId = string.Empty;
  public string CharaFolderId
  {
    get => _charaFolderId;
    set => SetProperty(ref _charaFolderId, value);
  }
  private string _createdAt = string.Empty;
  public string CreatedAt
  {
    get => _createdAt;
    set => SetProperty(ref _createdAt, value);
  }

  private string _updatedAt = string.Empty;
  public string UpdatedAt
  {
    get => _updatedAt;
    set => SetProperty(ref _updatedAt, value);
  }

  private string _createdBy = string.Empty;
  public string CreatedBy
  {
    get => _createdBy;
    set => SetProperty(ref _createdBy, value);
  }
  private string _participantsText = string.Empty;
  public string ParticipantsText
  {
    get => _participantsText;
    set => SetProperty(ref _participantsText, value);
  }
  public ProjectDetailViewModel(GetProjectCharaItemsUseCase useCase,
                                AppStatus appStatus,
                                IAppLogger logger,
                                 GetProjectDetailsUseCase getProjectDetailsUseCase,
                                 IAppTokenStorageService tokenStorage,
                                 InviteToProjectUseCase inviteToProjectUseCase)
  {
    _useCase = useCase;
    _appStatus = appStatus;
    _logger = logger;
    _getProjectDetailsUseCase = getProjectDetailsUseCase;
    _tokenStorage = tokenStorage;
    _inviteToProjectUseCase = inviteToProjectUseCase;
  }
  public event PropertyChangedEventHandler? PropertyChanged;
  protected bool SetProperty<T>(
          ref T backingStore,
          T value,
          [CallerMemberName] string propertyName = "")
  {
    if (EqualityComparer<T>.Default.Equals(backingStore, value))
      return false;

    backingStore = value;
    OnPropertyChanged(propertyName);
    return true;
  }

  protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

  public async Task FetchCharaDataAsync(string ProjectId)
  {
    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    if (accessToken == null) return;
    var charaItems = await _useCase.ExecuteAsync(accessToken, ProjectId);

    var grouped = charaItems.GroupBy(x => new { x.CharaName, x.MaterialName })
    .Select(g => new CharaDataSummary
    {
      CharaName = g.Key.CharaName,
      MaterialName = g.Key.MaterialName,
      CharaCount = g.Count(),
      SelectedCount = g.Count(x => x.IsSelected == true),
    }).ToList();

    await MainThread.InvokeOnMainThreadAsync(() =>
    {
      CharaDataSummaries.Clear();

      int _count = 0;
      foreach (var item in grouped)
      {
        bool isSelected = false;
        if (_appStatus.CharaName == item.CharaName && _appStatus.MaterialName == item.MaterialName)
        {
          isSelected = true;
        }
        item.Number = _count;
        item.IsOdd = _count % 2 == 1;
        item.IsSelected = isSelected;
        CharaDataSummaries.Add(item);
        _count++;
      }
      _logger.UserAction(_appStatus.UserId, this.GetType().Name, "個別文字の取得", "個別文字を取得しました。", new { ProjectId, _count });
    });

  }

  public async Task SetProjectDetailsAsync(string projectId)
  {
    try
    {
      var tokens = await _tokenStorage.GetTokensAsync();
      var accessToken = tokens?.AccessToken;
      if (accessToken == null) return;
      await _logger.UserAction(_appStatus.UserId, this.GetType().Name, "プロジェクト詳細を取得", "プロジェクト詳細取得を実行します。", new { projectId = projectId, projectName = ProjectName });
      var projectDetails = await _getProjectDetailsUseCase.ExecuteAsync(accessToken, projectId);
      if (projectDetails == null) return;

      ProjectId = projectDetails.Id;
      ProjectName = projectDetails.Name;
      ProjectDescription = projectDetails.Description;
      ProjectFolderId = projectDetails.ProjectFolderId;
      CharaFolderId = projectDetails.CharaFolderId;
      CreatedAt = projectDetails.CreatedAt.ToString("yyyy年MM月dd日");
      UpdatedAt = projectDetails.UpdatedAt.ToString("yyyy年MM月dd日");
      CreatedBy = projectDetails.CreatedBy;
      ParticipantsText = string.Join(", ", projectDetails.Participants);
    }
    catch (Exception ex)
    {
      await _logger.SystemError(ex, _appStatus.UserId, this.GetType().Name, "プロジェクト詳細取得", new { projectId });
      await SnackBarService.Error("プロジェクト詳細の読み込みに失敗しました。");
    }
  }

  public async Task<SimpleApiResult> InviteToProjectAsync(string projectId, string toUserId, string toRoleId)
  {
    try
    {
      var tokens = await _tokenStorage.GetTokensAsync();
      var accessToken = tokens?.AccessToken;
      if (accessToken == null) return new SimpleApiResult(false, "accessTokenエラーが発生しました"); ;

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
