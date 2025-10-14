namespace CharacomMaui.Application.Interfaces;

public interface ICloudStorageService
{
    Task UploadFileAsync(string folderId, string filePath);
    Task<Stream> DownloadFileAsync(string fileId);
}