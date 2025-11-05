using System.ComponentModel;
using CharacomMaui.Domain.Entities;

public class CreateProjectViewModel : INotifyPropertyChanged
{
  public List<BoxItem> Folders { get; }

  private BoxItem _selectedFolder = new();
  public BoxItem SelectedFolder
  {
    get => _selectedFolder;
    set
    {
      if (_selectedFolder != value)
      {
        _selectedFolder = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedFolder)));
        System.Diagnostics.Debug.WriteLine($"Selected -> {_selectedFolder.Name}");
      }
    }
  }

  public event PropertyChangedEventHandler PropertyChanged;

  public CreateProjectViewModel(List<BoxItem> folders)
  {
    Folders = folders;
    SelectedFolder = Folders.FirstOrDefault();
  }

}

