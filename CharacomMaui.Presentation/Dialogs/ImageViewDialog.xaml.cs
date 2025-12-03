using CommunityToolkit.Maui.Views;
using CharacomMaui.Domain.Entities;
using UraniumUI.Dialogs;
using System.ComponentModel;
using System.Diagnostics;
using CharacomMaui.Presentation.ViewModels;
using UraniumUI.Views;

namespace CharacomMaui.Presentation.Dialogs;

public partial class ImageViewDialog : Popup
{
  public ImageViewDialog(ImageSource imageSource)
  {
    InitializeComponent();
    PopupImage.Source = imageSource;
  }
  private void OnCloseClicked(object sender, EventArgs e)
  {
    CloseAsync();
  }

}
