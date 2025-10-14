namespace CharacomMaui.Domain.Entities;

public class ImageData
{
    public byte[] RawData { get; }
    public int Width { get; }
    public int Height { get; }

    public ImageData(byte[] rawData, int width, int height)
    {
        RawData = rawData;
        Width = width;
        Height = height;
    }
}
