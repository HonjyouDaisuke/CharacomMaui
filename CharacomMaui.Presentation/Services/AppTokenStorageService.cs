using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.Models;
using CharacomMaui.Domain.Entities;


namespace CharacomMaui.Presentation.Services;

public class AppTokenStorageService : IAppTokenStorageService
{
  private const string AppAccessTokenKey = "app_access_token";
  private const string AppRefreshTokenKey = "app_refresh_token";
  private const string AppExpireAtKey = "app_expire_at";

  // --- 個別操作 ---

  public Task SaveAccessTokenAsync(string? accessToken)
  {
    if (accessToken is not null)
      Preferences.Set(AppAccessTokenKey, accessToken);
    else
      Preferences.Remove(AppAccessTokenKey);

    return Task.CompletedTask;
  }

  public Task<string?> GetAccessTokenAsync()
  {
    var token = Preferences.Get(AppAccessTokenKey, null);
    return Task.FromResult<string?>(token);
  }

  public Task SaveRefreshTokenAsync(string? refreshToken)
  {
    if (refreshToken is not null)
      Preferences.Set(AppRefreshTokenKey, refreshToken);
    else
      Preferences.Remove(AppRefreshTokenKey);

    return Task.CompletedTask;
  }

  public Task<string?> GetRefreshTokenAsync()
  {
    var token = Preferences.Get(AppRefreshTokenKey, null);
    return Task.FromResult<string?>(token);
  }

  public Task SaveExpireAtAsync(DateTime expireAt)
  {
    Preferences.Set(AppExpireAtKey, expireAt.ToString("O")); // ISO8601形式
    return Task.CompletedTask;
  }

  public Task<DateTime?> GetExpireAtAsync()
  {
    var expireAtStr = Preferences.Get(AppExpireAtKey, null);
    if (DateTime.TryParse(expireAtStr, out var expireAt))
      return Task.FromResult<DateTime?>(expireAt);

    return Task.FromResult<DateTime?>(null);
  }

  // --- 一括保存 ---
  public async Task SaveTokensAsync(AppTokenResult tokens)
  {
    await SaveAccessTokenAsync(tokens.AccessToken);
    await SaveRefreshTokenAsync(tokens.RefreshToken);
    await SaveExpireAtAsync(DateTime.UtcNow.AddSeconds(tokens.ExpiresAt));
  }

  public async Task<AppTokenResult?> GetTokensAsync()
  {
    var accessToken = await GetAccessTokenAsync();
    var refreshToken = await GetRefreshTokenAsync();
    var expireAt = await GetExpireAtAsync();

    if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken) || expireAt == null)
      return null;

    var expiresInSeconds = (int)(expireAt.Value - DateTime.UtcNow).TotalSeconds;
    AppTokenResult result = new AppTokenResult();
    result.AccessToken = accessToken;
    result.RefreshToken = refreshToken;
    result.ExpiresAt = expiresInSeconds;
    return result;
  }

  public Task ClearTokensAsync()
  {
    Preferences.Remove(AppAccessTokenKey);
    Preferences.Remove(AppRefreshTokenKey);
    Preferences.Remove(AppExpireAtKey);
    return Task.CompletedTask;
  }
}
