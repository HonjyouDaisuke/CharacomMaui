using CharacomMaui.Application.Interfaces;

namespace CharacomMaui.Application.UseCases;

public class ProcessImageFromBoxUseCase
{
    private readonly ICloudStorageService _cloudStorage;
    private readonly IImageProcessingService _imageProcessing;

    public ProcessImageFromBoxUseCase(
        ICloudStorageService cloudStorage,
        IImageProcessingService imageProcessing)
    {
        _cloudStorage = cloudStorage;
        _imageProcessing = imageProcessing;
    }

    public async Task ExecuteAsync(string fileId, string folderId)
    {
        var raw = await _cloudStorage.DownloadFileAsync(fileId);
        //var img = new ImageData(raw, 0, 0);
        //var filtered = _imageProcessing.ApplyFilter(img, "grayscale");
        //await _cloudStorage.UploadFileAsync(folderId, "filtered.png", filtered.RawData);
    }
}