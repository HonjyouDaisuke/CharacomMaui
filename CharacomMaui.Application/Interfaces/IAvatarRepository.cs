namespace CharacomMaui.Application.Interfaces;

public interface IAvatarRepository
{
  Task<List<string>> GetAvatarsUrl(string accessToken);
}