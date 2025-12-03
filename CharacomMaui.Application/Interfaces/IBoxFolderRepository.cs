namespace CharacomMaui.Application.Interfaces;

using CharacomMaui.Domain.Entities;

public interface IBoxFolderRepository
{
  Task<List<BoxItem>> GetFolderItemsAsync(string asccessToken, string? folderName = null);
}
