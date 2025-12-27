namespace CharacomMaui.Application.UseCases;

using CharacomMaui.Domain.Entities;
using CharacomMaui.Application.Interfaces;

public class AppStatusUseCase
{
  private readonly AppStatus _status;

  // DI で AppStatus を注入
  public AppStatusUseCase(AppStatus status)
  {
    _status = status;
  }

  // AppStatus の参照を返す
  public AppStatus GetAppStatus() => _status;

  // ユーザー情報をセット
  public void SetUserInfo(AppUser userInfo)
  {
    _status.UserId = userInfo.Id;
    _status.UserName = userInfo.Name;
    _status.UserRole = userInfo.RoleId;
    _status.AvatarUrl = userInfo.PictureUrl;
    _status.UserEmail = userInfo.Email;
  }

  // プロジェクト情報をセット
  public void SetProjectInfo(Project projectInfo)
  {
    _status.ProjectId = projectInfo.Id;
    _status.ProjectName = projectInfo.Name;
    _status.ProjectRole = projectInfo.RoleId;
    _status.ProjectFolderId = projectInfo.FolderId;
    _status.CharaFolderId = projectInfo.CharaFolderId;
  }
}
