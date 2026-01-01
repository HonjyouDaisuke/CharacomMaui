// IProgressDialogService.cs
public interface IProgressDialogService
{
  Task ShowAsync(string title, string message);
  Task UpdateAsync(string message, double value);
  Task HideAsync();
}