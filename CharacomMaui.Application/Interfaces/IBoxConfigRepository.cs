namespace CharacomMaui.Application.Interfaces;

public interface IBoxConfigRepository
{
  Task<(string ClientId, string ClientSecret)> GetBoxConfigAsync();
}