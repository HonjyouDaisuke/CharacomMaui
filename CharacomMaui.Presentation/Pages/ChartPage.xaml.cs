using CharacomMaui.Application.UseCases;

namespace CharacomMaui.Presentation.Pages;

public partial class ChartPage : ContentPage
{
  private readonly AppStatusUseCase _statusUseCase;
  public ChartPage(AppStatusUseCase statusUseCase)
  {
    InitializeComponent();
    _statusUseCase = statusUseCase;
    var userState = _statusUseCase.GetAppStatus();
    BindingContext = userState;
    System.Diagnostics.Debug.WriteLine($"名前 {_statusUseCase.GetAppStatus().ToString()}");
  }
}