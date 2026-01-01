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
using Microsoft.Extensions.DependencyInjection;

namespace CharacomMaui.Presentation.DependencyInjection;

public static class ServicesRegistration
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    // UseCaseのDI
    services.AddTransient<ProcessImageFromBoxUseCase>();
    services.AddTransient<GetBoxConfigUseCase>();
    services.AddTransient<CreateOrUpdateProjectUseCase>();
    services.AddTransient<GetUserProjectsUseCase>();
    services.AddTransient<IGetUserInfoUseCase, GetUserInfoUseCase>();
    services.AddTransient<GetProjectCharaItemsUseCase>();
    services.AddSingleton<AppStatusUseCase>();
    services.AddTransient<LoginToBoxUseCase>();
    services.AddTransient<CreateUserUseCase>();
    services.AddTransient<GetBoxFolderItemsUseCase>();
    services.AddTransient<GetBoxImageItemsUseCase>();
    services.AddTransient<UpdateStrokeMasterUseCase>();
    services.AddTransient<UpdateStandardMasterUseCase>();
    services.AddTransient<FetchBoxItemUseCase>();
    services.AddTransient<GetStandardFileIdUseCase>();
    services.AddTransient<GetStrokeFileIdUseCase>();
    services.AddTransient<UpdateCharaSelectedUseCase>();
    services.AddTransient<DeleteProjectUseCase>();
    services.AddTransient<GetProjectDetailsUseCase>();
    services.AddTransient<GetAvatarsUrlUseCase>();
    services.AddTransient<UpdateUserInfoUseCase>();

    // Repository
    services.AddSingleton<IAppStatusRepository, AppStatusRepository>();

    // API Client
    services.AddHttpClient<IBoxConfigRepository, BoxConfigRepository>();
    services.AddHttpClient<IBoxApiRepository, BoxApiRepository>();

    // ViewModel
    services.AddSingleton<BoxLoginViewModel>();
    services.AddSingleton<CreateAppUserViewModel>();
    services.AddSingleton<CreateProjectViewModel>();
    services.AddSingleton<BoxFolderViewModel>();
    services.AddTransient<ProjectDetailViewModel>();
    services.AddSingleton<BoxItemViewModel>();
    services.AddSingleton<BoxImageItemViewModel>();
    services.AddSingleton<TitleBarViewModel>();
    services.AddSingleton<CharaSelectViewModel>();

    // Domain Service
    services.AddSingleton<AppStatus>();
    services.AddSingleton<AppStatusNotifier>();

    // Infrastructure Service
    services.AddSingleton<IBoxApiAuthService, BoxApiAuthService>();
    services.AddSingleton<IAppTokenStorageService, AppTokenStorageService>();

    // PageのDI
    services.AddSingleton<MainPage>();
    services.AddSingleton<ProjectDetailPage>();

    // その他
    services.AddSingleton<IDialogService, MopupsDialogService>();
    services.AddSingleton<IProgressDialogService, ProgressDialogService>();

    return services;
  }
}
