using CommunityToolkit.Maui.Views;
using CharacomMaui.Domain.Entities;
using UraniumUI.Dialogs;
using System.ComponentModel;
using System.Diagnostics;
using CharacomMaui.Presentation.ViewModels;
using System.Threading.Tasks;
using System.Data.SqlTypes;

namespace CharacomMaui.Presentation.Dialogs;

public sealed class CreateProjectResult
{
  public bool IsCanceled { get; init; }
  public Project? Project { get; init; }
}
public partial class CreateProjectDialog : Popup
{
  public string ProjectName => ProjectNameEntry.Text;
  public string ProjectDescription => ProjectDescriptionEditor.Text;
  public bool IsCanceled { get; private set; } = false;
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

  public event Func<CreateProjectResult, Task>? Completed;
  public event Action? Finished;
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
    await OnOkClickedAsync(sender, e);
  }
  private async Task OnOkClickedAsync(object sender, EventArgs e)
  {
    try
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
        if (Completed != null)
        {
          await Completed.Invoke(new CreateProjectResult
          {
            IsCanceled = false,
            Project = project
          });
        }
      }
    }
    finally
    {
      Finished?.Invoke();
      IsCanceled = false;
      await CloseAsync();
    }
  }

  private async void OnCancelClicked(object sender, EventArgs e)
  {
    try
    {
      IsCanceled = true;
    }
    finally
    {
      Finished?.Invoke();
      await CloseAsync();
    }
  }

}