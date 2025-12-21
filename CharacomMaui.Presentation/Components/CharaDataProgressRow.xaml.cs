namespace CharacomMaui.Presentation.Components;

using CharacomMaui.Domain.Entities;
using CharacomMaui.Presentation.Helpers;
using MauiApp = Microsoft.Maui.Controls.Application;
using CharacomMaui.Presentation.Models;

public partial class CharaDataProgressRow : ContentView
{
  private const int DoubleClickTime = 300; // ダブルクリック判定の最大間隔(ms)
  private DateTime _lastTapTime;
  private CancellationTokenSource? _cts;

  public CharaDataProgressRow()
  {
    InitializeComponent();
    // 初期状態を明示
    // VisualStateManager.GoToState(BackgroundBorder, "Normal");
  }

  public string CharaName { get => (string)GetValue(CharaNameProperty); set => SetValue(CharaNameProperty, value); }
  public string MaterialName { get => (string)GetValue(MaterialNameProperty); set => SetValue(MaterialNameProperty, value); }
  public int CharaCount { get => (int)GetValue(CharaCountProperty); set => SetValue(CharaCountProperty, value); }
  public int SelectedCount { get => (int)GetValue(SelectedCountProperty); set => SetValue(SelectedCountProperty, value); }
  public GridLength SelectedRatio { get => (GridLength)GetValue(SelectedRatioProperty); set => SetValue(SelectedRatioProperty, value); }
  public GridLength UnselectedRatio { get => (GridLength)GetValue(UnselectedRatioProperty); set => SetValue(UnselectedRatioProperty, value); }



  public static readonly BindableProperty CharaNameProperty =
    BindableProperty.Create(nameof(CharaName), typeof(string), typeof(CharaDataProgressRow), string.Empty);
  public static readonly BindableProperty MaterialNameProperty =
    BindableProperty.Create(nameof(MaterialName), typeof(string), typeof(CharaDataProgressRow), string.Empty);
  public static readonly BindableProperty CharaCountProperty =
    BindableProperty.Create(nameof(CharaCount), typeof(int), typeof(CharaDataProgressRow), 0, propertyChanged: OnDataChanged);
  public static readonly BindableProperty SelectedCountProperty =
    BindableProperty.Create(nameof(SelectedCount), typeof(int), typeof(CharaDataProgressRow), 0, propertyChanged: OnDataChanged);
  public static readonly BindableProperty SelectedRatioProperty =
    BindableProperty.Create(nameof(SelectedRatio), typeof(GridLength), typeof(CharaDataProgressRow), new GridLength(1, GridUnitType.Star));
  public static readonly BindableProperty UnselectedRatioProperty =
    BindableProperty.Create(nameof(UnselectedRatio), typeof(GridLength), typeof(CharaDataProgressRow), new GridLength(1, GridUnitType.Star));

  public static readonly BindableProperty IsSelectedProperty =
      BindableProperty.Create(
          nameof(IsSelected),
          typeof(bool),
          typeof(CharaDataProgressRow),
          false,
          propertyChanged: OnIsSelectedChanged);

  public bool IsSelected
  {
    get => (bool)GetValue(IsSelectedProperty);
    set => SetValue(IsSelectedProperty, value);
  }

  public static readonly BindableProperty IsOddProperty =
    BindableProperty.Create(
      nameof(IsOdd),
      typeof(bool),
      typeof(CharaDataProgressRow),
      false,
      propertyChanged: OnIsOddChanged);


  public bool IsOdd
  {
    get => (bool)GetValue(IsOddProperty);
    set => SetValue(IsOddProperty, value);
  }

  // イベント
  public event EventHandler<CharaDataProgressRowEventArgs>? RowClicked;
  public event EventHandler<CharaDataProgressRowEventArgs>? RowDoubleClicked;
  protected override void OnPropertyChanged(string? propertyName = null)
  {
    base.OnPropertyChanged(propertyName);

    if (propertyName == IsEnabledProperty.PropertyName)
    {
      if (IsEnabled)
      {
        // 🔥 Disabled から必ず引き戻す
        UpdateVisualState();
      }
    }
  }
  // クリック時の処理
  private void OnCardTapped(object? sender, EventArgs e)
  {
    // すでに遅延処理がある場合 → ダブルクリック
    System.Diagnostics.Debug.WriteLine($"TAP instance: {this.GetHashCode()}");
    if (_cts != null)
    {
      System.Diagnostics.Debug.WriteLine("DoubleTap: Cancel called");
      _cts.Cancel();
      _cts.Dispose();
      _cts = null;

      RowDoubleClicked?.Invoke(this, new CharaDataProgressRowEventArgs
      {
        CharaName = CharaName,
        MaterialName = MaterialName
      });
      return; // ここで早期リターン
    }

    // シングルクリックの遅延処理を準備
    _cts = new CancellationTokenSource();
    var token = _cts.Token;

    Task.Delay(DoubleClickTime, token).ContinueWith(t =>
    {
      if (t.IsCanceled)
      {
        System.Diagnostics.Debug.WriteLine("SingleTap: CANCELED");
        return;
      }

      System.Diagnostics.Debug.WriteLine("SingleTap: EXECUTED");

      MainThread.BeginInvokeOnMainThread(() =>
      {
        RowClicked?.Invoke(this, new CharaDataProgressRowEventArgs
        {
          CharaName = CharaName,
          MaterialName = MaterialName
        });
      });

      _cts?.Dispose();
      _cts = null;

    }, token);
  }

  //背景色の切り替え
  private static void OnIsSelectedChanged(BindableObject bindable, object oldValue, object newValue)
  {
    if (bindable is CharaDataProgressRow row)
      row.UpdateVisualState();
  }
  private static void OnIsOddChanged(BindableObject bindable, object oldValue, object newValue)
  {
    if (bindable is CharaDataProgressRow row)
      row.UpdateVisualState();
  }

  private void OnBackgroundLoaded(object? sender, EventArgs e)
  {
    UpdateVisualState();
  }

  protected override void OnBindingContextChanged()
  {
    base.OnBindingContextChanged();
    UpdateVisualState();
  }

  private void OnProgressGridLoaded(object? sender, EventArgs e)
  {
    UpdateBarWidths(this);
  }

  private static void OnDataChanged(BindableObject bindable, object oldValue, object newValue)
  {
    var view = (CharaDataProgressRow)bindable;
    UpdateBarWidths(view);
  }

  private void UpdateVisualState()
  {
    if (IsSelected)
    {
      VisualStateManager.GoToState(BackgroundBorder, "Selected");
      System.Diagnostics.Debug.WriteLine($"CharaName = {CharaName} Selected = {SelectedCount}  ★☆Selected!");
    }
    else
    {
      VisualStateManager.GoToState(
        BackgroundBorder,
        IsOdd ? "NormalOdd" : "NormalEven");
      System.Diagnostics.Debug.WriteLine($"CharaName = {CharaName} Selected = {SelectedCount}  ★☆Normal{IsOdd}");
    }

  }
  private static void UpdateBarWidths(CharaDataProgressRow view)
  {
    int total = view.CharaCount;
    int selected = view.SelectedCount;

    double ratio = total == 0 ? 0 : (double)selected / total;

    AbsoluteLayout.SetLayoutBounds(
        view.SelectedBar,
        new Rect(0, 0, ratio, 1)
    );
  }
}
public class CharaDataProgressRowEventArgs : EventArgs
{
  public string CharaName { get; set; } = string.Empty;
  public string MaterialName { get; set; } = string.Empty;
}