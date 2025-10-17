// CharacomMaui.Application/Interfaces/IBoxAuthService.cs
using CharacomMaui.Application.Models;

namespace CharacomMaui.Application.Interfaces;

public interface IBoxApiAuthService
{
  /// <summary>
  /// 認可 URL を生成して返す。ユーザーをこのURLにリダイレクトさせる。
  /// </summary>
  string GetAuthorizationUrl(string clientId, string clientSecret);

  /// <summary>
  /// 認可コードを使ってアクセストークンを取得する。
  /// </summary>
  /// <param name="authorizationCode">認可コード</param>
  /// <param name="redirectUri">リダイレクト URI（登録済のものと一致させる）</param>
  Task<BoxAuthResult> ExchangeCodeForTokenAsync(string authorizationCode, string redirectUri);

  /// <summary>
  /// リフレッシュトークンを使ってアクセストークンを再取得する。
  /// </summary>
  Task<BoxAuthResult> RefreshTokenAsync(string refreshToken);

  void SetBoxKeyString(string clientId, string clientSecret);
}


