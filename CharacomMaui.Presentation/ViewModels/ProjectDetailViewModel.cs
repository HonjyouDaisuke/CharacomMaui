using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CharacomMaui.Presentation.Models;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Presentation.Services;
using CharacomMaui.Domain.Entities;
namespace CharacomMaui.Presentation.ViewModels;

public class ProjectDetailViewModel : INotifyPropertyChanged
{
  public ObservableCollection<CharaDataSummary> CharaDataSummaries { get; } = [];
  private readonly AppStatus _appStatus;
  GetProjectCharaItemsUseCase _useCase;
  GetProjectDetailsUseCase _getProjectDetailsUseCase;
  private string _projectId;
  public string ProjectId
  {
    get => _projectId;
    set => SetProperty(ref _projectId, value);
  }
  private string _projectName;
  public string ProjectName
  {
    get => _projectName;
    set => SetProperty(ref _projectName, value);
  }

  private string _projectDescription;
  public string ProjectDescription
  {
    get => _projectDescription;
    set => SetProperty(ref _projectDescription, value);
  }
  private string _projectFolderId;
  public string ProjectFolderId
  {
    get => _projectFolderId;
    set => SetProperty(ref _projectFolderId, value);
  }
  private string _charaFolderId;
  public string CharaFolderId
  {
    get => _charaFolderId;
    set => SetProperty(ref _charaFolderId, value);
  }
  private string _createdAt;
  public string CreatedAt
  {
    get => _createdAt;
    set => SetProperty(ref _createdAt, value);
  }

  private string _updatedAt;
  public string UpdatedAt
  {
    get => _updatedAt;
    set => SetProperty(ref _updatedAt, value);
  }

  private string _createdBy;
  public string CreatedBy
  {
    get => _createdBy;
    set => SetProperty(ref _createdBy, value);
  }
  public ProjectDetailViewModel(GetProjectCharaItemsUseCase useCase, AppStatus appStatus, GetProjectDetailsUseCase getProjectDetailsUseCase)
  {
    _useCase = useCase;
    _appStatus = appStatus;
    _getProjectDetailsUseCase = getProjectDetailsUseCase;
  }
  public event PropertyChangedEventHandler PropertyChanged;
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
    var accessToken = Preferences.Get("app_access_token", string.Empty);
    var charaItems = await _useCase.ExecuteAsync(accessToken, ProjectId);
    System.Diagnostics.Debug.WriteLine($"Count = {charaItems.Count}");

    var grouped = charaItems.GroupBy(x => new { x.CharaName, x.MaterialName })
    .Select(g => new CharaDataSummary
    {
      CharaName = g.Key.CharaName,
      MaterialName = g.Key.MaterialName,
      CharaCount = g.Count(),
      SelectedCount = g.Count(x => x.IsSelected == true),
    }).ToList();
    CharaDataSummaries.Clear();

    int _count = 0;
    foreach (var item in grouped)
    {
      bool isSelected = false;
      if (_appStatus.CharaName == item.CharaName && _appStatus.MaterialName == item.MaterialName)
      {
        isSelected = true;
      }
      System.Diagnostics.Debug.WriteLine($"[{item.CharaName}]-{item.MaterialName} : {item.CharaCount}個 IsSelected = {isSelected}");
      item.Number = _count;
      item.IsSelected = isSelected;
      CharaDataSummaries.Add(item);
      _count++;
    }
    System.Diagnostics.Debug.WriteLine($"appStatus = {_appStatus}");
    System.Diagnostics.Debug.WriteLine($"行数は；；；{_count} CharaDataSummariesCount = {CharaDataSummaries.Count}");
  }

  public async Task SetProjectDatailsAsync(string projectId)
  {
    var accessToken = Preferences.Get("app_access_token", string.Empty);
    var projectDetails = await _getProjectDetailsUseCase.ExecuteAsync(accessToken, projectId);
    System.Diagnostics.Debug.WriteLine($"1ProjectId = {projectId}");
    System.Diagnostics.Debug.WriteLine($"1ProjectName = {ProjectName}");
    if (projectDetails == null) return;

    ProjectId = projectDetails.Id;
    ProjectName = projectDetails.Name;
    ProjectDescription = projectDetails.Description;
    ProjectFolderId = projectDetails.ProjectFolderId;
    CharaFolderId = projectDetails.CharaFolderId;
    CreatedAt = projectDetails.CreatedAt.ToString("yyyy年MM月dd日");
    UpdatedAt = projectDetails.UpdatedAt.ToString("yyyy年MM月dd日");
    CreatedBy = projectDetails.CreatedBy;
    System.Diagnostics.Debug.WriteLine($"2ProjectId = {projectId}");
    System.Diagnostics.Debug.WriteLine($"2ProjectName = {ProjectName} ?? ?? {projectDetails.Name}");

  }
}
