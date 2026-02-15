namespace CharacomMaui.Application.Interfaces;

using CharacomMaui.Domain.Entities;

public interface IUserProjectsRepository
{
  Task<SimpleApiResult> InviteToProjectAsync(string accessToken, string projectId, string toUserId, string toRoleId);
}