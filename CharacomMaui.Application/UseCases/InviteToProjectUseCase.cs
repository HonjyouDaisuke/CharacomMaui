namespace CharacomMaui.Application.UseCases;

using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

public class InviteToProjectUseCase
{
  private readonly IUserProjectsRepository _repo;

  public InviteToProjectUseCase(IUserProjectsRepository repo)
  {
    _repo = repo;
  }

  public async Task<SimpleApiResult> ExecuteAsync(string accessToken, string projectId, string toUserId, string toRoleId)
  {
    return await _repo.InviteToProjectAsync(accessToken, projectId, toUserId, toRoleId);
  }
}