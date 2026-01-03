using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace CharacomMaui.Presentation.Interfaces;

public interface IProgressDialogService
{
  void SetHost(Page page);
  Task ShowAsync(string title, string message);
  Task UpdateAsync(string message, double progress);
  Task CloseAsync();
}