
namespace CharacomMaui.Presentation.Interfaces;

/// <summary>
/// プログレスダイアログを表示するためのサービスインターフェースです。
/// </summary>
/// <remarks>
/// このインターフェースは、非同期的にプログレスダイアログを表示するためのメソッドを提供します。
/// </remarks>
/// <param name="title">ダイアログのタイトル。</param>
/// <param name="message">ダイアログに表示するメッセージ。</param>
/// <returns>表示されたプログレスダイアログのセッションを表すタスク。</returns>
public interface IProgressDialogService
{
  Task<IProgressDialogSession> ShowAsync(string title, string message);
}