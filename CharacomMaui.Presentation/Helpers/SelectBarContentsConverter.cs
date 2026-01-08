using CharacomMaui.Application.Models;
using CharacomMaui.Presentation.Models;

namespace CharacomMaui.Presentation.Helpers;

public static class SelectBarContentsConverter
{
  public static SelectBarContents ToSelectedBarContents(ProjectItems source)
  {
    return new SelectBarContents
    {
      Name = source.Name,
      Count = source.Count,
      Title = source.Title,
      IsDisabled = source.Count <= 0
    };
  }
}