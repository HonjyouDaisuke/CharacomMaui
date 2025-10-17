using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Infrastructure.Services;
using CharacomMaui.Presentation.Services;
using CharacomMaui.Presentation.ViewModels;
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

    // ServiceのDI
    builder.Services.AddHttpClient<IBoxConfigRepository, BoxConfigRepository>();
#if DEBUG
    builder.Logging.AddDebug();
#endif
    // BoxApiService を ICloudStorageService に紐付け
    builder.Services.AddSingleton<IBoxApiAuthService, BoxApiAuthService>();
    builder.Services.AddSingleton<ITokenStorageService, TokenStorageService>();
    // builder.Services.AddTransient<ICloudStorageService, BoxCloudStorageService>();

    // UseCaseのDI
    builder.Services.AddTransient<ProcessImageFromBoxUseCase>();
    // builder.Services.AddTransient<MainPageViewModel>();
    builder.Services.AddTransient<GetBoxConfigUseCase>();
    builder.Services.AddTransient<LoginToBoxUseCase>();
    builder.Services.AddSingleton<LoginViewModel>();

    // PageのDI
    builder.Services.AddTransient<MainPage>();
    return builder.Build();
  }
}

