#if IOS || MACCATALYST
using AuthenticationServices;
#endif
using CharacomMaui.Domain.Entities;
using CharacomMaui.Presentation.Pages;
namespace CharacomMaui.Presentation;

public partial class AppShell : Shell
{
  private readonly AppStatus _appStatus;
  public AppShell(AppStatus appStatus)
  {
    InitializeComponent();
    System.Console.WriteLine("これは標準出力です");
    System.Diagnostics.Debug.WriteLine("これはデバッグ出力です。");
    System.Diagnostics.Trace.WriteLine("これはトレース出力です");
    _appStatus = appStatus;
    AdminLogMenu.IsVisible = IsAdmin;
    Routing.RegisterRoute(nameof(ProjectDetailPage), typeof(ProjectDetailPage));
  }
  public bool IsAdmin => string.Equals(_appStatus.UserRole, "admin",
          StringComparison.OrdinalIgnoreCase);
}

