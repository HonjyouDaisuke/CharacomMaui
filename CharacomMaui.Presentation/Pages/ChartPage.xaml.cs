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

}