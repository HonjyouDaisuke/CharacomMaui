namespace CharacomMaui.Presentation.Services;

using CharacomMaui.Presentation.Interfaces;

public class NotificationPanelService : INotificationPanelService
{
  public event Action? OpenRequested;
  public event Action? CloseRequested;

  private bool _isOpen;

  public void Open()
  {
    if (_isOpen) return;
    _isOpen = true;
    OpenRequested?.Invoke();
  }

  public void Close()
  {
    if (!_isOpen) return;
    _isOpen = false;
    System.Diagnostics.Debug.WriteLine("PanelColseの呼び出し");
    CloseRequested?.Invoke();
  }

  public void Toggle()
  {
    System.Diagnostics.Debug.WriteLine($"Toggled!!{_isOpen}");
    if (_isOpen)
      Close();
    else
      Open();
  }
}
