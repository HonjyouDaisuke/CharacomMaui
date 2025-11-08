using CommunityToolkit.Maui.Views;
using CharacomMaui.Domain.Entities;
using Microsoft.Maui.Controls;
using UraniumUI.Dialogs;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CharacomMaui.Presentation.Dialogs;

public partial class CreateProjectDialog : Popup
{
  public string ProjectName => ProjectNameEntry.Text;
  public string ProjectDescription => ProjectDescriptionEditor.Text;
  public BoxItem SelectedTopFolder => (BoxItem)TopFolderDropdown.SelectedItem;
  public BoxItem SelectedCharaFolder => (BoxItem)CharaFolderDropdown.SelectedItem;
  private IDialogService _dialogService;
  private CreateProjectViewModel _viewModel;

  public CreateProjectDialog(List<BoxItem> topFolders, IDialogService dialogService, CreateProjectViewModel viewModel)
  {
    InitializeComponent();
    _dialogService = dialogService;
    _viewModel = viewModel;

    TopFolderDropdown.ItemsSource = topFolders;
    TopFolderDropdown.ItemDisplayBinding = new Binding("Name");
    // PropertyChanged で SelectedItem の変更を監視
    TopFolderDropdown.PropertyChanged += TopFolderDropdownPropertyChanged;

    List<BoxItem> charaFolders = new List<BoxItem>();
    BoxItem charaFolderItem = new()
    {
      Name = "選択なし",
      Id = "",
      Type = "",
    };
    charaFolders.Add(charaFolderItem);
    CharaFolderDropdown.ItemsSource = charaFolders;
    CharaFolderDropdown.ItemDisplayBinding = new Binding("Name");
  }

  private async void TopFolderDropdownPropertyChanged(object? sender, PropertyChangedEventArgs e)
  {

    if (e.PropertyName == nameof(TopFolderDropdown.SelectedItem))
    {
      var selected = TopFolderDropdown.SelectedItem as BoxItem;
      if (selected != null)
      {
        System.Diagnostics.Debug.WriteLine($"選択されたTopFolder: {selected.Name}");
        var charaFolders = await _viewModel.GetFolderItemsAsync(selected.Id);

        CharaFolderDropdown.ItemsSource.Clear();
        CharaFolderDropdown.ItemsSource = charaFolders;
      }
    }
  }

  private async void OnOkClicked(object sender, EventArgs e)
  {
    using (await _dialogService.DisplayProgressAsync("Loading", "Work in progress, please wait..."))
    {
      // Indicate a long running operation
      await Task.Delay(5000);
    }
    await CloseAsync(); // Close に渡す値は任意。複数渡したい場合は Tuple かクラスにまとめる
  }

  private void OnCancelClicked(object sender, EventArgs e)
  {
    CloseAsync();
  }

}
