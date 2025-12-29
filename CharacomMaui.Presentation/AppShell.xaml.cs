using CharacomMaui.Presentation.Components;
using CharacomMaui.Presentation.Services;

namespace CharacomMaui.Presentation;

public partial class AppShell : Shell
{
  public AppShell()
  {
    InitializeComponent();
  }

  protected override void OnNavigated(ShellNavigatedEventArgs args)
  {
    base.OnNavigated(args);

    // CurrentPage を ContentPage にキャストして処理する
    if (CurrentPage is ContentPage currentPage)
    {
      System.Diagnostics.Debug.WriteLine($"★★★ {currentPage.GetType().Name} に SnackBar を注入します");

      // すでに注入済み（Gridでラップ済み）かチェック
      if (currentPage.Content is not Grid containerGrid || containerGrid.StyleId != "SnackBarContainer")
      {
        // 元々のページの中身を取得
        var originalContent = currentPage.Content;

        // 新しい SnackBar インスタンスを作成
        var snackBar = new SnackBarView
        {
          VerticalOptions = LayoutOptions.End,
          HorizontalOptions = LayoutOptions.Fill,
          Margin = new Thickness(16),
          ZIndex = 999
        };

        // ラップ用の Grid を作成
        var newGrid = new Grid
        {
          StyleId = "SnackBarContainer",
          HorizontalOptions = LayoutOptions.Fill,
          VerticalOptions = LayoutOptions.Fill
        };

        // ページの中身を新しい Grid に差し替える
        currentPage.Content = newGrid;

        // 元の中身を Grid に入れ、その上に SnackBar を載せる
        if (originalContent != null)
        {
          newGrid.Children.Add(originalContent);
        }
        newGrid.Children.Add(snackBar);

        // SnackBarHost をこの新しいインスタンスで初期化
        SnackBarHost.Initialize(snackBar);

        System.Diagnostics.Debug.WriteLine("★★★ SnackBar Injection Success!");
      }
    }
  }


  //   public AppShell()
  // {
  //   InitializeComponent();
  //   System.Diagnostics.Debug.WriteLine("★★★ AppShell Constructor Start");
  //   Routing.RegisterRoute("ProjectDetailPage", typeof(Pages.ProjectDetailPage));
  // }

  // protected override void OnHandlerChanged()
  // {
  //   base.OnHandlerChanged();
  //   if (Handler != null)
  //   {
  //     System.Diagnostics.Debug.WriteLine("★★★ AppShell Handler Changed (View is ready)");
  //   }
  // }
}

