using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.Models;

namespace CharacomMaui.Application.UseCases;

public class LoginToBoxUseCase
{
  private readonly IBoxAuthService _authService;

  public LoginToBoxUseCase(IBoxAuthService authService)
  {
    _authService = authService;
  }

  /// <summary>
  /// 認可 URL を取得 → ユーザーにリダイレクト → コード取得 → トークン取得。
  /// 最終的にアクセストークンを返す。
  /// </summary>
  public string GetAuthorizationUrl() => _authService.GetAuthorizationUrl();

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
}
