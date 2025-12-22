using System.Threading.Tasks;

namespace CharacomMaui.Presentation.Components;

public partial class ProjectDetailCard : ContentView
{

  // ========== ProjectId ==========
  public static readonly BindableProperty ProjectIdProperty =
    BindableProperty.Create(nameof(ProjectId), typeof(string), typeof(ProjectDetailCard), string.Empty);
  public string ProjectId { get => (string)GetValue(ProjectIdProperty); set => SetValue(ProjectIdProperty, value); }

  // ========== ProjectName ==========
  public static readonly BindableProperty ProjectNameProperty =
    BindableProperty.Create(nameof(ProjectName), typeof(string), typeof(ProjectDetailCard), string.Empty);
  public string ProjectName { get => (string)GetValue(ProjectNameProperty); set => SetValue(ProjectNameProperty, value); }
  // ========== ProjectDescription ==========
  public static readonly BindableProperty ProjectDescriptionProperty =
    BindableProperty.Create(nameof(ProjectDescription), typeof(string), typeof(ProjectDetailCard), string.Empty);
  public string ProjectDescription { get => (string)GetValue(ProjectDescriptionProperty); set => SetValue(ProjectDescriptionProperty, value); }

  // ========== ParticipantsText ==========
  public static readonly BindableProperty ParticipantsTextProperty =
    BindableProperty.Create(nameof(ParticipantsText), typeof(string), typeof(ProjectDetailCard), string.Empty);
  public string ParticipantsText { get => (string)GetValue(ParticipantsTextProperty); set => SetValue(ParticipantsTextProperty, value); }
  // ========== Created date ==========
  public static readonly BindableProperty CreatedAtProperty =
    BindableProperty.Create(nameof(CreatedAt), typeof(string), typeof(ProjectDetailCard), string.Empty);
  public string CreatedAt { get => (string)GetValue(CreatedAtProperty); set => SetValue(CreatedAtProperty, value); }

  // ========== Updated date ==========
  public static readonly BindableProperty UpdatedAtProperty =
    BindableProperty.Create(nameof(UpdatedAt), typeof(string), typeof(ProjectDetailCard), string.Empty);
  public string UpdatedAt { get => (string)GetValue(UpdatedAtProperty); set => SetValue(UpdatedAtProperty, value); }

  // ========== CreatedBy ==========
  public static readonly BindableProperty CreatedByProperty =
    BindableProperty.Create(nameof(CreatedBy), typeof(string), typeof(ProjectDetailCard), string.Empty);
  public string CreatedBy { get => (string)GetValue(CreatedByProperty); set => SetValue(CreatedByProperty, value); }

  // ========== ProjectFolderId ==========
  public static readonly BindableProperty ProjectFolderIdProperty =
    BindableProperty.Create(nameof(ProjectFolderId), typeof(string), typeof(ProjectDetailCard), string.Empty);

  public string ProjectFolderId { get => (string)GetValue(ProjectFolderIdProperty); set => SetValue(ProjectFolderIdProperty, value); }
  // ========== CharaFolderId ===========
  public static readonly BindableProperty CharaFolderIdProperty =
    BindableProperty.Create(nameof(CharaFolderId), typeof(string), typeof(ProjectDetailCard), string.Empty);
  public string CharaFolderId { get => (string)GetValue(CharaFolderIdProperty); set => SetValue(CharaFolderIdProperty, value); }

  // ========== IsUpdateVisible ==========
  public static readonly BindableProperty IsUpdateVisibleProperty =
    BindableProperty.Create(nameof(IsUpdateVisible), typeof(bool), typeof(ProjectDetailCard), true);
  public bool IsUpdateVisible { get => (bool)GetValue(IsUpdateVisibleProperty); set => SetValue(IsUpdateVisibleProperty, value); }

  // ========== IsDeleteVisible ==========
  public static readonly BindableProperty IsDeleteVisibleProperty =
    BindableProperty.Create(nameof(IsDeleteVisible), typeof(bool), typeof(ProjectDetailCard), true);
  public bool IsDeleteVisible { get => (bool)GetValue(IsDeleteVisibleProperty); set => SetValue(IsDeleteVisibleProperty, value); }

  // ========== IsInviteVisible ===========
  public static readonly BindableProperty IsInviteVisibleProperty =
    BindableProperty.Create(nameof(IsInviteVisible), typeof(bool), typeof(ProjectDetailCard), true);
  public bool IsInviteVisible { get => (bool)GetValue(IsInviteVisibleProperty); set => SetValue(IsInviteVisibleProperty, value); }
  // イベントを追加
  public event Func<ProjectInfoEventArgs, Task>? UpdateRequested;
  public event Func<ProjectInfoEventArgs, Task>? DeleteRequested;
  public event Func<ProjectInfoEventArgs, Task>? InviteRequested;

  private bool _isActionProcessing = false;

  public ProjectDetailCard()
  {
    InitializeComponent();
  }

  private ProjectInfoEventArgs CreateArgs() => new()
  {
    ProjectId = ProjectId,
    ProjectName = ProjectName,
    ProjectDescription = ProjectDescription,
    ProjectFolderId = ProjectFolderId,
    CharaFolderId = CharaFolderId,
  };

  private async void OnUpdateClicked(object sender, EventArgs e)
  {
    await RunOnceAsync(async () =>
    {
      var handler = UpdateRequested;
      if (handler != null)
      {
        await handler(CreateArgs());
      }
    });
  }
  private async void OnDeleteClicked(object sender, EventArgs e)
  {
    await RunOnceAsync(async () =>
    {
      var handler = DeleteRequested;
      if (handler != null)
      {
        await handler(CreateArgs());
      }
    });
  }
  private async void OnInviteClicked(object sender, EventArgs e)
  {
    await RunOnceAsync(async () =>
    {
      var handler = InviteRequested;
      if (handler != null)
      {
        await handler(CreateArgs());
      }
    });
  }

  private async Task RunOnceAsync(
    Func<Task> action)
  {
    if (_isActionProcessing)
      return; // ← ここが重要

    _isActionProcessing = true;

    try
    {
      // 全ボタンをロック
      UpdateBtn.IsEnabled = false;
      DeleteBtn.IsEnabled = false;
      InviteBtn.IsEnabled = false;

      await action();
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[Error] {ex.Message}");
    }
    finally
    {
      UpdateBtn.IsEnabled = IsUpdateVisible;
      DeleteBtn.IsEnabled = IsDeleteVisible;
      InviteBtn.IsEnabled = IsInviteVisible;

      _isActionProcessing = false;
    }
  }

}

