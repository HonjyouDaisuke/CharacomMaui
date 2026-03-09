using CharacomMaui.Application.UseCases;
using CharacomMaui.Application.Logging;
using CharacomMaui.Application.Interfaces;

namespace CharacomMaui.Presentation.Pages;

public partial class ChartPage : ContentPage
{
  private readonly AppStatusUseCase _statusUseCase;
  private readonly ILogQueryService _appLog;
  public ChartPage(AppStatusUseCase statusUseCase, ILogQueryService appLog)
  {
    InitializeComponent();
    _statusUseCase = statusUseCase;
    _appLog = appLog;
    var userState = _statusUseCase.GetAppStatus();
    BindingContext = userState;
    System.Diagnostics.Debug.WriteLine($"名前 {_statusUseCase.GetAppStatus().ToString()}");
  }
  protected override async void OnAppearing()
  {
    base.OnAppearing();
    var logs = await _appLog.GetLogsAsync(DateTime.Today);

    foreach (LogDto log in logs)
    {
      System.Diagnostics.Debug.WriteLine($" Id = {log.Id}");
      System.Diagnostics.Debug.WriteLine($" UserId = {log.UserId}");
      System.Diagnostics.Debug.WriteLine($" Level = {log.Level}");
      System.Diagnostics.Debug.WriteLine($" Screen = {log.Screen}");
      System.Diagnostics.Debug.WriteLine($" Action = {log.Action}");
      System.Diagnostics.Debug.WriteLine($" Message = {log.Message}");
      System.Diagnostics.Debug.WriteLine($" CreatedAt = {log.CreatedAt}");
    }
  }
}