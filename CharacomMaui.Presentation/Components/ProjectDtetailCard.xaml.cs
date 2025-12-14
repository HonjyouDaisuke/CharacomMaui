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


  // イベントを追加
  public event EventHandler<ProjectInfoEventArgs>? UpdateRequested;
  public event EventHandler<ProjectInfoEventArgs>? DeleteRequested;
  public event EventHandler<ProjectInfoEventArgs>? InviteRequested;

  public ProjectDetailCard()
  {
    InitializeComponent();
  }
  private void OnUpdateClicked(object sender, EventArgs e)
  {
    // アップデートリクエスト
    System.Diagnostics.Debug.WriteLine($"Project {ProjectName} をアップデートします。 ID={ProjectId} folder={ProjectFolderId} charafolder={CharaFolderId}");

    UpdateRequested?.Invoke(this, new ProjectInfoEventArgs
    {
      ProjectId = ProjectId,
      ProjectName = ProjectName,
      ProjectDescription = ProjectDescription,
      ProjectFolderId = ProjectFolderId,
      CharaFolderId = CharaFolderId,
    });
  }
  private void OnDeleteClicked(object sender, EventArgs e)
  {
    // 削除リクエスト
    System.Diagnostics.Debug.WriteLine($"Project {ProjectName} を削除します。 ID={ProjectId} ");

    DeleteRequested?.Invoke(this, new ProjectInfoEventArgs
    {
      ProjectId = ProjectId,
      ProjectName = ProjectName,
      ProjectDescription = ProjectDescription,
      ProjectFolderId = ProjectFolderId,
      CharaFolderId = CharaFolderId,
    });
  }
  private void OnInviteClicked(object sender, EventArgs e)
  {
    // 招待リクエスト
    System.Diagnostics.Debug.WriteLine($"Project {ProjectName} に招待します。 ID={ProjectId} ");

    InviteRequested?.Invoke(this, new ProjectInfoEventArgs
    {
      ProjectId = ProjectId,
      ProjectName = ProjectName,
      ProjectDescription = ProjectDescription,
      ProjectFolderId = ProjectFolderId,
      CharaFolderId = CharaFolderId,
    });
  }
  public static readonly BindableProperty IsUpdateVisibleProperty =
          BindableProperty.Create(
              nameof(IsUpdateVisible),
              typeof(bool),
              typeof(ProjectDetailCard),
              true);

  public bool IsUpdateVisible
  {
    get => (bool)GetValue(IsUpdateVisibleProperty);
    set => SetValue(IsUpdateVisibleProperty, value);
  }

  public static readonly BindableProperty IsDeleteVisibleProperty =
      BindableProperty.Create(
          nameof(IsDeleteVisible),
          typeof(bool),
          typeof(ProjectDetailCard),
          true);

  public bool IsDeleteVisible
  {
    get => (bool)GetValue(IsDeleteVisibleProperty);
    set => SetValue(IsDeleteVisibleProperty, value);
  }

  public static readonly BindableProperty IsInviteVisibleProperty =
      BindableProperty.Create(
          nameof(IsInviteVisible),
          typeof(bool),
          typeof(ProjectDetailCard),
          true);

  public bool IsInviteVisible
  {
    get => (bool)GetValue(IsInviteVisibleProperty);
    set => SetValue(IsInviteVisibleProperty, value);
  }
}