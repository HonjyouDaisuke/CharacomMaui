public interface IProgressDialogService
{
  Task ShowAsync(string title, string message);
  void Update(string message, double progress);
  Task CloseAsync();
}