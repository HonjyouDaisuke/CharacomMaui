using CommunityToolkit.Maui.Views;
using CharacomMaui.Domain.Entities;
using UraniumUI.Dialogs;
using System.ComponentModel;
using System.Diagnostics;
using CharacomMaui.Presentation.ViewModels;
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
  private readonly Project? _project;
  private readonly List<BoxItem> _topFolders = new List<BoxItem>();

  // ========== Title ==========
  public static readonly BindableProperty TitleProperty =
      BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(CreateProjectDialog),
        string.Empty);
  public string Title
  {
    get => (string)GetValue(TitleProperty);
    set => SetValue(TitleProperty, value);
  }
  public CreateProjectDialog(string title, List<BoxItem> topFolders, IDialogService dialogService, CreateProjectViewModel viewModel, Project? project = null)
  {
    InitializeComponent();
    _dialogService = dialogService;
    _viewModel = viewModel;

    // Popupが開いたら呼ばれる
    this.Opened += CreateProjectDialog_Opened;

    _project = project;
    _topFolders = topFolders;
    Title = title;

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

  private async void CreateProjectDialog_Opened(object? sender, EventArgs e)
  {
    if (_project == null) return;
    // プロジェクト名と説明をプリセット
    ProjectNameEntry.Text = _project.Name;
    ProjectDescriptionEditor.Text = _project.Description;

    if (string.IsNullOrEmpty(_project.FolderId)) return;
    // プロジェクトフォルダをプリセット
    foreach (var folder in _topFolders)
    {
      if (folder.Id != _project.FolderId) continue;

      TopFolderDropdown.SelectedItem = folder;
      break;
    }

    if (string.IsNullOrEmpty(_project.CharaFolderId)) return;
    // charaフォルダをプリセット
    var charaFolders = await _viewModel.GetFolderItemsAsync(_project.FolderId);
    foreach (var charaFolder in charaFolders)
    {
      if (charaFolder.Id != _project.CharaFolderId) continue;

      CharaFolderDropdown.SelectedItem = charaFolder;
      break;
    }

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
        if (CharaFolderDropdown.ItemsSource != null)
        {
          CharaFolderDropdown.SelectedItem = CharaFolderDropdown.ItemsSource[0];
        }
        ProjectNameEntry.Text = selected.Name;
      }
    }
  }

  private async void OnOkClicked(object sender, EventArgs e)
  {
    using (await _dialogService.DisplayProgressAsync("Loading", "Work in progress, please wait..."))
    {
      // Indicate a long running operation
      // エントリーの作成
      var project = new Project
      {
        Id = _project?.Id ?? string.Empty,
        Name = ProjectName,
        Description = ProjectDescription,
        FolderId = SelectedTopFolder.Id,
        CharaFolderId = SelectedCharaFolder.Id,
      };

      var res = await _viewModel.CreateOrUpdateProjectAsync(project);
      System.Diagnostics.Debug.WriteLine(res.ToString());
    }
    await CloseAsync(); // Close に渡す値は任意。複数渡したい場合は Tuple かクラスにまとめる
  }

  private void OnCancelClicked(object sender, EventArgs e)
  {
    CloseAsync();
  }
}