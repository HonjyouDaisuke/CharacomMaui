// CharacomMaui.Presentation/ViewModels/BoxItemViewModel.cs
using CharacomMaui.Domain.Entities;
using CharacomMaui.Presentation.Helpers;
using SkiaSharp;
namespace CharacomMaui.Presentation.ViewModels
{
  public class BoxImageItemViewModel
  {
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public ImageSource? Image { get; set; }
    // DomainのBoxItemからViewModelに変換するコンストラクタ

    public BoxImageItemViewModel(BoxImageItem item)
    {
      using var ms = new MemoryStream(item.Image);
      var bmp = SKBitmap.Decode(ms);
      var thumbnail = CharacomMaui.Presentation.Helpers.ImageSourceConverter.FromSKBitmap(bmp);
      Id = item.Id;
      Name = item.Name;
      Type = item.Type;
      Image = thumbnail;
    }
  }
}
