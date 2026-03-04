using CharacomMaui.Application.Interfaces;
using CharacomMaui.Infrastructure.Services;
using CharacomMaui.Presentation.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using CharacomMaui.Application.Logging;
using CharacomMaui.Infrastructure.Logging;

namespace CharacomMaui.Presentation.DependencyInjection;

public static class ApiClientRegistration
{
  public static IServiceCollection AddApiClients(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddHttpClient<IUserRepository, ApiUserRepository>(ConfigureClient);
    services.AddHttpClient<ICharaDataRepository, CharaDataRepository>(ConfigureClient);
    services.AddHttpClient<IProjectRepository, ProjectRepository>(ConfigureClient);
    services.AddHttpClient<IFetchBoxItemContentService, FetchBoxItemContentService>(ConfigureClient);
    services.AddHttpClient<IStrokeMasterRepository, StrokeMasterRepository>(ConfigureClient);
    services.AddHttpClient<IStandardMasterRepository, StandardMasterRepository>(ConfigureClient);
    services.AddHttpClient<ITokenValidationService, ApiTokenValidationService>(ConfigureClient);
    services.AddHttpClient<IBoxFolderRepository, ApiBoxFolderRepository>(ConfigureClient);
    services.AddHttpClient<IAvatarRepository, AvatarRepository>(ConfigureClient);
    services.AddHttpClient<IUserRolesRepository, UserRolesRepository>(ConfigureClient);
    services.AddHttpClient<IProxyLoginService, ProxyLoginService>(ConfigureClient);
    services.AddHttpClient<IProxyLogoutService, ProxyLogoutService>(ConfigureClient);
    services.AddHttpClient<IProjectRolesRepository, ProjectRolesRepository>(ConfigureClient);
    services.AddHttpClient<IUserProjectsRepository, UserProjectsRepository>(ConfigureClient);
    services.AddHttpClient<INotificationsRepository, NotificationsRepository>(ConfigureClient);
    services.AddHttpClient<ILogApiClient, LogApiClient>(ConfigureClient);
    services.AddHttpClient<ILogQueryService, LogQueryService>(ConfigureClient);
    return services;
  }

  private static void ConfigureClient(IServiceProvider sp, HttpClient client)
  {
    var settings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;

    if (string.IsNullOrWhiteSpace(settings.BaseUrl))
      throw new InvalidOperationException("ApiSettings.BaseUrl is null or empty");

    client.BaseAddress = new Uri(settings.BaseUrl.EndsWith('/')
        ? settings.BaseUrl
        : settings.BaseUrl + "/");
  }
}