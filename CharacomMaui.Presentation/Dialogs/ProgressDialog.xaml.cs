using CommunityToolkit.Maui.Views;
using CharacomMaui.Domain.Entities;
using UraniumUI.Dialogs;

namespace CharacomMaui.Presentation.Dialogs;

public partial class ProgressDialog : Popup
{
  // BindableProperty: Title
  public static readonly BindableProperty TitleProperty =
      BindableProperty.Create(nameof(Title), typeof(string), typeof(ProgressDialog), string.Empty);

  public string Title
  {
    get => (string)GetValue(TitleProperty);
    set => SetValue(TitleProperty, value);
  }

  // BindableProperty: Message
  public static readonly BindableProperty MessageProperty =
      BindableProperty.Create(nameof(Message), typeof(string), typeof(ProgressDialog), string.Empty);

  public string Message
  {
    get => (string)GetValue(MessageProperty);
    set => SetValue(MessageProperty, value);
  }

  public ProgressDialog(string title, string message/**, IDialogService dialogService**/)
  {
    InitializeComponent();
    Title = title;
    Message = message;

  }

  /// <summary>
  /// プログレスバーをアニメーションさせながら更新します
  /// </summary>
  /// <param name="targetValue">目標値 (0.0 ～ 1.0)</param>
  public async Task AnimateProgressAsync(double targetValue)
  {
    // 500ms かけてスルスルと動かす
    await MainProgressBar.ProgressTo(targetValue, 500, Easing.CubicOut);
  }
}