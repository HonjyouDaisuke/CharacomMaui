using System.Runtime.InteropServices;
using CommunityToolkit.Maui;
using InputKit.Handlers;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using SkiaSharp.Views.Maui.Controls.Hosting;
using UraniumUI;
using Microsoft.Extensions.Configuration;
using CharacomMaui.Presentation.Config;
using Microsoft.Maui.Storage;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Infrastructure.Services;
using Microsoft.Extensions.Options;
using CharacomMaui.Presentation.DependencyInjection;

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
        .UseSkiaSharp()
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


    using var stream = typeof(MauiProgram).Assembly
      .GetManifestResourceStream("CharacomMaui.Presentation.Resources.Raw.appsettings.json");
    if (stream == null)
      throw new FileNotFoundException("appsettings.json not found");
    builder.Configuration.AddJsonStream(stream);

    builder.Services.Configure<ApiSettings>(
      builder.Configuration.GetSection("ApiSettings"));

    builder.Services
      .AddApiClients(builder.Configuration)
      .AddApplicationServices();

    builder.Services.AddMopupsDialogs();

#if DEBUG
    builder.Logging.AddDebug();
#endif

    return builder.Build();
  }
}
