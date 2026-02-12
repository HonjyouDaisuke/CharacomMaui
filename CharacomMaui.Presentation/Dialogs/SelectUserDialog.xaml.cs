using CommunityToolkit.Maui.Views;
using CharacomMaui.Domain.Entities;
using UraniumUI.Dialogs;
using System.ComponentModel;
using System.Diagnostics;
using CharacomMaui.Presentation.ViewModels;
using CharacomMaui.Presentation.Services;
using CharacomMaui.Presentation.Components;
using System.Threading.Tasks;
using Mopups.Pages;
using Mopups.Services;
using CharacomMaui.Presentation.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.Sessions;
using System.Dynamic;


namespace CharacomMaui.Presentation.Dialogs;

public partial class SelectUserDialog : Popup
{

  private readonly IDialogService _dialogService;

  public ObservableCollection<string> Avatars { get; } = new();
  // ========== Title ==========
  public static readonly BindableProperty TitleProperty =
      BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(SelectUserDialog),
        string.Empty);
  public string Title
  {
    get => (string)GetValue(TitleProperty);
    set => SetValue(TitleProperty, value);
  }

  public ObservableCollection<string> UserNames { get; private set; } = new() { "管理者", "デビルマン", "スーパーマン" };

  public bool IsCanceled { get; private set; } = true;
  public string SelectedUserId { get; private set; } = string.Empty;
  private List<AppUser> _users;
  public SelectUserDialog(string title, IDialogService dialogService, List<AppUser> users)
  {
    _users = users;
    UserNamesInit();
    InitializeComponent();
    BindingContext = this;

    Title = title;
    _dialogService = dialogService;
  }
  private void UserNamesInit()
  {
    var list = _users.Select(r => r.Name).ToList();
    UserNames = new ObservableCollection<string>(list);
  }


  private async void OnOkClicked(object sender, EventArgs e)
  {
    IsCanceled = false;
    foreach (var user in _users)
    {
      if (SelectUserName.Text == user.Name)
      {
        SelectedUserId = user.Id;
      }
    }
    await CloseAsync();

  }

  private async void OnCancelClicked(object sender, EventArgs e)
  {
    IsCanceled = true;
    System.Diagnostics.Debug.WriteLine("OnCancelClicked");
    await CloseAsync();

  }

}