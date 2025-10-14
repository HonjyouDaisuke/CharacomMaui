using CharacomMaui.Application.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace CharacomMaui.Infrastructure.Services;

public class ImageProcessingService : IImageProcessingService
{
    public async Task<Stream> ConvertToGrayscaleAsync(Stream imageStream)
    {
        // 仮実装：入力をそのまま返す
        var ms = new MemoryStream();
        await imageStream.CopyToAsync(ms);
        ms.Position = 0;
        return ms;
    }

    public async Task<Stream> BinarizeAsync(Stream imageStream, int threshold = 128)
    {
        // 仮実装：入力をそのまま返す
        var ms = new MemoryStream();
        await imageStream.CopyToAsync(ms);
        ms.Position = 0;
        return ms;
    }

    public async Task<Stream> ResizeAsync(Stream imageStream, int width, int height)
    {
        // 仮実装：入力をそのまま返す
        var ms = new MemoryStream();
        await imageStream.CopyToAsync(ms);
        ms.Position = 0;
        return ms;
    }

    public async Task<Stream> OverlayAsync(Stream baseImage, Stream overlayImage)
    {
        // 仮実装：ベース画像をそのまま返す
        var ms = new MemoryStream();
        await baseImage.CopyToAsync(ms);
        ms.Position = 0;
        return ms;
    }

    public byte[] ApplyFilter(byte[] imageData, string str)
    {
        // 仮実装：ここで実際の画像処理を行う
        return imageData;
    }
}
