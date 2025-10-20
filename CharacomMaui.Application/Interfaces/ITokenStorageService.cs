using CharacomMaui.Application.Models;

namespace CharacomMaui.Application.Interfaces;

public interface ITokenStorageService
{
  // --- 個別アクセス ---
  Task SaveAccessTokenAsync(string? accessToken);
  Task<string?> GetAccessTokenAsync();

  Task SaveRefreshTokenAsync(string? refreshToken);
  Task<string?> GetRefreshTokenAsync();

  Task SaveExpireAtAsync(DateTime expireAt);
  Task<DateTime?> GetExpireAtAsync();

  // --- 一括操作 ---
  Task SaveTokensAsync(BoxAuthResult tokens);
  Task<BoxAuthResult?> GetTokensAsync();
  Task ClearTokensAsync();
}