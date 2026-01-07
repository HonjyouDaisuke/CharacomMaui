namespace CharacomMaui.Application.Coodinators;

using CharacomMaui.Application.Models;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Application.Interfaces;

public class ProjectItemsLoadCoordinator : IProjectItemsLoadCoordinator
{
  private readonly GetProjectCharaItemsUseCase _useCase;
  private readonly AppStatus _appStatus;


  public ProjectItemsLoadCoordinator(GetProjectCharaItemsUseCase useCase, AppStatus appStatus)
  {
    _useCase = useCase;
    _appStatus = appStatus;
  }

  public async Task<ProjectItemsLoadResult> LoadProjectItemsAsync(string accessToken)
  {
    try
    {
      var items = await _useCase.ExecuteAsync(accessToken, _appStatus.ProjectId);

      var charaItems = items
        .Select(x => x.CharaName)
        .Distinct()
        .ToList();

      var materialItems = items
        .Select(x => x.MaterialName)
        .Distinct()
        .ToList();

      var currentItems = items
        .Select(x => new
        {
          x.FileId,
          x.CharaName,
          x.MaterialName,
          x.TimesName
        })
        .Distinct()
        .ToList();

      var charaNames = new List<ProjectItems>();
      foreach (var charaItem in charaItems)
      {
        var count = currentItems.Count(x => x.CharaName == charaItem);
        charaNames.Add(new ProjectItems(
          Name: charaItem,
          Count: count,
          Title: $"{charaItem} ({count})"
        ));
      }

      var materialNames = new List<ProjectItems>();
      foreach (var materialItem in materialItems)
      {
        var count = items.Count(x =>
          x.MaterialName == materialItem &&
          x.CharaName == _appStatus.CharaName);

        materialNames.Add(new ProjectItems(
          Name: materialItem,
          Count: count,
          Title: $"{materialItem} ({count})"
        ));
      }

      var result = new ProjectItemsLoadResult(
        AllCharaData: items,
        CharaNames: charaNames,
        MaterialNames: materialNames
      );

      return result;
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
      throw new Exception(ex.Message, ex);
    }

  }
}