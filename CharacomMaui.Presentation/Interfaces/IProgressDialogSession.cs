namespace CharacomMaui.Presentation.Interfaces;

public interface IProgressDialogSession : IAsyncDisposable
{
  Task UpdateAsync(string message, double progress);
}