using CharacomMaui.Application.UseCases;
using CharacomMaui.Infrastructure.Services;
using CharacomMaui.Presentation.ViewModels;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Text.Json;
using MauiApp = Microsoft.Maui.Controls.Application;

namespace CharacomMaui.Presentation;

public partial class MainPage : ContentPage
{
  int count = 0;

  public MainPage()
  {
    try
    {
      InitializeComponent();
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[MainPage ctor] {ex}");
      throw;
    }
  }



  private async void OnLoginClicked(object sender, EventArgs e)
  {
    var window = MauiApp.Current?.Windows.FirstOrDefault();
    if (window != null)
    {
      window.Page = new AppShell();
    }
  }


}

