namespace CharacomMaui.Application.Interfaces;

using CharacomMaui.Domain.Entities;

public interface ICharaDataRepository
{
  Task<List<CharaData>> GetCharaDataAsync(string accessToken, string projectId);
}
