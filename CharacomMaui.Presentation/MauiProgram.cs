using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

namespace CharacomMaui.Presentation;

public static class MauiProgram
{
  public static MauiApp CreateMauiApp()
  {
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()
        .ConfigureFonts(fonts =>
        {
          fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
          fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });

#if DEBUG
    builder.Logging.AddDebug();
#endif
    // BoxApiService を ICloudStorageService に紐付け
    builder.Services.AddSingleton<ICloudStorageService>(sp =>
        new BoxApiService("YOUR_ACCESS_TOKEN_HERE"));
    // builder.Services.AddTransient<ICloudStorageService, BoxCloudStorageService>();
    builder.Services.AddTransient<ProcessImageFromBoxUseCase>();
    // builder.Services.AddTransient<MainPageViewModel>();

    return builder.Build();
  }
}

