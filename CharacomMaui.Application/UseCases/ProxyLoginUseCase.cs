using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Application.Models;

namespace CharacomMaui.Application.UseCases;

public class ProxyLoginUseCase
{
  private readonly IProxyLoginService _proxyLoginService;
  public ProxyLoginUseCase(IProxyLoginService proxyLoginService)
  {
    _proxyLoginService = proxyLoginService;
  }

  public async Task<AppTokenResult> ProxyLoginAsync(string accessToken, AppUser user, string toUserId, string toUserName, string toUserEmail, string toBoxUserId)
  {
    return await _proxyLoginService.ProxyLoginAsync(accessToken, user, toUserId, toUserName, toUserEmail, toBoxUserId);
  }

}
