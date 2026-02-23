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

  // ========== Icon ==========
  public static readonly BindableProperty IconProperty =
      BindableProperty.Create(
        nameof(Icon),
        typeof(string),
        typeof(NotificationDialog),
        string.Empty);
  public string Icon
  {
    get => (string)GetValue(IconProperty);
    set => SetValue(IconProperty, value);
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

  // ========== CreatedAt ==========
  public static readonly BindableProperty CreatedAtProperty =
      BindableProperty.Create(
        nameof(CreatedAt),
        typeof(string),
        typeof(NotificationDialog),
        string.Empty);
  public string CreatedAt
  {
    get => (string)GetValue(CreatedAtProperty);
    set => SetValue(CreatedAtProperty, value);
  }

  private string _id;
  public string SelectedId = string.Empty;

  public NotificationDialog(string id, string title, string message, string icon, string createdAt)
  {
    InitializeComponent();

    Title = title;
    Message = message;
    Icon = icon;
    CreatedAt = createdAt;
    _id = id;
  }

  private async void OnOkClicked(object sender, EventArgs e)
  {
    SelectedId = _id;
    System.Diagnostics.Debug.WriteLine($"id:{_id} の通知を既読にします");
    // ここで通知を既読にするAPIを呼び出す
    await CloseAsync();
  }
}