using CommunityToolkit.Maui.Views;
using CharacomMaui.Domain.Entities;
using UraniumUI.Dialogs;
using System.ComponentModel;
using System.Diagnostics;
using CharacomMaui.Presentation.ViewModels;
using CharacomMaui.Presentation.Services;
using System.Threading.Tasks;
using Mopups.Pages;
using Mopups.Services;
using CharacomMaui.Presentation.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CharacomMaui.Application.UseCases;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Application.Sessions;
using CharacomMaui.Application.Logging;
using Org.BouncyCastle.Asn1.Cms;

namespace CharacomMaui.Presentation.Dialogs;

public partial class LogInfoDialog : Popup
{

  private readonly IDialogService _dialogService;

  // ========== Title ==========
  public static readonly BindableProperty TitleProperty =
      BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(LogInfoDialog),
        string.Empty);
  public string Title
  {
    get => (string)GetValue(TitleProperty);
    set => SetValue(TitleProperty, value);
  }
  // ========== Time ==========
  public static readonly BindableProperty CreatedAtProperty =
      BindableProperty.Create(
        nameof(CreatedAt),
        typeof(string),
        typeof(LogInfoDialog),
        string.Empty);
  public string CreatedAt
  {
    get => (string)GetValue(CreatedAtProperty);
    set => SetValue(CreatedAtProperty, value);
  }
  // ========== Level ==========
  public static readonly BindableProperty LevelProperty =
      BindableProperty.Create(
        nameof(Level),
        typeof(string),
        typeof(LogInfoDialog),
        string.Empty);
  public string Level
  {
    get => (string)GetValue(LevelProperty);
    set => SetValue(LevelProperty, value);
  }
  // ========== UserId ==========
  public static readonly BindableProperty UserIdProperty =
      BindableProperty.Create(
        nameof(UserId),
        typeof(string),
        typeof(LogInfoDialog),
        string.Empty);
  public string UserId
  {
    get => (string)GetValue(UserIdProperty);
    set => SetValue(UserIdProperty, value);
  }
  // ========== Screen ==========
  public static readonly BindableProperty ScreenProperty =
      BindableProperty.Create(
        nameof(Screen),
        typeof(string),
        typeof(LogInfoDialog),
        string.Empty);
  public string Screen
  {
    get => (string)GetValue(ScreenProperty);
    set => SetValue(ScreenProperty, value);
  }
  // ========== Action ==========
  public static readonly BindableProperty ActionProperty =
      BindableProperty.Create(
        nameof(Action),
        typeof(string),
        typeof(LogInfoDialog),
        string.Empty);
  public string Action
  {
    get => (string)GetValue(ActionProperty);
    set => SetValue(ActionProperty, value);
  }
  // ========== Message ==========
  public static readonly BindableProperty MessageProperty =
      BindableProperty.Create(
        nameof(Message),
        typeof(string),
        typeof(LogInfoDialog),
        string.Empty);
  public string Message
  {
    get => (string)GetValue(MessageProperty);
    set => SetValue(MessageProperty, value);
  }
  // ========== Data ==========
  public static readonly BindableProperty DataProperty =
      BindableProperty.Create(
        nameof(Data),
        typeof(string),
        typeof(LogInfoDialog),
        string.Empty);
  public string Data
  {
    get => (string)GetValue(DataProperty);
    set => SetValue(DataProperty, value);
  }

  public LogInfoDialog(string title, IDialogService dialogService, LogDto log)
  {
    InitializeComponent();

    BindingContext = this;
    Title = title;
    _dialogService = dialogService;
    CreatedAt = log.CreatedAt;
    Level = log.Level;
    UserId = log.UserId;
    Screen = log.Screen;
    Action = log.Action;
    Message = log.Message;
    Data = log.Data;
  }

  private async void OnOkClicked(object sender, EventArgs e)
  {
    await CloseAsync();
    return;
  }
}