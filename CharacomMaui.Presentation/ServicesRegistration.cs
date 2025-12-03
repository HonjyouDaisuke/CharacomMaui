using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Infrastructure.Services;
using CharacomMaui.Presentation.Models;
using CharacomMaui.Presentation.Pages;
using CharacomMaui.Presentation.Services;
using CharacomMaui.Presentation.ViewModels;
using UraniumUI.Dialogs;
using UraniumUI.Dialogs.Mopups;

namespace CharacomMaui.Presentation;

public static class ServicesRegistration
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    // UseCaseのDI
    services.AddTransient<ProcessImageFromBoxUseCase>();
    services.AddTransient<GetBoxConfigUseCase>();
    services.AddTransient<CreateProjectUseCase>();
    services.AddTransient<GetUserProjectsUseCase>();
    services.AddTransient<GetUserInfoUseCase>();
    services.AddTransient<GetProjectCharaItemsUseCase>();
    services.AddSingleton<AppStatusUseCase>();
    services.AddTransient<LoginToBoxUseCase>();
    services.AddTransient<CreateUserUseCase>();
    services.AddTransient<GetBoxFolderItemsUseCase>();
    services.AddTransient<GetBoxImageItemsUseCase>();

    // Repository
    services.AddTransient<IBoxFolderRepository, ApiBoxFolderRepository>();
    services.AddSingleton<IAppStatusRepository, AppStatusRepository>();
    services.AddTransient<IUserRepository, ApiUserRepository>();
    services.AddTransient<IProjectRepository, ProjectRepository>();
    services.AddTransient<ICharaDataRepository, CharaDataRepository>();

    // API Client
    services.AddHttpClient<IBoxConfigRepository, BoxConfigRepository>();
    services.AddHttpClient<ITokenValidationService, ApiTokenValidationService>();
    services.AddHttpClient<IBoxApiRepository, BoxApiRepository>();

    // ViewModel
    services.AddSingleton<BoxLoginViewModel>();
    services.AddSingleton<CreateAppUserViewModel>();
    services.AddSingleton<CreateProjectViewModel>();
    services.AddSingleton<BoxFolderViewModel>();
    services.AddTransient<ProjectDetailViewModel>();
    services.AddSingleton<BoxItemViewModel>();
    services.AddSingleton<BoxImageItemViewModel>();
    services.AddSingleton<TitleBarViewModel>();   // ViewModel

    // Domain Service
    services.AddSingleton<AppStatus>();           // Domain 層の状態
    services.AddSingleton<AppStatusNotifier>();   // Presentation 層の通知ラッパー

    // Infrastructure Service
    services.AddSingleton<IBoxApiAuthService, BoxApiAuthService>();
    services.AddSingleton<ITokenStorageService, TokenStorageService>();
    services.AddSingleton<IAppTokenStorageService, AppTokenStorageService>();

    // PageのDI
    services.AddSingleton<MainPage>();
    services.AddSingleton<ProjectDetailPage>();

    // その他
    services.AddSingleton<IDialogService, MopupsDialogService>();

    return services;
  }
}
