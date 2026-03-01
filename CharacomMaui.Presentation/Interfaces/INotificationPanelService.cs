namespace CharacomMaui.Presentation.Interfaces;

/// <summary>
/// 通知パネルの開閉要求を扱うサービスのインターフェースです。
/// </summary>
public interface INotificationPanelService
{
  /// <summary>
  /// 通知パネルを開く要求が発生したときに発火します。
  /// </summary>
  event Action? OpenRequested;

  /// <summary>
  /// 通知パネルを閉じる要求が発生したときに発火します。
  /// </summary>
  event Action? CloseRequested;

  /// <summary>
  /// 通知パネルを開く操作を要求します。
  /// </summary>
  void Open();

  /// <summary>
  /// 通知パネルを閉じる操作を要求します。
  /// </summary>
  void Close();

  /// <summary>
  /// 通知パネルの開閉状態を切り替える操作を要求します。
  /// </summary>
  void Toggle();
}