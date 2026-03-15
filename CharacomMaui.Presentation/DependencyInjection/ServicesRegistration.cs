using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Application.Coordinators;
using CharacomMaui.Application.Sessions;
using CharacomMaui.Application.Logging;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Infrastructure.Services;
using CharacomMaui.Infrastructure.Logging;
using CharacomMaui.Presentation.Models;
using CharacomMaui.Presentation.Pages;
using CharacomMaui.Presentation.Services;
using CharacomMaui.Presentation.ViewModels;
using UraniumUI.Dialogs;
using UraniumUI.Dialogs.Mopups;
using Microsoft.Extensions.DependencyInjection;
using CharacomMaui.Presentation.Interfaces;

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
    services.AddTransient<ICharaImageOverlayUseCase, CharaImageOverlayUseCase>();
    services.AddTransient<FetchUserRolesUseCase>();
    services.AddTransient<UpdateUserRoleUseCase>();
    services.AddTransient<ProxyLoginUseCase>();
    services.AddTransient<ProxyLogoutUseCase>();
    services.AddTransient<FetchProjectRolesUseCase>();
    services.AddTransient<InviteToProjectUseCase>();
    services.AddTransient<FetchNotificationsUseCase>();
    services.AddTransient<UpdateNotificationReadUseCase>();

    // Coordinator
    services.AddTransient<ICharaLoadCoordinator, CharaLoadCoordinator>();
    services.AddTransient<IProjectItemsLoadCoordinator, ProjectItemsLoadCoordinator>();

    // Repository
    services.AddSingleton<IAppStatusRepository, AppStatusRepository>();

    // API Client
    services.AddHttpClient<IBoxConfigRepository, BoxConfigRepository>();
    services.AddHttpClient<IBoxApiRepository, BoxApiRepository>();

    // Sessions
    services.AddSingleton<UserRolesSession>();

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
    services.AddSingleton<UserListViewModel>();
    services.AddSingleton<LogListViewModel>();

    // Domain Service
    services.AddSingleton<AppStatus>();
    services.AddSingleton<AppStatusNotifier>();

    // Infrastructure Service
    services.AddSingleton<IBoxApiAuthService, BoxApiAuthService>();
    services.AddSingleton<IAppTokenStorageService, AppTokenStorageService>();

    // PageのDI
    services.AddSingleton<MainPage>();
    services.AddTransient<ProjectDetailPage>();
    services.AddSingleton<UserListPage>();
    services.AddSingleton<LogListPage>();

    // その他
    services.AddSingleton<IDialogService, MopupsDialogService>();
    services.AddSingleton<IProgressDialogService, ProgressDialogService>();
    services.AddSingleton<ISimpleProgressDialogService, SimpleProgressDialogService>();
    services.AddSingleton<INotificationService, NotificationService>();
    services.AddSingleton<INotificationPanelService, NotificationPanelService>();

    return services;
  }
}
