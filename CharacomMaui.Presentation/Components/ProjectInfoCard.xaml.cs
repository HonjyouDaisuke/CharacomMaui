namespace CharacomMaui.Presentation.Components;

public partial class ProjectInfoCard : ContentView
{
  public ProjectInfoCard()
  {
    InitializeComponent();
  }

  public static readonly BindableProperty ProjectNameProperty =
      BindableProperty.Create(nameof(ProjectName), typeof(string), typeof(ProjectInfoCard), string.Empty);
  public static readonly BindableProperty CharaCountProperty = BindableProperty.Create(nameof(CharaCount), typeof(int), typeof(ProjectInfoCard), 0);
  public static readonly BindableProperty UserCountProperty = BindableProperty.Create(nameof(UserCount), typeof(int), typeof(ProjectInfoCard), 0);


  public string ProjectName
  {
    get => (string)GetValue(ProjectNameProperty);
    set => SetValue(ProjectNameProperty, value);
  }
  public int CharaCount
  {
    get => (int)GetValue(CharaCountProperty);
    set => SetValue(CharaCountProperty, value);
  }
  public int UserCount
  {
    get => (int)GetValue(UserCountProperty);
    set => SetValue(UserCountProperty, value);
  }
}