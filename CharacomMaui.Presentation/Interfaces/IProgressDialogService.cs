
namespace CharacomMaui.Presentation.Interfaces;

public interface IProgressDialogService
{
  Task ShowAsync(string title, string message);
  Task UpdateAsync(string message, double progress);
  Task CloseAsync();
}