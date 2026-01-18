using System.Collections.ObjectModel;
using System.Xml.Serialization;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.Sessions;
using CharacomMaui.Presentation.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CharacomMaui.Presentation.ViewModels;

public partial class UserListViewModel : ObservableObject
{
  private readonly AppStatus _appStatus;
  [ObservableProperty]
  private ObservableCollection<UserInfoSummary> _users = new();
  private readonly IGetUserInfoUseCase _userInfoUseCase;
  private readonly IAppTokenStorageService _tokenStorage;
  private readonly UserRolesSession _userRolesSession;
  private readonly UpdateUserRoleUseCase _updateUserRoleUseCase;
  private readonly FetchUserRolesUseCase _fetchUserRolesUseCase;
  public ObservableCollection<UserRole> UserRoles { get; } = new();

  public UserListViewModel(
    AppStatus appStatus,
    IGetUserInfoUseCase userInfoUseCase,
    IAppTokenStorageService tokenStorage,
    UpdateUserRoleUseCase updateUserRoleUseCase,
    FetchUserRolesUseCase fetchUserRolesUseCase,
    UserRolesSession userRolesSession)
  {
    _appStatus = appStatus;
    _userInfoUseCase = userInfoUseCase;
    _tokenStorage = tokenStorage;
    _updateUserRoleUseCase = updateUserRoleUseCase;
    _fetchUserRolesUseCase = fetchUserRolesUseCase;
    _userRolesSession = userRolesSession;
  }

  public async Task<bool> UpdateUserRoleAsync(string userId, string userRoleId)
  {
    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    if (accessToken == null) return false;

    System.Diagnostics.Debug.WriteLine($"userId : {userId} useRoleId {userRoleId}");

    var result = await _updateUserRoleUseCase.ExecuteAsync(accessToken, userId, userRoleId);
    if (result.Success == false) return false;

    foreach (var user in Users)
    {
      if (user.Id != userId) continue;

      user.RoleId = userRoleId;
      user.RoleName = _userRolesSession.GetRoleNameFromRoleId(userRoleId);
    }
    return true;
  }
  public async Task FetchUserListAsync()
  {
    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    if (accessToken == null) return;


    // 1. 新しいコレクションを作成してデータを詰め込む
    var res = await _userInfoUseCase.GetUserListAsync(accessToken);
    if (res == null)
    {
      System.Diagnostics.Debug.WriteLine("ユーザーリストの取得に失敗しました");
      return;
    }
    var tempCollection = new ObservableCollection<UserInfoSummary>();
    int _count = 0;
    System.Diagnostics.Debug.WriteLine($"Rolse count = {_userRolesSession.Roles.Count}");
    foreach (var user in res)
    {
      System.Diagnostics.Debug.WriteLine($"roleId {user.RoleId} roleName={_userRolesSession.GetRoleNameFromRoleId(user.RoleId)}");

      var userInfo = new UserInfoSummary
      {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        AvatarUrl = user.PictureUrl,
        RoleId = user.RoleId,
        RoleName = _userRolesSession.GetRoleNameFromRoleId(user.RoleId),
        IsOdd = _count % 2 == 1
      };
      tempCollection.Add(userInfo);
      _count++;
    }
    await MainThread.InvokeOnMainThreadAsync(() =>
 {
   // 2. プロパティごと差し替える（これで View に「全部変わった」と通知が飛ぶ）
   Users = tempCollection;
 });

    foreach (var user in Users)
      System.Diagnostics.Debug.WriteLine($"userName= {user.Name} userId= {user.Id} email= {user.Email}");
    return;
  }

  public UserInfoSummary? GetUserInfoSummaryById(string userId)
  {
    foreach (var user in Users)
    {
      if (user.Id == userId)
      {
        return user;
      }
    }
    return null;
  }
}
