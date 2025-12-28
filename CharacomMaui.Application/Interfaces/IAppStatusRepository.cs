using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Application.Interfaces;

public interface IAppStatusRepository
{
  void SetUserInfo(string userId, string userName, string userRole, string avaterUrl, string userEmail);
  void SetProjectInfo(string projectId, string projectName, string projectRole, string projectFolderId, string charaFolderId);
  AppStatus GetAppStatus();
}
