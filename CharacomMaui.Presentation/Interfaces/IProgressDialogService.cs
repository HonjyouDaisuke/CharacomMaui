
namespace CharacomMaui.Presentation.Interfaces;

public interface IProgressDialogService
{
  Task<IProgressDialogSession> ShowAsync(string title, string message);
}