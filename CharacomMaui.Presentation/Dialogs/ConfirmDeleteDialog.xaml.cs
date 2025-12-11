using CommunityToolkit.Maui.Views;
using CharacomMaui.Domain.Entities;
using UraniumUI.Dialogs;
using System.ComponentModel;
using System.Diagnostics;
using CharacomMaui.Presentation.ViewModels;
using System.Threading.Tasks;

namespace CharacomMaui.Presentation.Dialogs;

public partial class ConfirmDeleteDialog : Popup
{
  private readonly Project? _project;
  private IDialogService _dialogService;
  public bool IsConfirmed { get; set; }
  // ========== Title ==========
  public static readonly BindableProperty TitleProperty =
      BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(ConfirmDeleteDialog),
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
        typeof(ConfirmDeleteDialog),
        string.Empty);
  public string Message
  {
    get => (string)GetValue(MessageProperty);
    set => SetValue(MessageProperty, value);
  }

  public ConfirmDeleteDialog(string title, IDialogService dialogService, Project? project = null)
  {
    InitializeComponent();
    _dialogService = dialogService;

    _project = project;
    Title = title;
    Message = $"プロジェクト「{_project?.Name}」を削除してもよろしいですか？";

  }

  private async void OnOkClicked(object sender, EventArgs e)
  {
    IsConfirmed = true;
    await CloseAsync(); // Close に渡す値は任意。複数渡したい場合は Tuple かクラスにまとめる
  }

  private void OnCancelClicked(object sender, EventArgs e)
  {
    IsConfirmed = false;
    CloseAsync();
  }
}