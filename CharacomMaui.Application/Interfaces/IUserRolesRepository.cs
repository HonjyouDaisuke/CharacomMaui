namespace CharacomMaui.Application.Interfaces;

using CharacomMaui.Domain.Entities;

public interface IUserRolesRepository
{
  Task<List<UserRole>?> FetchUserRolesAsync(string accessToken);
}
