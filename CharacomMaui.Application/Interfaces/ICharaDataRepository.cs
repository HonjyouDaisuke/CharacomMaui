namespace CharacomMaui.Application.Interfaces;

using CharacomMaui.Domain.Entities;

public interface ICharaDataRepository
{
  Task<List<CharaData>> GetCharaDataAsync(string accessToken, string projectId);
  Task<SimpleApiResult> UpdateSelectdCharaAsync(string accessToken, string charaId, bool isSelected);
}
