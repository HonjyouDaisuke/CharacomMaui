namespace CharacomMaui.Application.Interfaces;

using CharacomMaui.Domain.Entities;

public interface IBoxTopFolderRepository
{
  Task<List<BoxItem>> GetTopFolders(string asccessToken);
}
