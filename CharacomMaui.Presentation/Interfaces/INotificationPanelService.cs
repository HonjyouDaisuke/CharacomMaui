namespace CharacomMaui.Presentation.Interfaces;

public interface INotificationPanelService
{
  event Action? OpenRequested;
  event Action? CloseRequested;

  void Open();
  void Close();
  void Toggle();
}