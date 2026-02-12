namespace CharacomMaui.Application.Interfaces;

using CharacomMaui.Domain.Entities;

public interface IProxyLoginService
{
  Task<AppTokenResult> ProxyLogin(string accessToken, AppUser user, string toUserId, string toUserName, string toUserEmail, string toBoxUserId);
}
