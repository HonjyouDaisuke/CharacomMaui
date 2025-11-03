using CharacomMaui.Application.Models;
using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Application.Interfaces;

public interface IAppTokenStorageService
{
  // --- 個別アクセス ---
  Task SaveAccessTokenAsync(string? accessToken);
  Task<string?> GetAccessTokenAsync();

  Task SaveRefreshTokenAsync(string? refreshToken);
  Task<string?> GetRefreshTokenAsync();

  Task SaveExpireAtAsync(DateTime expireAt);
  Task<DateTime?> GetExpireAtAsync();

  // --- 一括操作 ---
  Task SaveTokensAsync(AppTokenResult tokens);
  Task<AppTokenResult?> GetTokensAsync();
  Task ClearTokensAsync();
}