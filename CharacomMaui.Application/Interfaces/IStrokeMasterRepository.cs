namespace CharacomMaui.Application.Interfaces;

using CharacomMaui.Domain.Entities;

public interface IStrokeMasterRepository
{
  Task<SimpleApiResult> UpdateStrokeMasterAsync(string accessToken);
  Task<string> GetStrokeFileIdAsync(string accessToken, string charaName);
}
