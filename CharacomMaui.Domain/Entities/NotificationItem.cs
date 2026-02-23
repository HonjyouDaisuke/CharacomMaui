using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace CharacomMaui.Domain.Entities;

public class NotificationItem : INotifyPropertyChanged
{
  [JsonPropertyName("id")]
  public string Id { get; set; } = string.Empty;
  [JsonPropertyName("title")]
  public string Title { get; set; } = string.Empty;
  [JsonPropertyName("message")]
  public string Message { get; set; } = string.Empty;
  [JsonPropertyName("type_id")]
  public string TypeId { get; set; } = string.Empty;
  [JsonPropertyName("created_at")]
  public string CreatedAt { get; set; } = string.Empty;
  private bool _isRead = false;
  [JsonPropertyName("is_read")]

  public bool IsRead
  {
    get => _isRead;
    set
    {
      if (_isRead == value) return;
      _isRead = value;
      OnPropertyChanged();
    }
  }

  public event PropertyChangedEventHandler PropertyChanged;
  protected void OnPropertyChanged([CallerMemberName] string name = null)
      => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}