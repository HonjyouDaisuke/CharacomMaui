using CharacomMaui.Domain.Entities;
using CommunityToolkit.Maui.Views;

namespace CharacomMaui.Presentation.Dialogs;

public partial class CreateProjectDialog : CommunityToolkit.Maui.Views.Popup
{
  public CreateProjectDialog(List<BoxItem> folderItems)
  {
    InitializeComponent();
    BindingContext = new CreateProjectViewModel(folderItems);
    System.Diagnostics.Debug.WriteLine("BaseType = " + this.GetType().BaseType);
  }

  private void OnOkClicked(object sender, EventArgs e)
  {

    CloseAsync(); // これで通る
  }

  private void OnCancelClicked(object sender, EventArgs e)
  {
    CloseAsync();
  }
}
