namespace CharacomMaui.Presentation.Components;

using CharacomMaui.Domain.Entities;
using CharacomMaui.Presentation.Helpers;
using MauiApp = Microsoft.Maui.Controls.Application;

public partial class CharaDataProgressRow : ContentView
{
  private const int DoubleClickTime = 300; // ダブルクリック判定の最大間隔(ms)
  private DateTime _lastTapTime;
  private CancellationTokenSource? _cts;

  public CharaDataProgressRow()
  {
    InitializeComponent();
    this.BindingContextChanged += (s, e) =>
    {
      UpdateBackground(IsSelected);
    };
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

  // イベント
  public event EventHandler<CharaDataProgressRowEventArgs>? RowClicked;
  public event EventHandler<CharaDataProgressRowEventArgs>? RowDoubleClicked;

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
    if (bindable is CharaDataProgressRow row && newValue is bool isSelected)
    {
      System.Diagnostics.Debug.WriteLine($"OnIsSelectedChange -> charaName={row.CharaName} MaterialName={row.MaterialName} isSelect={row.IsSelected}");
      row.UpdateBackground(isSelected);
    }
  }

  protected override void OnBindingContextChanged()
  {
    base.OnBindingContextChanged();

    if (BindingContext is CharaDataSummary data)
    {
      UpdateSelectionState(data.IsSelected);
    }
    else
    {
      // バインド解除された瞬間にも一応リセット
      UpdateSelectionState(false);
    }
  }

  private void UpdateSelectionState(bool isSelected)
  {
    if (isSelected)
    {
      BackgroundBorder.BackgroundColor = ThemeHelper.GetColor("Secondary");
      CharaNameLabel.TextColor = ThemeHelper.GetColor("OnSecondary");
      MaterialNameLabel.TextColor = ThemeHelper.GetColor("OnSecondary");
      CharaCountLabel.TextColor = ThemeHelper.GetColor("OnSecondary");
      SelectedCountLabel.TextColor = ThemeHelper.GetColor("OnSecondary");
    }
    else
    {
      CharaNameLabel.TextColor = ThemeHelper.GetColor("OnSurface");
      MaterialNameLabel.TextColor = ThemeHelper.GetColor("OnSurface");
      CharaCountLabel.TextColor = ThemeHelper.GetColor("OnSurface");
      SelectedCountLabel.TextColor = ThemeHelper.GetColor("OnSurface");
    }
  }
  private void UpdateBackground(bool isSelected)
  {


    // BindingContext から Number を取り出す
    int number = 0;
    if (BindingContext != null)
    {
      var prop = BindingContext.GetType().GetProperty("Number");
      if (prop != null)
      {
        number = (int)(prop.GetValue(BindingContext) ?? 0);
      }
    }

    bool isOdd = number % 2 == 1;

    BackgroundBorder.BackgroundColor = isOdd ? ThemeHelper.GetColor("DisabledBackground") : ThemeHelper.GetColor("Surface");

  }


  private static void OnDataChanged(BindableObject bindable, object oldValue, object newValue)
  {
    var view = (CharaDataProgressRow)bindable;

    int total = view.CharaCount;
    int selected = view.SelectedCount;

    if (total <= 0)
    {
      view.SelectedRatio = new GridLength(0, GridUnitType.Star);
      view.UnselectedRatio = new GridLength(1, GridUnitType.Star);
      return;
    }

    int unselected = Math.Max(total - selected, 0);

    view.SelectedRatio = new GridLength(selected, GridUnitType.Star);
    view.UnselectedRatio = new GridLength(unselected, GridUnitType.Star);
    System.Diagnostics.Debug.WriteLine($"selected:{selected} - unselected:{unselected}");
  }
}
public class CharaDataProgressRowEventArgs : EventArgs
{
  public string CharaName { get; set; } = string.Empty;
  public string MaterialName { get; set; } = string.Empty;
}