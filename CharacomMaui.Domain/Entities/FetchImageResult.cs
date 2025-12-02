namespace CharacomMaui.Domain.Entities;

public class FetchImageResult
{
  public bool Success { get; set; }
  public string? ErrorMessage { get; set; }
  public string? ContentType { get; set; }
  public byte[]? BinaryData { get; set; }
  public override string ToString()
  {
    if (Success)
    {
      return $"Success:{Success} --- ContentType:{ContentType} --- BinaryData Length:{BinaryData?.Length}";
    }
    else
    {
      return $"Success:{Success} --- ErrorMessage:{ErrorMessage}";
    }
  }
}