// CharacomMaui.Presentation/ViewModels/BoxItemViewModel.cs
using CharacomMaui.Domain.Entities;
using SkiaSharp;
namespace CharacomMaui.Presentation.ViewModels
{
  public class BoxImageItemViewModel
  {
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public SKBitmap Image { get; set; }
    // DomainのBoxItemからViewModelに変換するコンストラクタ

    public BoxImageItemViewModel(BoxImageItem item)
    {
      using var ms = new MemoryStream(item.Image);
      var bmp = SKBitmap.Decode(ms);

      Id = item.Id;
      Name = item.Name;
      Type = item.Type;
      Image = bmp;
    }
  }
}
