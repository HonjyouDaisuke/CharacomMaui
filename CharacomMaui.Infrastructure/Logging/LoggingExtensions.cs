using Microsoft.Extensions.DependencyInjection;
using Serilog;
using CharacomMaui.Application.Interfaces;

namespace CharacomMaui.Infrastructure.Logging;

public static class LoggingExtensions
{
  public static IServiceCollection AddLoggingInfrastructure(
      this IServiceCollection services)
  {
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .WriteTo.Debug()
        .CreateLogger();

    services.AddSingleton<IAppLogger, SerilogAppLogger>();

    return services;
  }
}