using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Application.Models;

public sealed record ProjectItemsLoadResult(
  List<CharaData?> AllCharaData,
  List<ProjectItems?> CharaNames,
  List<ProjectItems?> MaterialNames
);

public sealed record ProjectItems(
  string Name,
  int Count,
  string Title
);