namespace CharacomMaui.Presentation;

public partial class AppShell : Shell
{
  public AppShell()
  {
    InitializeComponent();
    Routing.RegisterRoute("ProjectDetailPage", typeof(Pages.ProjectDetailPage));
  }
}

