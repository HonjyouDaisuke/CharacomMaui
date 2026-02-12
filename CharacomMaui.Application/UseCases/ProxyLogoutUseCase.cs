using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Application.Models;

namespace CharacomMaui.Application.UseCases;

public class ProxyLogoutUseCase
{
  private readonly IProxyLogoutService _proxyLogoutService;
  public ProxyLogoutUseCase(IProxyLogoutService proxyLogoutService)
  {
    _proxyLogoutService = proxyLogoutService;
  }

  public async Task<AppTokenResult> ProxyLogoutAsync(string accessToken)
  {
    var result = await _proxyLogoutService.ProxyLogout(accessToken);
    System.Diagnostics.Debug.WriteLine($"ProxyLogoutAsync accessToken = {result.AccessToken}");
    return result;
  }

}
