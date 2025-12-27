namespace CharacomMaui.Application.Interfaces;

using CharacomMaui.Domain.Entities;
public interface IGetUserInfoUseCase
{
  Task<AppUser> GetUserInfoAsync(string accessToken);
}