namespace CharacomMaui.Application.Interfaces;

using CharacomMaui.Domain.Entities;

public interface IProjectRolesRepository
{
  Task<List<ProjectRole>?> FetchProjectRolesAsync(string accessToken);
}
