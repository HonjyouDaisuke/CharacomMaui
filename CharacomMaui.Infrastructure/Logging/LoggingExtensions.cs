using Microsoft.Extensions.DependencyInjection;
using Serilog;
using CharacomMaui.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace CharacomMaui.Infrastructure.Logging;

public static class LoggingExtensions
{
  public static IServiceCollection AddLoggingInfrastructure(
      this IServiceCollection services)
  {
    // 1. Serilog 自体の設定
    Log.Logger = new LoggerConfiguration()
#if DEBUG
        .MinimumLevel.Debug()
#else
        .MinimumLevel.Information()
#endif
        .WriteTo.Console(
          // 💡 Theme を None にすることで、色変えエラーを回避します
          theme: Serilog.Sinks.SystemConsole.Themes.ConsoleTheme.None,
          outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        )
        //.WriteTo.Debug()
        .CreateLogger();
    //Serilog.Debugging.SelfLog.Enable(msg => System.Console.WriteLine($"SERILOG-ERROR: {msg}"));
    if (Environment.GetEnvironmentVariable("SERILOG_SELFLOG") == "1")
    {
      Serilog.Debugging.SelfLog.Enable(
          TextWriter.Synchronized(Console.Error));
    }
    // 2. ILogger 経由のログを Serilog に流す設定を DI に追加
    services.AddLogging(logging =>
    {
      logging.ClearProviders(); // 既存の(動いていない)DebugProviderをクリア
      logging.AddSerilog(Log.Logger);
    });

    services.AddSingleton<IAppLogger, SerilogAppLogger>();

    return services;
  }
}