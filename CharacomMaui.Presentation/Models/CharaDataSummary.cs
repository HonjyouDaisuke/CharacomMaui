using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CharacomMaui.Presentation.Models;

public class CharaDataSummary : INotifyPropertyChanged
{
  public int Number { get; set; } = 0;
  public string CharaName { get; set; } = string.Empty;
  public string MaterialName { get; set; } = string.Empty;
  public int CharaCount { get; set; } = 0;
  public int SelectedCount { get; set; } = 0;
  public bool _isOdd;
  public bool IsOdd
  {
    get => _isOdd;
    set
    {
      if (_isOdd != value)
      {
        _isOdd = value;
        OnPropertyChanged();
      }
    }
  }
  private bool _isSelected;
  public bool IsSelected
  {
    get => _isSelected;
    set
    {
      if (_isSelected != value)
      {
        _isSelected = value;
        OnPropertyChanged();
      }
    }
  }
  public event PropertyChangedEventHandler? PropertyChanged;
  private void OnPropertyChanged([CallerMemberName] string? name = null)
  {
    System.Diagnostics.Debug.WriteLine($"CharaDataSummary OnPropertyChanged: {name} for {CharaName}-{MaterialName}");
    MainThread.BeginInvokeOnMainThread(() =>
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    });
  }
}
