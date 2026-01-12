using CharacomMaui.Application.Models;

namespace CharacomMaui.Application.Interfaces;

public interface IProjectItemsLoadCoordinator
{
  Task<ProjectItemsLoadResult> LoadProjectItemsAsync(string accessToken);
}
