using System.Collections.ObjectModel;
using System.ComponentModel;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Presentation.ViewModels;

public class ProjectDetailViewModel : INotifyPropertyChanged
{
  public ObservableCollection<CharaDataSummary> CharaDataSummaries { get; } = [];
  private readonly AppStatus _appStatus;
  GetProjectCharaItemsUseCase _useCase;
  /// <summary>
  /// Initializes a new ProjectDetailViewModel with the given use case and application status.
  /// </summary>
  /// <param name="useCase">Use case responsible for retrieving project character items.</param>
  /// <param name="appStatus">Application status used to determine per-item selection state.</param>
  public ProjectDetailViewModel(GetProjectCharaItemsUseCase useCase, AppStatus appStatus)
  {
    _useCase = useCase;
    _appStatus = appStatus;
  }
  public event PropertyChangedEventHandler PropertyChanged;
  /// <summary>
  /// Populates the ViewModel's CharaDataSummaries by fetching character items for the given project and grouping them by character and material, while applying selection state from the current AppStatus.
  /// </summary>
  /// <param name="ProjectId">The identifier of the project whose character items should be fetched.</param>
  /// <remarks>
  /// Clears the existing CharaDataSummaries, adds one CharaDataSummary per distinct (CharaName, MaterialName) group, and sets each item's Number and IsSelected according to the current AppStatus. Updates CharaCount and SelectedCount for each group.
  /// </remarks>
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
}