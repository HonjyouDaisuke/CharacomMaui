using System;
using Serilog;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.Logging;
using System.Threading.Tasks;

namespace CharacomMaui.Infrastructure.Logging;

public class SerilogAppLogger : IAppLogger
{
  private readonly ILogApiClient _logApiClient;

  public SerilogAppLogger(ILogApiClient logApiClient)
  {
    _logApiClient = logApiClient;
  }
  public void Info(string message, object? data = null)
  {
    if (data is null)
      Log.Information(message);
    else
      Log.Information("{Message} {@Data}", message, data);
  }
  public void Debug(string message, object? data = null)
  {
    if (data is null)
      Log.Debug(message);
    else
      Log.Debug("{Message} {@Data}", message, data);
  }

  public void Warning(string message, object? data = null)
  {
    if (data is null)
      Log.Warning(message);
    else
      Log.Warning("{Message} {@Data}", message, data);
  }
  public void Error(Exception ex, string message, object? data = null)
  {
    if (data is null)
      Log.Error(ex, message);
    else
      Log.Error(ex, "{Message} {@Data}", message, data);
  }

  // 🔥 ユーザー操作ログ
  public async Task UserAction(
      string userId,
      string screen,
      string action,
      string message,
      object? data = null)
  {
    Console.WriteLine($"[UserAction] Screen:{screen}, Action:{action}, Msg:{message}");
    Log.Information("UserAction {@UserAction}", new
    {
      UserId = userId,
      Screen = screen,
      Action = action,
      Message = message,
      Data = data,
      Timestamp = DateTime.UtcNow
    });
    if (userId == "" || userId == null) return;
    var request = new LogRequest
    {
      Level = "User Info",
      Screen = screen,
      Action = action,
      Message = message,
      Data = data,
      CorrelationId = Guid.NewGuid().ToString()
    };
    try
    {
      await _logApiClient.SendAsync(request);
    }
    catch
    {
      System.Diagnostics.Debug.WriteLine("Failed to send log to API");
    }

  }
  public async Task UserActionError(
      Exception ex,
      string userId,
      string screen,
      string action,
      object? data = null)
  {
    Log.Error(ex, "UserActionError {@UserAction}", new
    {
      UserId = userId,
      Screen = screen,
      Action = action,
      Data = data,
      Message = ex.Message,
      Timestamp = DateTime.UtcNow
    });
    if (userId == "" || userId == null) return;
    var request = new LogRequest
    {
      Level = "User Error",
      Screen = screen,
      Action = action,
      Data = data,
      Message = ex.Message,
      CorrelationId = Guid.NewGuid().ToString()
    };

    try
    {
      await _logApiClient.SendAsync(request);
    }
    catch
    {
      System.Diagnostics.Debug.WriteLine("Failed to send log to API");
    }
  }

  public async Task SystemInfo(
      string userId,
      string screen,
      string action,
      string message,
      object? data = null)
  {
    Console.WriteLine($"[SystemInfo] Screen:{screen}, Action:{action}, Msg:{message}");
    Log.Information("SystemInfo {@SystemMessage}", new
    {
      UserId = userId,
      Screen = screen,
      Action = action,
      Message = message,
      Data = data,
      Timestamp = DateTime.UtcNow
    });
    if (userId == "" || userId == null) return;
    var request = new LogRequest
    {
      Level = "System Info",
      Screen = screen,
      Action = action,
      Message = message,
      Data = data,
      CorrelationId = Guid.NewGuid().ToString()
    };

    try
    {
      await _logApiClient.SendAsync(request);
    }
    catch
    {
      System.Diagnostics.Debug.WriteLine("Failed to send log to API");
    }
  }
  public async Task SystemWarning(
      string userId,
      string screen,
      string action,
      string message,
      object? data = null)
  {
    Log.Warning("SystemWarning {@SystemMessage}", new
    {
      UserId = userId,
      Screen = screen,
      Action = action,
      Message = message,
      Data = data,
      Timestamp = DateTime.UtcNow
    });
    if (userId == "" || userId == null) return;
    var request = new LogRequest
    {
      Level = "System Warning",
      Screen = screen,
      Action = action,
      Message = message,
      Data = data,
      CorrelationId = Guid.NewGuid().ToString()
    };

    await _logApiClient.SendAsync(request);
  }

  public async Task SystemError(
    Exception ex,
    string userId,
    string screen,
    string action,
    object? data = null)
  {
    Log.Error(ex, "SystemError {@SystemMessage}", new
    {
      UserId = userId,
      Screen = screen,
      Action = action,
      Message = ex.Message,
      Data = data,
      Timestamp = DateTime.UtcNow
    });
    if (userId == "" || userId == null) return;
    var request = new LogRequest
    {
      Level = "System Error",
      Screen = screen,
      Action = action,
      Message = ex.Message,
      Data = data,
      CorrelationId = Guid.NewGuid().ToString()
    };

    try
    {
      await _logApiClient.SendAsync(request);
    }
    catch
    {
      System.Diagnostics.Debug.WriteLine("Failed to send log to API");
    }
  }
}