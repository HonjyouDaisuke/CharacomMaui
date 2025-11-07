using System.Threading.Tasks;
using Box.Sdk.Gen.Schemas;
using CharacomMaui.Application.Interfaces;
using CharacomMaui.Domain.Entities;
using CharacomMaui.Infrastructure.Services;
using CharacomMaui.Presentation.Dialogs;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using UraniumUI.Dialogs;

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

    /**
    var dialog = new MaterialDialogBuilder()
        .SetTitle("新規プロジェクト作成")
        .SetMessage("必要な情報を入力してください。")
        .AddDropdown<List<BoxItem>>("プロジェクトフォルダ", out var projectFolderPicker)
        .AddDropdown<List<BoxItem>>("データフォルダ", out var dataFolderPicker)
        .AddTextField("プロジェクト名", out var projectNameInput)
        .AddTextField("説明（任意）", out var descriptionInput, isMultiLine: true)
        .AddAction("作成")
        .AddCancelAction("キャンセル");

    var result = await dialog.ShowAsync();
    **/
    try
    {

      var popup = new CreateProjectDialog(folderItems);
      this.ShowPopup(popup, new PopupOptions
      {
        CanBeDismissedByTappingOutsideOfPopup = false
      });
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine("Popup show failed: " + ex);
    }





  }
}