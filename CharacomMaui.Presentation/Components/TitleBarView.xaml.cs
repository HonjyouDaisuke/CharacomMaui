namespace CharacomMaui.Presentation.Components;

using CharacomMaui.Presentation.ViewModels;
using CharacomMaui.Application.UseCases;

public partial class TitleBarView : ContentView
{
  public TitleBarView()
  {
    InitializeComponent();

    this.Loaded += (s, e) =>
        {
          var vm = Handler?.MauiContext?.Services.GetService<TitleBarViewModel>();
          if (vm == null)
          {
            System.Diagnostics.Debug.WriteLine("DI に TitleBarViewModel が登録されていません！");
            return;
          }
          BindingContext = vm;
        };
  }
}
