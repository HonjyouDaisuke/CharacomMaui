using CharacomMaui.Application.UseCases;
using CharacomMaui.Application.Models;

namespace CharacomMaui.Presentation.Interfaces;

public interface IProgressPublisher
{
  event EventHandler<ImageProgress> ProgressChanged;
}