using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Application.Sessions;

public class UserRolesSession
{
  private List<UserRole> _roles = new();

  public IReadOnlyList<UserRole> Roles => _roles;

  public void SetRoles(IEnumerable<UserRole> roles)
  {
    _roles = roles.ToList();
  }

  public string GetRoleIdFromRoleName(string roleName)
  {
    if (roleName == null) return string.Empty;
    foreach (UserRole role in _roles)
    {
      if (role.Name == roleName) return role.Id;
    }
    return string.Empty;
  }

  public string GetRoleNameFromRoleId(string roleId)
  {
    if (roleId == null) return string.Empty;
    foreach (UserRole role in _roles)
    {
      if (role.Id == roleId) return role.Name;
    }
    return string.Empty;
  }
}
