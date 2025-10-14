namespace CharacomMaui.Application.Interfaces;

using System.IO;
using System.Threading.Tasks;

public interface IImageProcessingService
{
    /// <summary>
    /// 指定した画像をグレースケールに変換します。
    /// </summary>
    /// <param name="imageStream">入力画像のストリーム。</param>
    /// <returns>変換後の画像ストリーム。</returns>
    Task<Stream> ConvertToGrayscaleAsync(Stream imageStream);

    /// <summary>
    /// 指定した画像を二値化します。
    /// </summary>
    /// <param name="imageStream">入力画像のストリーム。</param>
    /// <param name="threshold">しきい値（0〜255）。</param>
    /// <returns>二値化後の画像ストリーム。</returns>
    Task<Stream> BinarizeAsync(Stream imageStream, int threshold = 128);

    /// <summary>
    /// 指定した画像をリサイズします。
    /// </summary>
    /// <param name="imageStream">入力画像のストリーム。</param>
    /// <param name="width">リサイズ後の幅。</param>
    /// <param name="height">リサイズ後の高さ。</param>
    /// <returns>リサイズ後の画像ストリーム。</returns>
    Task<Stream> ResizeAsync(Stream imageStream, int width, int height);

    /// <summary>
    /// 2枚の画像をオーバーレイ合成します。
    /// </summary>
    /// <param name="baseImage">ベース画像。</param>
    /// <param name="overlayImage">オーバーレイする画像。</param>
    /// <returns>合成後の画像ストリーム。</returns>
    Task<Stream> OverlayAsync(Stream baseImage, Stream overlayImage);

    byte[] ApplyFilter(byte[] imageData, string str);
}
