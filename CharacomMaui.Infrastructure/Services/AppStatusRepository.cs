using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;
using System.IO;
using System.Threading.Tasks;

namespace CharacomMaui.Infrastructure.Services;

public class AppStatusRepository : IAppStatusRepository
{
  private AppStatus _appStatus = new AppStatus();

  public void SetUserInfo(string userId, string userName, string userRole, string avaterUrl, string userEmail)
  {
    _appStatus.UserId = userId;
    _appStatus.UserName = userName;
    _appStatus.UserRole = userRole;
    _appStatus.UserEmail = userEmail;
    _appStatus.AvatarUrl = avaterUrl;
    System.Diagnostics.Debug.WriteLine($"ユーザをセットしました.{_appStatus.UserName}");
  }

  public void SetProjectInfo(string projectId, string projectName, string projectRole, string projectFolderId, string charaFolderId)
  {
    _appStatus.ProjectId = projectId;
    _appStatus.ProjectName = projectName;
    _appStatus.ProjectRole = projectRole;
    _appStatus.ProjectFolderId = projectFolderId;
    _appStatus.CharaFolderId = charaFolderId;
  }

  public AppStatus GetAppStatus()
  {
    return _appStatus;
  }
}