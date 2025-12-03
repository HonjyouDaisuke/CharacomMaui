namespace CharacomMaui.Application.Interfaces;

using CharacomMaui.Domain.Entities;

public interface IStandardMasterRepository
{
  Task<SimpleApiResult> UpdateStandardMasterAsync(string accessToken);
  Task<string> GetStandardFileIdAsync(string accessToken, string charaName);
}
