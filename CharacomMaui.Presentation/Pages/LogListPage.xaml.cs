using CharacomMaui.Application.UseCases;
using CharacomMaui.Application.Logging;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Presentation.Interfaces;
using CharacomMaui.Presentation.ViewModels;

namespace CharacomMaui.Presentation.Pages;

public partial class LogListPage : BasePage
{
  private readonly ILogQueryService _appLog;
  private readonly LogListViewModel _viewModel;
  public LogListPage(
    ILogQueryService appLog,
    IAppTokenStorageService tokenStorage,
    LogListViewModel viewModel,
    INotificationService notificationService,
    INotificationPanelService panelService) : base(notificationService, panelService, tokenStorage)
  {
    InitializeComponent();
    _appLog = appLog;
    _viewModel = viewModel;
    BindingContext = _viewModel;
  }
  protected override async void OnAppearing()
  {
    base.OnAppearing();
    System.Diagnostics.Debug.WriteLine("LogListPage Appearing");
    await _viewModel.LoadAsync();
  }
}