namespace CharacomMaui.Application.Interfaces;

using CharacomMaui.Domain.Entities;

public interface IProxyLoginService
{
  Task<AppTokenResult> ProxyLoginAsync(string accessToken, string toUserId);
}
