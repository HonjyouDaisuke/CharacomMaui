using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.Models;


namespace CharacomMaui.Presentation.Services;

public class TokenStorageService : ITokenStorageService
{
  private const string AccessTokenKey = "box_access_token";
  private const string RefreshTokenKey = "box_refresh_token";
  private const string ExpireAtKey = "box_expire_at";

  // --- 個別操作 ---

  public Task SaveAccessTokenAsync(string? accessToken)
  {
    if (accessToken is not null)
      Preferences.Set(AccessTokenKey, accessToken);
    else
      Preferences.Remove(AccessTokenKey);

    return Task.CompletedTask;
  }

  public Task<string?> GetAccessTokenAsync()
  {
    var token = Preferences.Get(AccessTokenKey, null);
    return Task.FromResult<string?>(token);
  }

  public Task SaveRefreshTokenAsync(string? refreshToken)
  {
    if (refreshToken is not null)
      Preferences.Set(RefreshTokenKey, refreshToken);
    else
      Preferences.Remove(RefreshTokenKey);

    return Task.CompletedTask;
  }

  public Task<string?> GetRefreshTokenAsync()
  {
    var token = Preferences.Get(RefreshTokenKey, null);
    return Task.FromResult<string?>(token);
  }

  public Task SaveExpireAtAsync(DateTime expireAt)
  {
    Preferences.Set(ExpireAtKey, expireAt.ToString("O")); // ISO8601形式
    return Task.CompletedTask;
  }

  public Task<DateTime?> GetExpireAtAsync()
  {
    var expireAtStr = Preferences.Get(ExpireAtKey, null);
    if (DateTime.TryParse(expireAtStr, out var expireAt))
      return Task.FromResult<DateTime?>(expireAt);

    return Task.FromResult<DateTime?>(null);
  }

  // --- 一括保存 ---
  public async Task SaveTokensAsync(BoxAuthResult tokens)
  {
    await SaveAccessTokenAsync(tokens.AccessToken);
    await SaveRefreshTokenAsync(tokens.RefreshToken);
    await SaveExpireAtAsync(DateTime.UtcNow.AddSeconds(tokens.ExpiresAt));
  }

  public async Task<BoxAuthResult?> GetTokensAsync()
  {
    var accessToken = await GetAccessTokenAsync();
    var refreshToken = await GetRefreshTokenAsync();
    var expireAt = await GetExpireAtAsync();

    if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken) || expireAt == null)
      return null;

    var expiresInSeconds = (int)(expireAt.Value - DateTime.UtcNow).TotalSeconds;
    BoxAuthResult result = new BoxAuthResult();
    result.AccessToken = accessToken;
    result.RefreshToken = refreshToken;
    result.ExpiresAt = expiresInSeconds;
    return result;
  }

  public Task ClearTokensAsync()
  {
    Preferences.Remove(AccessTokenKey);
    Preferences.Remove(RefreshTokenKey);
    Preferences.Remove(ExpireAtKey);
    return Task.CompletedTask;
  }
}
