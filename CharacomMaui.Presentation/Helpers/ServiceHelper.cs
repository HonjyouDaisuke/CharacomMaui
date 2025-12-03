namespace CharacomMaui.Presentation.Helpers;

public static class ServiceHelper
{
  public static IServiceProvider Services =>
      Current?.Handler?.MauiContext?.Services
      ?? throw new InvalidOperationException("Unable to access MAUI service provider.");

  public static T GetService<T>() where T : class
      => Services.GetService(typeof(T)) as T
          ?? throw new InvalidOperationException($"Service not found: {typeof(T)}");

  private static Microsoft.Maui.Controls.Application? Current => Microsoft.Maui.Controls.Application.Current;
}

