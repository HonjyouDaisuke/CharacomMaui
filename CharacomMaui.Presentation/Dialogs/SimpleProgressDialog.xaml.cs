using CommunityToolkit.Maui.Views;
using CharacomMaui.Domain.Entities;
using UraniumUI.Dialogs;

namespace CharacomMaui.Presentation.Dialogs;

public partial class SimpleProgressDialog : Popup
{
  // BindableProperty: Title
  public static readonly BindableProperty DialogTitleProperty =
      BindableProperty.Create(nameof(DialogTitle), typeof(string), typeof(SimpleProgressDialog), string.Empty);

  public string DialogTitle
  {
    get => (string)GetValue(DialogTitleProperty);
    set => SetValue(DialogTitleProperty, value);
  }

  // BindableProperty: Message
  public static readonly BindableProperty MessageProperty =
      BindableProperty.Create(nameof(Message), typeof(string), typeof(SimpleProgressDialog), string.Empty);

  public string Message
  {
    get => (string)GetValue(MessageProperty);
    set => SetValue(MessageProperty, value);
  }

  public SimpleProgressDialog(string title, string message)
  {
    InitializeComponent();
    DialogTitle = title;
    Message = message;

  }
}