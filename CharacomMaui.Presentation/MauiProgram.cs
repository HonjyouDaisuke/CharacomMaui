using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Infrastructure;
using CharacomMaui.Infrastructure.Services;
using CharacomMaui.Presentation.Services;
using CharacomMaui.Presentation.ViewModels;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UraniumUI;
using UraniumUI.Dialogs;

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

        .ConfigureFonts(fonts =>
        {
          fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
          fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });

    // ServiceのDI
    builder.Services.AddHttpClient<IBoxConfigRepository, BoxConfigRepository>();
    builder.Services.AddCommunityToolkitDialogs();
#if DEBUG
    builder.Logging.AddDebug();
#endif
    // BoxApiService を ICloudStorageService に紐付け
    builder.Services.AddSingleton<IBoxApiAuthService, BoxApiAuthService>();
    builder.Services.AddSingleton<ITokenStorageService, TokenStorageService>();
    builder.Services.AddSingleton<IAppTokenStorageService, AppTokenStorageService>();
    builder.Services.AddTransient<IBoxTopFolderRepository, ApiBoxTopFolderRepository>();
    builder.Services.AddTransient<IUserRepository, ApiUserRepository>();
    // builder.Services.AddTransient<ICloudStorageService, BoxCloudStorageService>();

    // UseCaseのDI
    builder.Services.AddTransient<ProcessImageFromBoxUseCase>();
    // builder.Services.AddTransient<MainPageViewModel>();
    builder.Services.AddTransient<GetBoxConfigUseCase>();
    builder.Services.AddTransient<LoginToBoxUseCase>();
    builder.Services.AddTransient<CreateUserUseCase>();
    builder.Services.AddSingleton<BoxLoginViewModel>();
    builder.Services.AddSingleton<CreateAppUserViewModel>();

    builder.Services.AddHttpClient<IBoxApiRepository, BoxApiRepository>();
    builder.Services.AddTransient<GetBoxFolderItemsUseCase>();
    builder.Services.AddTransient<GetBoxImageItemsUseCase>();
    builder.Services.AddSingleton<BoxFolderViewModel>();
    builder.Services.AddSingleton<BoxItemViewModel>();
    builder.Services.AddSingleton<BoxImageItemViewModel>();
    // PageのDI
    builder.Services.AddSingleton<MainPage>();
    return builder.Build();
  }
}
