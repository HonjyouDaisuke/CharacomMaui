using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace CharacomMaui.Presentation.Components;

public partial class CharaImageCard : ContentView
{
  public CharaImageCard()
  {
    InitializeComponent();
  }

  // bindableProperty
  // ========== Title ==========
  public static readonly BindableProperty TitleProperty =
      BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(CharaImageCard),
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
          typeof(CharaImageCard),
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
          typeof(CharaImageCard),
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
          typeof(CharaImageCard),
          null,
          propertyChanged: OnBitmapChanged);

  public SKBitmap Bitmap
  {
    get => (SKBitmap)GetValue(BitmapProperty);
    set => SetValue(BitmapProperty, value);
  }

  private static void OnBitmapChanged(BindableObject bindable, object oldValue, object newValue)
  {
    var control = (CharaImageCard)bindable;
    control.CharaImage.InvalidateSurface();
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