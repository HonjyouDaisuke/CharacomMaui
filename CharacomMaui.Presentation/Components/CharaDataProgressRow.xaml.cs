namespace CharacomMaui.Presentation.Components;

using CharacomMaui.Domain.Entities;
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

  /// <summary>
  /// Handles changes to the IsSelected bindable property and updates the row background accordingly.
  /// </summary>
  /// <param name="bindable">The bindable object whose property changed (expected to be a CharaDataProgressRow).</param>
  /// <param name="oldValue">The previous value of the property.</param>
  /// <param name="newValue">The new value of the property (expected to be a bool indicating selection state).</param>
  private static void OnIsSelectedChanged(BindableObject bindable, object oldValue, object newValue)
  {
    if (bindable is CharaDataProgressRow row && newValue is bool isSelected)
    {
      System.Diagnostics.Debug.WriteLine($"OnIsSelectedChange -> charaName={row.CharaName} MaterialName={row.MaterialName} isSelect={row.IsSelected}");
      row.UpdateBackground(isSelected);
    }
  }

  /// <summary>
  /// Resolve a theme-aware color resource by base key.
  /// </summary>
  /// <param name="key">The base resource key; "Dark" is appended when the app is using the dark theme.</param>
  /// <returns>The Color from application resources matching the theme-adjusted key, or <see cref="Colors.Transparent"/> if not found.</returns>
  private static Color GetColor(string key)
  {
    string LorD = MauiApp.Current!.RequestedTheme == AppTheme.Light ? "" : "Dark";
    key = $"{key}{LorD}";
    if (MauiApp.Current!.Resources.TryGetValue(key, out var value) && value is Color color)
      return color;

    return Colors.Transparent;
  }
  /// <summary>
  /// Applies the view's selection styling to match the new BindingContext.
  /// </summary>
  /// <remarks>
  /// If the new BindingContext is a <c>CharaDataSummary</c>, the view's selection state is set to that object's <c>IsSelected</c> value; otherwise the selection state is reset to false.
  /// </remarks>
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

  /// <summary>
  /// Apply the visual selection state to the row's background and text colors.
  /// </summary>
  /// <param name="isSelected">True to apply the selected theme (secondary background and on-secondary text); false to apply the default surface/on-surface text colors.</param>
  private void UpdateSelectionState(bool isSelected)
  {
    if (isSelected)
    {
      BackgroundBorder.BackgroundColor = GetColor("Secondary");
      CharaNameLabel.TextColor = GetColor("OnSecondary");
      MaterialNameLabel.TextColor = GetColor("OnSecondary");
      CharaCountLabel.TextColor = GetColor("OnSecondary");
      SelectedCountLabel.TextColor = GetColor("OnSecondary");
    }
    else
    {
      CharaNameLabel.TextColor = GetColor("OnSurface");
      MaterialNameLabel.TextColor = GetColor("OnSurface");
      CharaCountLabel.TextColor = GetColor("OnSurface");
      SelectedCountLabel.TextColor = GetColor("OnSurface");
    }
  }
  /// <summary>
  /// Updates the row background color based on the parity of the BindingContext's `Number` property.
  /// </summary>
  /// <param name="isSelected">Present for API compatibility; this method determines background solely from BindingContext.`Number` parity.</param>
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

    BackgroundBorder.BackgroundColor = isOdd ? GetColor("Desiabled") : GetColor("Surface");

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