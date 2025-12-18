using System.Collections.Specialized;
using System.ComponentModel;

namespace CharacomMaui.Presentation.Models;

public class SelectBarContents : INotifyPropertyChanged
{
  public string Name { get; set; } = string.Empty;
  public int Count { get; set; } = 0;
  public string Title { get; set; } = string.Empty;
  public bool IsDisabled { get; set; } = false;
  bool _isSelected;
  public bool IsSelected
  {
    get => _isSelected;
    set
    {
      if (_isSelected == value) return;
      _isSelected = value;
      PropertyChanged?.Invoke(this,
          new PropertyChangedEventArgs(nameof(IsSelected)));
    }
  }

  public event PropertyChangedEventHandler? PropertyChanged;
}