namespace CharacomMaui.Presentation.Interfaces;

using System.Collections.ObjectModel;
using CharacomMaui.Presentation.Services;
using CharacomMaui.Presentation.Models;
using CharacomMaui.Domain.Entities;


/// <summary>
/// 通知の取得・表示・削除・既読操作などを提供するサービスのインターフェースです。
/// </summary>
public interface INotificationService
{
  /// <summary>
  /// 通知を表示する要求が発生したときに発火します。
  /// </summary>
  event EventHandler<NotificationRequest>? NotificationRequested;

  /// <summary>
  /// 通知の削除要求が発生したときに発火します。引数は削除する通知の ID です。
  /// </summary>
  event EventHandler<string>? DeleteRequested;

  /// <summary>
  /// 現在の通知一覧を表します。
  /// </summary>
  ObservableCollection<NotificationItem> Notifications { get; set; }

  /// <summary>
  /// 通知を表示する要求を発行します。
  /// </summary>
  /// <param name="id">通知の識別子</param>
  /// <param name="title">通知タイトル</param>
  /// <param name="message">通知メッセージ</param>
  /// <param name="icon">通知に表示するアイコンの識別子</param>
  /// <param name="createdAt">通知作成日時（文字列）</param>
  void RequestOpen(string id, string title, string message, string icon, string createdAt);

  /// <summary>
  /// 指定した通知の削除要求を発行します。
  /// </summary>
  /// <param name="id">削除する通知の識別子</param>
  void RequestDelete(string id);

  /// <summary>
  /// 指定した通知を既読にします（サーバーへ既読状態を通知します）。
  /// </summary>
  /// <param name="accessToken">認証用アクセストークン</param>
  /// <param name="id">既読にする通知の識別子</param>
  /// <returns>非同期操作を表す Task</returns>
  Task MarkAsReadAsync(string accessToken, string id);

  /// <summary>
  /// 指定した通知を削除済みにします（サーバー側操作）。
  /// </summary>
  /// <param name="accessToken">認証用アクセストークン</param>
  /// <param name="id">削除する通知の識別子</param>
  /// <returns>非同期操作を表す Task</returns>
  Task MarkAsDeleteAsync(string accessToken, string id);

  /// <summary>
  /// 全ての通知を既読にします。
  /// </summary>
  /// <param name="accessToken">認証用アクセストークン</param>
  /// <returns>非同期操作を表す Task</returns>
  Task AllReadAsync(string accessToken);

  /// <summary>
  /// 通知一覧を初期化または取得します。
  /// </summary>
  /// <param name="accessToken">認証用アクセストークン</param>
  /// <returns>非同期操作を表す Task</returns>
  Task InitNotificationsAsync(string accessToken);
}