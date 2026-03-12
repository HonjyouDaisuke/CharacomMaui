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
    AdminLogMenu.IsVisible = IsAdmin;
    Routing.RegisterRoute(nameof(ProjectDetailPage), typeof(ProjectDetailPage));
  }
  public bool IsAdmin => string.Equals(_appStatus.UserRole, "admin",
          StringComparison.OrdinalIgnoreCase);
}

