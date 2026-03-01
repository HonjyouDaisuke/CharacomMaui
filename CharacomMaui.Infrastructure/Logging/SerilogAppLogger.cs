using System;
using Serilog;
using CharacomMaui.Application.Interfaces;

namespace CharacomMaui.Infrastructure.Logging;

public class SerilogAppLogger : IAppLogger
{
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
  public void UserAction(
      string userId,
      string screen,
      string action,
      string message,
      object? data = null)
  {
    Log.Information("UserAction {@UserAction}", new
    {
      UserId = userId,
      Screen = screen,
      Action = action,
      Message = message,
      Data = data,
      Timestamp = DateTime.UtcNow
    });
  }

  public void UserActionError(
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
      Timestamp = DateTime.UtcNow
    });
  }

  public void SystemInfo(
      string userId,
      string screen,
      string action,
      string message,
      object? data = null)
  {
    Log.Information("SystemInfo {@SystemMessage}", new
    {
      UserId = userId,
      Screen = screen,
      Action = action,
      Message = message,
      Data = data,
      Timestamp = DateTime.UtcNow
    });
  }
  public void SystemWarning(
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
  }

  public void SystemError(
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
      Data = data,
      Timestamp = DateTime.UtcNow
    });
  }
}