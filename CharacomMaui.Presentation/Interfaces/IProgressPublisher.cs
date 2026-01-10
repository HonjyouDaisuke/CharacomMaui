using CharacomMaui.Application.UseCases;
namespace CharacomMaui.Presentation.Interfaces;

public interface IProgressPublisher
{
  event EventHandler<ImageProgress> ProgressChanged;
}