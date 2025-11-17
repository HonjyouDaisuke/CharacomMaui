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

    // ServiceのDI
    builder.Services.AddHttpClient<IBoxConfigRepository, BoxConfigRepository>();
    builder.Services.AddMopupsDialogs();
#if DEBUG
    builder.Logging.AddDebug();
#endif
    // BoxApiService を ICloudStorageService に紐付け
    builder.Services.AddSingleton<IBoxApiAuthService, BoxApiAuthService>();
    builder.Services.AddSingleton<ITokenStorageService, TokenStorageService>();
    builder.Services.AddSingleton<IAppTokenStorageService, AppTokenStorageService>();
    builder.Services.AddTransient<IBoxFolderRepository, ApiBoxFolderRepository>();
    builder.Services.AddSingleton<IAppStatusRepository, AppStatusRepository>();
    builder.Services.AddHttpClient<ITokenValidationService, ApiTokenValidationService>();
    builder.Services.AddTransient<IUserRepository, ApiUserRepository>();
    builder.Services.AddSingleton<IDialogService, MopupsDialogService>();
    builder.Services.AddTransient<IProjectRepository, ProjectRepository>();

    // builder.Services.AddTransient<ICloudStorageService, BoxCloudStorageService>();

    // UseCaseのDI
    builder.Services.AddTransient<ProcessImageFromBoxUseCase>();
    // builder.Services.AddTransient<MainPageViewModel>();
    builder.Services.AddTransient<GetBoxConfigUseCase>();
    builder.Services.AddTransient<CreateProjectUseCase>();
    builder.Services.AddTransient<GetUserProjectsUseCase>();
    builder.Services.AddTransient<GetUserInfoUseCase>();
    builder.Services.AddSingleton<AppStatusUseCase>();
    builder.Services.AddTransient<LoginToBoxUseCase>();
    builder.Services.AddTransient<CreateUserUseCase>();
    builder.Services.AddSingleton<BoxLoginViewModel>();
    builder.Services.AddSingleton<CreateAppUserViewModel>();
    builder.Services.AddSingleton<CreateProjectViewModel>();

    builder.Services.AddHttpClient<IBoxApiRepository, BoxApiRepository>();
    builder.Services.AddTransient<GetBoxFolderItemsUseCase>();
    builder.Services.AddTransient<GetBoxImageItemsUseCase>();
    builder.Services.AddSingleton<BoxFolderViewModel>();
    builder.Services.AddSingleton<BoxItemViewModel>();
    builder.Services.AddSingleton<BoxImageItemViewModel>();
    builder.Services.AddSingleton<AppStatus>();           // Domain 層の状態
    builder.Services.AddSingleton<AppStatusNotifier>();   // Presentation 層の通知ラッパー
    builder.Services.AddSingleton<TitleBarViewModel>();   // ViewModel
    // PageのDI
    builder.Services.AddSingleton<MainPage>();
    builder.Services.AddSingleton<ProjectDetailPage>();
    return builder.Build();
  }
}
