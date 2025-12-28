namespace CharacomMaui.Application.Interfaces;

using CharacomMaui.Domain.Entities;

public interface IUserRepository
{
  Task<AppTokenResult> CreateUserAsync(AppUser user);
  Task<AppUser> GetUserInfoAsync(string accessToken);
  Task<SimpleApiResult> UpdateUserInfoAsync(string accessToken, string userId, string userName, string userEmail, string avatarUrl);
}
