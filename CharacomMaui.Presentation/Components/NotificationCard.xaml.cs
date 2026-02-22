namespace CharacomMaui.Presentation.Components;

using System.Windows.Input;
using CharacomMaui.Presentation.Helpers;
using MauiApp = Microsoft.Maui.Controls.Application;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Views;
using UraniumUI.Dialogs;
using UraniumUI.Dialogs.Mopups;
using CharacomMaui.Domain.Entities;
using System.Runtime.CompilerServices;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Presentation.Interfaces;

public partial class NotificationCard : ContentView
{
  private readonly INotificationService _notificationService;

  public NotificationCard()
  {
    InitializeComponent();
    _notificationService = IPlatformApplication.Current.Services.GetService<INotificationService>();
  }

  private string SettingTypeToIcon(string typeId)
  {
    return typeId switch
    {
      "admin_message" => "\uefd1",
      "comment" => "\ue0b9",
      "project_invite" => "\ue2c9",
      "project_update" => "\ue923",
      "reminder" => "\ue855",
      "system" => "\ue8b8",
      _ => "\ue0b9"
    };
  }
  public static readonly BindableProperty IdProperty =
        BindableProperty.Create(nameof(Id), typeof(string), typeof(NotificationCard), string.Empty);
  public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(NotificationCard), string.Empty);
  public static readonly BindableProperty MessageProperty =
      BindableProperty.Create(nameof(Message), typeof(string), typeof(NotificationCard), string.Empty);
  public static readonly BindableProperty TypeIdProperty =
      BindableProperty.Create(nameof(TypeId), typeof(string), typeof(NotificationCard), string.Empty, propertyChanged: OnTypeIdChanged);
  public static readonly BindableProperty IconProperty =
      BindableProperty.Create(nameof(Icon), typeof(string), typeof(NotificationCard), string.Empty);
  public static readonly BindableProperty IsReadProperty =
      BindableProperty.Create(nameof(IsRead), typeof(bool), typeof(NotificationCard), true, propertyChanged: OnIsReadChanged);

  public string Id { get => (string)GetValue(IdProperty); set => SetValue(IdProperty, value); }
  public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
  public string Message { get => (string)GetValue(MessageProperty); set => SetValue(MessageProperty, value); }
  public string TypeId { get => (string)GetValue(TypeIdProperty); set => SetValue(TypeIdProperty, value); }
  public string Icon { get => (string)GetValue(IconProperty); set => SetValue(IconProperty, value); }
  public bool IsRead { get => (bool)GetValue(IsReadProperty); set => SetValue(IsReadProperty, value); }

  // クリック時の処理
  private async void OnCardTapped(object? sender, EventArgs e)
  {
    System.Diagnostics.Debug.WriteLine($"NotificationCard Tapped: Id={Id}");
    // ここで通知の詳細を表示するダイアログを開く
    if (_notificationService == null)
    {
      System.Diagnostics.Debug.WriteLine("NotificationService が取得できません");
      return;
    }
    _notificationService.RequestOpen(Id, Title, Message, Icon);

  }

  private static void OnTypeIdChanged(BindableObject bindable, object oldValue, object newValue)
  {
    var control = (NotificationCard)bindable;
    var type = newValue as string ?? string.Empty;

    control.Icon = control.SettingTypeToIcon(type);
  }

  private static void OnIsReadChanged(BindableObject bindable, object oldValue, object newValue)
  {
    System.Diagnostics.Debug.WriteLine($"IsRead changed: {newValue}");
  }
}
