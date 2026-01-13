using System.Collections.ObjectModel;
using System.Xml.Serialization;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Application.Interfaces;
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
  public UserListViewModel(AppStatus appStatus, IGetUserInfoUseCase userInfoUseCase, IAppTokenStorageService tokenStorage)
  {
    _appStatus = appStatus;
    _userInfoUseCase = userInfoUseCase;
    _tokenStorage = tokenStorage;
  }

  public async Task FetchUserListAsync()
  {
    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    if (accessToken == null) return;

    await MainThread.InvokeOnMainThreadAsync(async () =>
    {
      // 1. 新しいコレクションを作成してデータを詰め込む
      var res = await _userInfoUseCase.GetUserListAsync(accessToken);
      var tempCollection = new ObservableCollection<UserInfoSummary>();
      int _count = 0;
      foreach (var user in res)
      {
        var userInfo = new UserInfoSummary
        {
          Id = user.Id,
          Name = user.Name,
          Email = user.Email,
          AvatarUrl = user.PictureUrl,
          RoleId = user.RoleId,
          IsOdd = _count % 2 == 1
        };
        tempCollection.Add(userInfo);
        _count++;
      }
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
