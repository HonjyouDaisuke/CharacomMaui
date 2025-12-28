using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;
using Microsoft.Maui.Storage;

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
      Preferences.Set(AppAccessTokenKey, SimpleCrypto.Encrypt(accessToken));
    else
      Preferences.Remove(AppAccessTokenKey);

    return Task.CompletedTask;
  }

  public Task<string?> GetAccessTokenAsync()
  {
    var encrypted = Preferences.Get(AppAccessTokenKey, null);
    if (encrypted is null) return Task.FromResult<string?>(null);

    try
    {
      return Task.FromResult(SimpleCrypto.Decrypt(encrypted));
    }
    catch
    {
      return Task.FromResult<string?>(null);
    }
  }

  public Task SaveRefreshTokenAsync(string? refreshToken)
  {
    if (refreshToken is not null)
      Preferences.Set(AppRefreshTokenKey, SimpleCrypto.Encrypt(refreshToken));
    else
      Preferences.Remove(AppRefreshTokenKey);

    return Task.CompletedTask;
  }

  public Task<string?> GetRefreshTokenAsync()
  {
    var encrypted = Preferences.Get(AppRefreshTokenKey, null);
    if (encrypted is null) return Task.FromResult<string?>(null);

    try
    {
      return Task.FromResult(SimpleCrypto.Decrypt(encrypted));
    }
    catch
    {
      return Task.FromResult<string?>(null);
    }
  }

  public Task SaveExpireAtAsync(DateTime expireAt)
  {
    Preferences.Set(AppExpireAtKey, SimpleCrypto.Encrypt(expireAt.ToString("O")));
    return Task.CompletedTask;
  }

  public Task<DateTime?> GetExpireAtAsync()
  {
    var encrypted = Preferences.Get(AppExpireAtKey, null);
    if (encrypted is null) return Task.FromResult<DateTime?>(null);

    try
    {
      var decrypted = SimpleCrypto.Decrypt(encrypted);
      if (DateTime.TryParse(decrypted, out var expireAt))
        return Task.FromResult<DateTime?>(expireAt);
    }
    catch { }

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

    if (string.IsNullOrEmpty(accessToken) ||
        string.IsNullOrEmpty(refreshToken) ||
        expireAt == null)
      return null;

    var expiresInSeconds = (int)(expireAt.Value - DateTime.UtcNow).TotalSeconds;

    return new AppTokenResult
    {
      AccessToken = accessToken,
      RefreshToken = refreshToken,
      ExpiresAt = expiresInSeconds
    };
  }

  public Task ClearTokensAsync()
  {
    Preferences.Remove(AppAccessTokenKey);
    Preferences.Remove(AppRefreshTokenKey);
    Preferences.Remove(AppExpireAtKey);
    return Task.CompletedTask;
  }
}
