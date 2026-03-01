namespace CharacomMaui.Application.Interfaces;

public interface IAppLogger
{
  void Info(string message, object? data = null);

  void Debug(string message, object? data = null);

  void Warning(string message, object? data = null);

  void Error(Exception ex, string message, object? data = null);

  /// <summary>
  /// ユーザーアクションログを記録します。
  /// </summary>
  /// <param name="userId">ユーザーID</param>
  /// <param name="screen">画面名</param>
  /// <param name="action">動作</param>
  /// <param name="message">メッセージ</param>
  /// <param name="data">ログデータ</param>
  void UserAction(string userId, string screen, string action, string message, object? data = null);

  /// <summary>
  /// ユーザーアクションのエラーログを記録します。
  /// </summary>
  /// <param name="ex">例外記録</param>
  /// <param name="userId">ユーザーID</param>
  /// <param name="screen">画面名</param>
  /// <param name="action">動作</param>
  /// <param name="data">ログデータ</param>
  void UserActionError(Exception ex, string userId, string screen, string action, object? data = null);

  /// <summary>
  /// システム情報のログを記録します。
  /// </summary>
  /// <param name="userId">ユーザーID</param>
  /// <param name="screen">画面名</param>
  /// <param name="action">動作</param>
  /// <param name="message">メッセージ</param>
  /// <param name="data">ログデータ</param>
  void SystemInfo(string userId, string screen, string action, string message, object? data = null);

  /// <summary>
  /// システム警告のログを記録します。
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="userId">ユーザーID</param>
  /// <param name="screen">画面名</param>
  /// <param name="action">動作</param>
  /// <param name="message">メッセージ</param>
  /// <param name="data">ログデータ</param>
  void SystemWarning(string userId, string screen, string action, string message, object? data = null);

  /// <summary>
  /// システムのエラーログを記録します。
  /// </summary>
  /// <param name="ex">例外記録</param>
  /// <param name="userId">ユーザーID</param>
  /// <param name="screen">画面名</param>
  /// <param name="action">動作</param>
  /// <param name="data">ログデータ</param>
  void SystemError(Exception ex, string userId, string screen, string action, object? data = null);
}