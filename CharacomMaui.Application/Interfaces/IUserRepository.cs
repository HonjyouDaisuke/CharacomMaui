namespace CharacomMaui.Application.Interfaces;

using CharacomMaui.Domain.Entities;

public interface IUserRepository
{
  Task<AppTokenResult> CreateUserAsync(AppUser user);
  Task<AppUser> GetUserInfoAsync(string accessToken);
  Task<string> GetAvatarImgStringAsync(string accessToken);
}
