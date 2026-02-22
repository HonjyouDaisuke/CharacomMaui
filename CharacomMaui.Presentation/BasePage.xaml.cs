using CharacomMaui.Presentation.Interfaces;
using CharacomMaui.Presentation.Panels;
using CharacomMaui.Presentation.Services;
using CharacomMaui.Presentation.Dialogs;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.Models;
using System.ComponentModel.Design;
using CommunityToolkit.Maui.Extensions;


namespace CharacomMaui.Presentation;

public partial class BasePage : ContentPage
{
  private INotificationPanelService _panelService;
  private View? _overlay;
  private View? _notificationPanel;
  private INotificationService _notificationService;
  private IAppTokenStorageService _tokenStorage;
  public BasePage(INotificationService notificationService, INotificationPanelService panelService, IAppTokenStorageService tokenStorage)
  {
    InitializeComponent();

    _notificationService = notificationService;
    _panelService = panelService;
    _tokenStorage = tokenStorage;

    _panelService.OpenRequested += OpenPanel;
    _panelService.CloseRequested += ClosePanel;
    _notificationService.NotificationRequested += OnNotificationRequested;
  }

  private async void OnNotificationRequested(
            object? sender,
            NotificationRequest request)
  {
    var dialog = new NotificationDialog(
            request.Id,
            request.Title,
            request.Message,
            request.Icon);

    await this.ShowPopupAsync(dialog);

    var id = dialog.SelectedId;
    if (id == null || id == string.Empty) return;

    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    if (accessToken == null) return;
    await _notificationService.MarkAsReadAsync(accessToken, id);

  }
  protected override void OnApplyTemplate()
  {
    base.OnApplyTemplate();

    _overlay = GetTemplateChild("Overlay") as View;
    _notificationPanel = GetTemplateChild("NotificationPanel") as View;
  }

  private async void OpenPanel()
  {
    if (Shell.Current?.CurrentPage != this) return;
    if (_notificationPanel == null || _overlay == null) return;

    var tokens = await _tokenStorage.GetTokensAsync();
    var accessToken = tokens?.AccessToken;
    if (accessToken == null) return;
    await _notificationService.InitNotificationsAsync(accessToken);

    _notificationPanel.IsVisible = true;
    _overlay.IsVisible = true;

    _notificationPanel.TranslationX = 300;

    await Task.WhenAll(
        _notificationPanel.TranslateTo(0, 0, 250, Easing.CubicOut),
        _overlay.FadeTo(1, 250)
    );
  }

  private async void ClosePanel()
  {
    System.Diagnostics.Debug.WriteLine("ClosePanel 閉じる処理");
    if (_notificationPanel == null || _overlay == null) return;
    if (!_notificationPanel.IsVisible) return;

    await Task.WhenAll(
        _notificationPanel.TranslateTo(300, 0, 250, Easing.CubicIn),
        _overlay.FadeTo(0, 250)
    );

    _notificationPanel.IsVisible = false;
    _overlay.IsVisible = false;
  }
}