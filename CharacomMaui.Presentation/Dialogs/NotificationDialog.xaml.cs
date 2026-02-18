using CommunityToolkit.Maui.Views;
using CharacomMaui.Domain.Entities;

namespace CharacomMaui.Presentation.Dialogs;

public partial class NotificationDialog : Popup
{
  public bool IsConfirmed { get; set; }
  // ========== Title ==========
  public static readonly BindableProperty TitleProperty =
      BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(NotificationDialog),
        string.Empty);
  public string Title
  {
    get => (string)GetValue(TitleProperty);
    set => SetValue(TitleProperty, value);
  }

  // ========== Message ==========
  public static readonly BindableProperty MessageProperty =
      BindableProperty.Create(
        nameof(Message),
        typeof(string),
        typeof(NotificationDialog),
        string.Empty);
  public string Message
  {
    get => (string)GetValue(MessageProperty);
    set => SetValue(MessageProperty, value);
  }

  private string _id;
  private string _icon;

  public NotificationDialog(string id, string title, string message, string icon)
  {
    InitializeComponent();

    Title = title;
    Message = message;
    _icon = icon;
    _id = id;
  }

  private async void OnOkClicked(object sender, EventArgs e)
  {
    System.Diagnostics.Debug.WriteLine($"id:{_id} の通知を既読にします");
    // ここで通知を既読にするAPIを呼び出す
    await CloseAsync();
  }
}