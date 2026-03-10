using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CharacomMaui.Application.Logging;

namespace CharacomMaui.Presentation.ViewModels;

public class LogListViewModel : INotifyPropertyChanged
{
  private readonly ILogQueryService _logQueryService;

  public ObservableCollection<LogDto> Logs { get; } = new();

  // 元データ
  private List<LogDto> _allLogs = new();

  public LogListViewModel(ILogQueryService logQueryService)
  {
    _logQueryService = logQueryService;
    SelectedDate = DateTime.Today;
    SelectedLevel = "All";
  }

  // Levelフィルタ
  public ObservableCollection<string> LevelFilters { get; } =
  [
      "All",
        "User Error",
        "User Warning",
        "User Info",
        "System Error",
        "System Warning",
        "System Info"
  ];
  private int page = 1;
  private int totalPages = 1;
  private const int PageSize = 50;

  private string _selectedLevel = "All";
  public string SelectedLevel
  {
    get => _selectedLevel;
    set
    {
      if (SetProperty(ref _selectedLevel, value))
        ApplyFilter();
    }
  }

  private DateTime _selectedDate = DateTime.Today;
  public DateTime SelectedDate
  {
    get => _selectedDate;
    set
    {
      if (SetProperty(ref _selectedDate, value))
      {
        page = 1;
        _ = ReloadLogsAsync();
      }
    }
  }
  private bool _isLoading;
  public bool IsLoading
  {
    get => _isLoading;
    set
    {
      SetProperty(ref _isLoading, value);
      OnPropertyChanged(nameof(IsNotLoading));
    }
  }
  private bool _canGoNext;
  public bool CanGoNext
  {
    get => _canGoNext;
    set => SetProperty(ref _canGoNext, value);
  }

  private bool _canGoPrev;
  public bool CanGoPrev
  {
    get => _canGoPrev;
    set => SetProperty(ref _canGoPrev, value);
  }
  public bool IsNotLoading => !IsLoading;
  public ICommand ClearFilterCommand => new Command(ClearFilter);
  public ICommand NextButtonCommand => new Command(NextPage);
  public ICommand PrevButtonCommand => new Command(PrevPage);
  private void ClearFilter()
  {
    SelectedLevel = "All";
    SelectedDate = DateTime.Today;
    ApplyFilter();
  }
  private void UpdatePagingState()
  {
    CanGoNext = page < totalPages;
    CanGoPrev = page > 1;
  }
  private async void NextPage()
  {
    if (page >= totalPages) return;
    page++;
    await ReloadLogsAsync();
  }

  private async void PrevPage()
  {
    if (page < 2) return;
    page--;
    await ReloadLogsAsync();
  }

  private async Task ReloadLogsAsync()
  {
    IsLoading = true;
    var result = await _logQueryService.GetLogsAsync(SelectedDate, PageSize, page);

    _allLogs = result.Logs;
    totalPages = (int)Math.Ceiling(result.LogsCount / (double)PageSize);
    ApplyFilter();
    UpdatePagingState();
    IsLoading = false;

    System.Diagnostics.Debug.WriteLine($"page = {page} / {totalPages} prev={CanGoPrev} next={CanGoNext}");
  }
  public async Task LoadAsync()
  {
    IsLoading = true;
    var result = await _logQueryService.GetLogsAsync(SelectedDate, PageSize);
    if (result == null) return;

    _allLogs = result.Logs;
    totalPages = (int)Math.Ceiling(result.LogsCount / (double)PageSize);
    page = 1;
    ApplyFilter();
    UpdatePagingState();
    IsLoading = false;

    System.Diagnostics.Debug.WriteLine($"page = {page} / {totalPages} prev={CanGoPrev} next={CanGoNext}");
  }

  private void ApplyFilter()
  {
    // データがロードされていない時は何もしない
    if (_allLogs == null || !_allLogs.Any()) return;

    IEnumerable<LogDto> query = _allLogs;

    // Levelフィルタ
    if (SelectedLevel != "All")
    {
      query = query.Where(x => x.Level == SelectedLevel);
    }

    // Dateフィルタ (TryParseの結果を考慮)
    query = query.Where(x =>
    {
      if (DateTime.TryParse(x.CreatedAt, out var dt))
      {
        return dt.Date == SelectedDate.Date;
      }
      return false;
    });

    // MainThreadでコレクションを更新（念のため）
    MainThread.BeginInvokeOnMainThread(() =>
    {
      Logs.Clear();
      foreach (var log in query)
      {
        Logs.Add(log);
      }
    });
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  protected bool SetProperty<T>(
      ref T backingStore,
      T value,
      [CallerMemberName] string propertyName = "")
  {
    if (EqualityComparer<T>.Default.Equals(backingStore, value))
      return false;

    backingStore = value;

    PropertyChanged?.Invoke(this,
        new PropertyChangedEventArgs(propertyName));

    return true;
  }
}