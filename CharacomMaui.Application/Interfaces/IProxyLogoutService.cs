namespace CharacomMaui.Application.Interfaces;

using CharacomMaui.Domain.Entities;

public interface IProxyLogoutService
{
  Task<AppTokenResult> ProxyLogoutAsync(string accessToken);
}
