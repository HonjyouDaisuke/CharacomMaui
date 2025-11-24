using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Infrastructure;
using CharacomMaui.Infrastructure.Services;
using CharacomMaui.Presentation.Models;
using CharacomMaui.Presentation.Pages;
using CharacomMaui.Presentation.Services;
using CharacomMaui.Presentation.ViewModels;
using CommunityToolkit.Maui;
using InputKit.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using UraniumUI;
using UraniumUI.Dialogs;
using UraniumUI.Dialogs.Mopups;

namespace CharacomMaui.Presentation;

public static class MauiProgram
{
  public static MauiApp CreateMauiApp()
  {
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()
        .UseMauiCommunityToolkit()
        .UseUraniumUI()
        .UseUraniumUIMaterial()
        .ConfigureMopups()
        .ConfigureMauiHandlers(handlers =>
        {
          handlers.AddInputKitHandlers();
        })
        .ConfigureFonts(fonts =>
        {
          fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
          fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
          fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
        });
    builder.Services.AddMopupsDialogs();

    builder.Services.AddApplicationServices();

#if DEBUG
    builder.Logging.AddDebug();
#endif

    return builder.Build();
  }
}
