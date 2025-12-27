using CharacomMaui.Application.Interfaces;
using CharacomMaui.Infrastructure.Services;
using CharacomMaui.Presentation.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

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