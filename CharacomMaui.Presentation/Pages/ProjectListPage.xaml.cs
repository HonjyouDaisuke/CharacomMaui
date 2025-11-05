using System.Threading.Tasks;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Infrastructure.Services;
using CharacomMaui.Presentation.Dialogs;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;

namespace CharacomMaui.Presentation.Pages;

public partial class ProjectListPage : ContentPage
{
  private readonly IBoxTopFolderRepository _repository;
  public ProjectListPage(IBoxTopFolderRepository repository)
  {
    InitializeComponent();
    _repository = repository;
  }

  private async void OnCreateProjectBtn(object? sender, EventArgs e)
  {
    LogEditor.Text += "プロジェクトの新規作成\n";
    var accessToken = Preferences.Get("app_access_token", string.Empty);
    var folderItems = await _repository.GetTopFolders(accessToken);
    LogEditor.Text += $"accessToken = {accessToken}\n";
    foreach (var item in folderItems)
    {
      LogEditor.Text += $"{item.Id}: {item.Name} ({item.Type})\n";
    }

    var popup = new CreateProjectDialog(folderItems);
    this.ShowPopup(popup, new PopupOptions
    {
      CanBeDismissedByTappingOutsideOfPopup = false
    });


  }
}