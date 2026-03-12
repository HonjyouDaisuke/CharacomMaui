using AuthenticationServices;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Presentation.Pages;
namespace CharacomMaui.Presentation;

public partial class AppShell : Shell
{
  private readonly AppStatus _appStatus;
  public AppShell(AppStatus appStatus)
  {
    InitializeComponent();

    _appStatus = appStatus;
    UpdateMenuVisibility();
    Routing.RegisterRoute(nameof(ProjectDetailPage), typeof(ProjectDetailPage));
  }

  private void UpdateMenuVisibility()
  {
    var roleId = _appStatus.UserRole;

    if (roleId == "admin")
    {
      AdminLogMenu.IsVisible = true;
    }
    else
    {
      AdminLogMenu.IsVisible = false;
    }
  }
}

