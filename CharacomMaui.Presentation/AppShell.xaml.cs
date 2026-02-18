using CharacomMaui.Presentation.Components;
using CharacomMaui.Presentation.Services;
using CharacomMaui.Presentation.Interfaces;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.Models;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CharacomMaui.Presentation.Dialogs;


namespace CharacomMaui.Presentation;

public partial class AppShell : Shell
{
  public AppShell()
  {

    InitializeComponent();
    Routing.RegisterRoute("ProjectDetailPage", typeof(Pages.ProjectDetailPage));
  }


}

