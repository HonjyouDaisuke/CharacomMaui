// CharacomMaui.Presentation/ViewModels/BoxItemViewModel.cs
using CharacomMaui.Domain.Entities;
namespace CharacomMaui.Presentation.ViewModels
{
  public class BoxItemViewModel
  {
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;

    // DomainのBoxItemからViewModelに変換するコンストラクタ
    public BoxItemViewModel(BoxItem item)
    {
      Id = item.Id;
      Name = item.Name;
      Type = item.Type;
    }
  }
}
