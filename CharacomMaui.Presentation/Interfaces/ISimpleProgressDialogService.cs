
namespace CharacomMaui.Presentation.Interfaces;

public interface ISimpleProgressDialogService
{
  Task ShowAsync(string title, string message);
  Task CloseAsync();
}