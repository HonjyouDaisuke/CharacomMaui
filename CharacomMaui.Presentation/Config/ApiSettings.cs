namespace CharacomMaui.Presentation.Config;

public class ApiSettings
{
  public string BaseUrl { get; set; } = string.Empty;
  public ApiEndpoints Endpoints { get; set; } = new();
}

public class ApiEndpoints
{
  public string GetUserInfo { get; set; } = default!;
  public string GetTopFolders { get; set; } = default!;
  public string AnotherApi { get; set; } = default!;
}
