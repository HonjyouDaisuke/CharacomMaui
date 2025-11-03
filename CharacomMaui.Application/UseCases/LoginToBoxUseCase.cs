using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.Models;

namespace CharacomMaui.Application.UseCases;

public class LoginToBoxUseCase
{
  private readonly IBoxApiAuthService _authService;

  public LoginToBoxUseCase(IBoxApiAuthService authService)
  {
    _authService = authService;
  }

  /// <summary>
  /// 認可 URL を取得 → ユーザーにリダイレクト → コード取得 → トークン取得。
  /// 最終的にアクセストークンを返す。
  /// </summary>
  public string GetAuthorizationUrl(string clientId, string clientSecret)
  {
    return _authService.GetAuthorizationUrl(clientId, clientSecret);
  }
  public async Task<BoxAuthResult> LoginWithCodeAsync(string code, string redirectUri)
  {
    var result = await _authService.ExchangeCodeForTokenAsync(code, redirectUri);
    return result;
  }

  public async Task<BoxAuthResult> RefreshTokenAsync(string refreshToken)
  {
    var result = await _authService.RefreshTokenAsync(refreshToken);
    return result;
  }

  public async Task<BoxUserResult> GetUserInfoAsync(string accessToken)
  {
    var result = await _authService.GetUserInfoAsync(accessToken);
    return result;
  }

}
