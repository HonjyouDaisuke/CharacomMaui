// CharacomMaui.Application/Interfaces/IBoxApiRepository.cs
using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Application.Interfaces
{
  // Domainに依存してBoxItemを返すようにするのも可
  public interface IBoxApiRepository
  {
    /// <summary>
    /// 指定フォルダのBoxアイテム一覧を取得
    /// </summary>
    /// <param name="accessToken">Box API用アクセストークン</param>
    /// <param name="folderId">取得するフォルダID</param>
    /// <returns>Boxアイテム一覧</returns>
    Task<List<BoxItem>> GetFolderItemsAsync(string accessToken, string folderId);
    Task<List<BoxImageItem>> GetJpgImagesAsync(string accessToken, string folderId);
  }
}
