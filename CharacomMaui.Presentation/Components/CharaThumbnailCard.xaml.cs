using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using MauiApp = Microsoft.Maui.Controls.Application;

namespace CharacomMaui.Presentation.Components;

public partial class CharaThumbnailCard : ContentView
{
  private const int DoubleClickTime = 300; // ダブルクリック判定の最大間隔(ms)
  private DateTime _lastTapTime;
  private CancellationTokenSource? _cts;
  public CharaThumbnailCard()
  {
    InitializeComponent();
    this.BindingContextChanged += (s, e) =>
    {
      UpdateBackground(IsSelected);
    };
  }

  // イベント
  public event EventHandler<CharaThumnailCardEventArgs>? CardClicked;
  public event EventHandler<CharaThumnailCardEventArgs>? CardDoubleClicked;

  // bindableProperty
  // ========== CharaId ==========
  public static readonly BindableProperty CharaIdProperty =
      BindableProperty.Create(
        nameof(CharaId),
        typeof(string),
        typeof(CharaThumbnailCard),
        string.Empty);
  public string CharaId
  {
    get => (string)GetValue(CharaIdProperty);
    set => SetValue(CharaIdProperty, value);
  }

  // ========== Title ==========
  public static readonly BindableProperty TitleProperty =
      BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(CharaThumbnailCard),
        string.Empty);
  public string Title
  {
    get => (string)GetValue(TitleProperty);
    set => SetValue(TitleProperty, value);
  }

  // ========== CanvasWidth ==========
  public static readonly BindableProperty CanvasWidthProperty =
      BindableProperty.Create(
          nameof(CanvasWidth),
          typeof(double),
          typeof(CharaThumbnailCard),
          160.0);
  public double CanvasWidth
  {
    get => (double)GetValue(CanvasWidthProperty);
    set => SetValue(CanvasWidthProperty, value);
  }

  // ========== CanvasHeight ==========
  public static readonly BindableProperty CanvasHeightProperty =
      BindableProperty.Create(
          nameof(CanvasHeight),
          typeof(double),
          typeof(CharaThumbnailCard),
          160.0);
  public double CanvasHeight
  {
    get => (double)GetValue(CanvasHeightProperty);
    set => SetValue(CanvasHeightProperty, value);
  }

  // ========== Bitmap ==========
  public static readonly BindableProperty BitmapProperty =
      BindableProperty.Create(
          nameof(Bitmap),
          typeof(SKBitmap),
          typeof(CharaThumbnailCard),
          null,
          propertyChanged: OnBitmapChanged);

  public SKBitmap? Bitmap
  {
    get => (SKBitmap)GetValue(BitmapProperty);
    set => SetValue(BitmapProperty, value);
  }

  private static void OnBitmapChanged(BindableObject bindable, object oldValue, object newValue)
  {
    var control = (CharaThumbnailCard)bindable;
    control.CharaImage.InvalidateSurface();
  }

  // ========== RawImageData ==========
  public static readonly BindableProperty RawImageDataProperty =
      BindableProperty.Create(
          nameof(RawImageData),
          typeof(byte[]),
          typeof(CharaThumbnailCard),
          null,
          propertyChanged: OnRawImageDataChanged);

  public byte[] RawImageData
  {
    get => (byte[])GetValue(RawImageDataProperty);
    set => SetValue(RawImageDataProperty, value);
  }

  private static void OnRawImageDataChanged(BindableObject bindable, object oldValue, object newValue)
  {
    var control = (CharaThumbnailCard)bindable;

    if (newValue is byte[] imageData && imageData.Length > 0)
    {
      using var stream = new MemoryStream(imageData);
      control.Bitmap = SKBitmap.Decode(stream);
    }
    else
    {
      control.Bitmap = null;
    }
    control.CharaImage.InvalidateSurface();
  }

  // ========== IsSelected ==========
  public static readonly BindableProperty IsSelectedProperty =
      BindableProperty.Create(
          nameof(IsSelected),
          typeof(bool),
          typeof(CharaThumbnailCard),
          false,
          propertyChanged: OnIsSelectedChanged);

  public bool IsSelected
  {
    get => (bool)GetValue(IsSelectedProperty);
    set => SetValue(IsSelectedProperty, value);
  }
  //背景色の切り替え
  private static void OnIsSelectedChanged(BindableObject bindable, object oldValue, object newValue)
  {
    if (bindable is CharaThumbnailCard card && newValue is bool isSelected)
    {
      card.UpdateBackground(isSelected);
    }
  }

  /// <summary>
  /// Updates the visual state of the card elements to either "Selected" or "Normal".
  /// </summary>
  /// <param name="isSelected">If true, applies the "Selected" visual state; otherwise applies the "Normal" visual state.</param>
  private void UpdateBackground(bool isSelected)
  {
    var state = isSelected ? "Selected" : "Normal";
    VisualStateManager.GoToState(CardBorder, state);
    VisualStateManager.GoToState(TitleLabel, state);
    VisualStateManager.GoToState(CharaIdLabel, state);
  }

  // カードがタップされたとき
  private void OnCardTapped(object? sender, EventArgs e)
  {
    // すでに遅延処理がある場合 → ダブルクリック
    System.Diagnostics.Debug.WriteLine($"TAP instance: {this.GetHashCode()}");

    IsSelected = !IsSelected;
    if (_cts != null)
    {
      System.Diagnostics.Debug.WriteLine("DoubleTap: Cancel called");
      _cts.Cancel();
      _cts.Dispose();
      _cts = null;

      CardDoubleClicked?.Invoke(this, new CharaThumnailCardEventArgs
      {
        CharaId = CharaId,
        Title = Title,
        IsSelected = IsSelected,
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
        CardClicked?.Invoke(this, new CharaThumnailCardEventArgs
        {
          CharaId = CharaId,
          Title = Title,
          IsSelected = IsSelected,
        });
      });

      _cts?.Dispose();
      _cts = null;

    }, token);
  }

  // draw
  private void OnPaintCharaImageSurface(object sender, SKPaintSurfaceEventArgs e)
  {
    if (Bitmap == null) return;

    var canvas = e.Surface.Canvas;
    canvas.Clear(SKColors.White);

    var info = e.Info;
    var rect = new SKRect(0, 0, info.Width, info.Height);
    canvas.DrawBitmap(Bitmap, rect);
    System.Diagnostics.Debug.WriteLine($"OnPaintCharaImageSurface {info.Width}x{info.Height} - {CanvasWidth}x{CanvasHeight}");
  }
}

public class CharaThumnailCardEventArgs : EventArgs
{
  public string CharaId { get; set; } = string.Empty;
  public string Title { get; set; } = string.Empty;
  public bool IsSelected { get; set; } = false;
  // public string MaterialName { get; set; } = string.Empty;
}