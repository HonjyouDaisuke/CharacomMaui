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

public partial class InviteUserDialog : Popup
{

  private readonly IDialogService _dialogService;

  public ObservableCollection<string> Avatars { get; } = new();
  // ========== Title ==========
  public static readonly BindableProperty TitleProperty =
      BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(InviteUserDialog),
        string.Empty);
  public string Title
  {
    get => (string)GetValue(TitleProperty);
    set => SetValue(TitleProperty, value);
  }

  public ObservableCollection<string> UserNames { get; private set; } = new();
  public ObservableCollection<string> ProjectRoles { get; private set; } = new();

  public bool IsCanceled { get; private set; } = true;
  public string SelectedUserId { get; private set; } = string.Empty;
  public string SelectedProjectRoleId { get; private set; } = string.Empty;
  private List<AppUser> _users;
  private List<ProjectRole> _projectRoles;

  public InviteUserDialog(string title, IDialogService dialogService, List<AppUser> users, List<ProjectRole> projectRoles)
  {
    _users = users;
    _projectRoles = projectRoles;
    UserNamesInit();
    ProjectRolesInit();
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
  private void ProjectRolesInit()
  {
    var list = _projectRoles.Select(r => r.Name).ToList();
    ProjectRoles = new ObservableCollection<string>(list);
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

    foreach (var role in _projectRoles)
    {
      if (SelectProjectRoleName.Text == role.Name)
      {
        SelectedProjectRoleId = role.Id;
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