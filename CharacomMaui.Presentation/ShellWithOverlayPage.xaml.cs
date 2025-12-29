using CharacomMaui.Presentation.Components;
using CharacomMaui.Presentation.Services;

namespace CharacomMaui.Presentation;

public partial class ShellWithOverlayPage : ContentPage
{

  public ShellWithOverlayPage()
  {
    InitializeComponent();
    // SnackBar を static ホストに登録
    SnackBarHost.Initialize(SnackBar);
  }
}
