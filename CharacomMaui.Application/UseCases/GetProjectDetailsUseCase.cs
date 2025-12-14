using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Application.UseCases;

public class GetProjectDetailsUseCase
{
  private readonly IProjectRepository _repository;

  public GetProjectDetailsUseCase(IProjectRepository repository)
  {
    _repository = repository;
  }

  public async Task<ProjectDetails?> ExecuteAsync(string accessToken, string projectId)
  {
    return await _repository.GetProjectDetailsAsync(accessToken, projectId);
  }
}