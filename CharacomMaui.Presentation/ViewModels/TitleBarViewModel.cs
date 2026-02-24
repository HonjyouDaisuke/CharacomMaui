using System.ComponentModel;
using System.Runtime.CompilerServices;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Presentation.Interfaces;
using CharacomMaui.Presentation.Models;
using CharacomMaui.Presentation.Services;
using CharacomMaui.Application.Sessions;
using MauiApp = Microsoft.Maui.Controls.Application;

namespace CharacomMaui.Presentation.ViewModels;

public class TitleBarViewModel : INotifyPropertyChanged
{
  private readonly AppStatusNotifier _notifier;
  private readonly AppStatus _appStatus;
  private readonly ProxyLoginUseCase _proxyLoginUseCase;
  private readonly ProxyLogoutUseCase _proxyLogoutUseCase;
  private readonly IAppTokenStorageService _tokenStorage;
  private readonly IGetUserInfoUseCase _getUserInfoUseCase;
  private readonly UserRolesSession _userRolesSession;

  // バインディング用プロパティ
  private string titleString = string.Empty;
  public string TitleString
  {
    get => titleString;
    set
    {
      if (titleString != value)
      {
        titleString = value;
        OnPropertyChanged();
      }
    }
  }

  // avaterText
  private string avatarString = string.Empty;
  public string AvatarString
  {
    get => avatarString;
    set
    {
      if (avatarString != value)
      {
        avatarString = value;
        OnPropertyChanged();
      }
    }
  }

  private string avatarUrl = string.Empty;
  public string AvatarUrl
  {
    get => avatarUrl;
    set
    {
      if (avatarUrl != value)
      {
        avatarUrl = value;
        OnPropertyChanged();
      }
    }
  }

  public string ProxyIcon =>
        IsProxy
            ? Icons.ProxyLogoutIcon
            : Icons.ProxyLoginIcon;
  private ImageSource? avatarImageSource;
  public ImageSource? AvatarImageSource
  {
    get => avatarImageSource;
    set
    {
      if (avatarImageSource != value)
      {
        avatarImageSource = value;
        OnPropertyChanged();
      }
    }
  }

  public string NotificationIcon => "\ue7f4"; // ← 通知アイコン

  public string ProjectName => _notifier.ProjectName;
  public bool IsProxy => _notifier.IsProxy;

  public INotificationService NotificationService { get; }
  public bool IsAdmin => string.Equals(_appStatus.UserRole, "admin",
        StringComparison.OrdinalIgnoreCase);
  public bool IsProxyIconVisible => IsProxy || IsAdmin;
  public TitleBarViewModel(AppStatusNotifier notifier,
                           AppStatus appStatus,
                           ProxyLoginUseCase proxyLoginUseCase,
                           ProxyLogoutUseCase proxyLogoutUseCase,
                           IAppTokenStorageService tokenStorage,
                           INotificationService notificationService,
                           UserRolesSession userRolesSession,
                           IGetUserInfoUseCase getUserInfoUseCase)
  {
    System.Diagnostics.Debug.WriteLine($"[VM] TitleBarViewModel created: {GetHashCode()}");
    _notifier = notifier;
    _appStatus = appStatus;
    NotificationService = notificationService;
    _proxyLoginUseCase = proxyLoginUseCase;
    _proxyLogoutUseCase = proxyLogoutUseCase;
    _getUserInfoUseCase = getUserInfoUseCase;
    _userRolesSession = userRolesSession;
    _tokenStorage = tokenStorage;
    // AppStatusNotifier の変更を購読
    _notifier.PropertyChanged += (_, e) =>
    {
      if (e.PropertyName == nameof(AppStatusNotifier.ProjectName)
          || e.PropertyName == nameof(AppStatusNotifier.IsProxy)
          || e.PropertyName == nameof(AppStatusNotifier.FromUserName)
          || e.PropertyName == nameof(AppStatusNotifier.UserName))
      {
        TitleString = MakeTitleString();
      }

      if (e.PropertyName == nameof(AppStatusNotifier.AvatarUrl))
      {
        AvatarUrl = _notifier.AvatarUrl;
      }
      if (e.PropertyName == nameof(AppStatusNotifier.IsProxy))
      {
        OnPropertyChanged(nameof(IsProxy));
        OnPropertyChanged(nameof(ProxyIcon));
        OnPropertyChanged(nameof(IsProxyIconVisible));
        OnPropertyChanged(nameof(IsAdmin));
      }
    };


    TitleString = MakeTitleString();
    AvatarString = MakeAvatarString();
    AvatarUrl = MakeAvatarUrl();
  }

  private string MakeAvatarString()
  {
    System.Diagnostics.Debug.WriteLine($"UserId{_appStatus.UserId}");

    if (string.IsNullOrEmpty(_appStatus.UserId) || _appStatus.UserId.Length < 2)
    {
      return _appStatus.UserId ?? string.Empty;
    }
    return _appStatus.UserId.Substring(0, 2);
  }

  private string MakeAvatarUrl()
  {
    return _notifier.AvatarUrl;
  }

  private string MakeTitleString()
  {
    System.Diagnostics.Debug.WriteLine($"Make String !!{_notifier.ProjectName}");
    var proxyMessage = _notifier.IsProxy ? $" 代理ログイン中({_notifier.FromUserName})" : "";
    string projectTitle = string.IsNullOrEmpty(_notifier.ProjectName)
        ? ""
        : " - " + _notifier.ProjectName;

    projectTitle += proxyMessage;
    return "CharacomMaui" + projectTitle;
  }

  private void SetNotifier(AppUser newUser)
  {
    _notifier.UserId = newUser.Id;
    _notifier.UserName = newUser.Name;
    _notifier.UserEmail = newUser.Email;
    _notifier.AvatarUrl = newUser.PictureUrl;
    _appStatus.UserRole = newUser.RoleId;
  }

  private async Task SetTokenStorageAsync(AppTokenResult tokens)
  {
    await _tokenStorage.SaveTokensAsync(new AppTokenResult
    {
      AccessToken = tokens.AccessToken,
      RefreshToken = tokens.RefreshToken,
      ExpiresAt = tokens.ExpiresAt,
    });
  }

  public async Task ProxyLoginAsync(string proxyUserId)
  {
    System.Diagnostics.Debug.WriteLine($"ProxyLogin: {proxyUserId}");
    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    if (accessToken == null) return;

    // 元のユーザー名を保存
    var fromUserName = _appStatus.UserName ?? string.Empty;

    var res = await _proxyLoginUseCase.ProxyLoginAsync(accessToken, proxyUserId);

    if (!res.Success)
    {
      System.Diagnostics.Debug.WriteLine($"ProxyLogin失敗: {res.Message}");
      await SnackBarService.Error($"代理ログインに失敗しました: {res.Message}");
      return;
    }

    var newUser = await _getUserInfoUseCase.GetUserInfoAsync(res.AccessToken);
    if (newUser == null) return;

    await SetTokenStorageAsync(res);

    SetNotifier(newUser);
    _notifier.IsProxy = true;
    _notifier.FromUserName = fromUserName;
    await SnackBarService.Success($"Proxy Login {newUser.Name} に代理ログインしました。", 500);
    MauiApp.Current!.Windows[0].Page = new AppShell();
  }

  public async Task ProxyLogoutAsync()
  {
    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    if (accessToken == null) return;

    var result = await _proxyLogoutUseCase.ProxyLogoutAsync(accessToken);
    if (result == null || !result.Success)
    {
      System.Diagnostics.Debug.WriteLine("Logout処理に失敗しました");
      if (result != null)
      {
        System.Diagnostics.Debug.WriteLine($"Message: {result.Message}");
      }
      return;
    }

    await SetTokenStorageAsync(result);

    var newUser = await _getUserInfoUseCase.GetUserInfoAsync(result.AccessToken);
    if (newUser == null)
    {
      System.Diagnostics.Debug.WriteLine("元のユーザー情報の取得に失敗しました");
      return;
    }

    SetNotifier(newUser);
    _notifier.IsProxy = false;
    await SnackBarService.Success($"Proxy Logout {newUser.Name} に戻りました。", 500);
    MauiApp.Current!.Windows[0].Page = new AppShell();
  }
  public event PropertyChangedEventHandler? PropertyChanged;

  protected void OnPropertyChanged([CallerMemberName] string? name = null)
      => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
