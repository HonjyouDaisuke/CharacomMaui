using System.Collections.ObjectModel;
using System.ComponentModel;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Presentation.ViewModels;

public class ProjectDetailViewModel : INotifyPropertyChanged
{
  public ObservableCollection<CharaDataSummary> CharaDataSummaries { get; } = [];
  GetProjectCharaItemsUseCase _useCase;
  public ProjectDetailViewModel(GetProjectCharaItemsUseCase useCase)
  {
    _useCase = useCase;
  }
  public event PropertyChangedEventHandler PropertyChanged;
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
      System.Diagnostics.Debug.WriteLine($"[{item.CharaName}]-{item.MaterialName} : {item.CharaCount}個");
      item.Number = _count;
      CharaDataSummaries.Add(item);
      _count++;
    }
  }
}
