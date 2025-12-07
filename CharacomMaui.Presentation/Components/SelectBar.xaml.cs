using System.Collections.Specialized;
using CharacomMaui.Presentation.Helpers;
using CharacomMaui.Presentation.Models;
using MauiApp = Microsoft.Maui.Controls.Application;

namespace CharacomMaui.Presentation.Components;

public partial class SelectBar : ContentView
{
  public SelectBar()
  {
    InitializeComponent();
  }

  private Button? _selectedButton;
  private INotifyCollectionChanged? _observableItems;

  // ========= ラベルリスト =========
  public static readonly BindableProperty ItemsProperty =
      BindableProperty.Create(
          nameof(Items),
          typeof(IEnumerable<SelectBarContents>),
          typeof(SelectBar),
          null,
          propertyChanged: OnItemsChanged);

  public IEnumerable<SelectBarContents> Items
  {
    get => (IEnumerable<SelectBarContents>)GetValue(ItemsProperty);
    set => SetValue(ItemsProperty, value);
  }

  // ========= テーマ =========
  public static readonly BindableProperty ThemeKeyProperty =
    BindableProperty.Create(

        nameof(ThemeKey),
        typeof(string),
        typeof(SelectBar),
        default(string),
        propertyChanged: OnThemeKeyChanged);

  public string ThemeKey
  {
    get => (string)GetValue(ThemeKeyProperty);
    set => SetValue(ThemeKeyProperty, value);
  }
  // ========= 初期選択ラベル =========
  public static readonly BindableProperty InitialSelectedLabelProperty =
      BindableProperty.Create(
          nameof(InitialSelectedLabel),
          typeof(string),
          typeof(SelectBar),
          null,
          propertyChanged: OnInitialSelectedLabelChanged);

  public string InitialSelectedLabel
  {
    get => (string)GetValue(InitialSelectedLabelProperty);
    set => SetValue(InitialSelectedLabelProperty, value);
  }

  // ========= 押されたときのイベント =========
  public event EventHandler<SelectBarEventArgs>? ItemSelected;

  private static void OnItemsChanged(BindableObject bindable, object oldValue, object newValue)
  {
    var control = (SelectBar)bindable;

    // 古い ObservableCollection の監視を解除
    if (control._observableItems != null)
    {
      control._observableItems.CollectionChanged -= control.OnItemsCollectionChanged;
      control._observableItems = null;
    }

    if (newValue is INotifyCollectionChanged observable)
    {
      control._observableItems = observable;
      observable.CollectionChanged += control.OnItemsCollectionChanged;
    }

    control.BuildButtons();
  }
  private static void OnThemeKeyChanged(BindableObject bindable, object oldVal, object newVal)
  {
    var control = (SelectBar)bindable;
    control.UpdateBackground();
  }

  private void UpdateBackground()
  {
    if (string.IsNullOrEmpty(ThemeKey)) return;

    // Light/Dark 両方取得（存在すれば）
    var app = MauiApp.Current;

    var lightKey = ThemeKey;
    var darkKey = $"{ThemeKey}Dark";

    if (app!.Resources.TryGetValue(lightKey, out var light)
        && app.Resources.TryGetValue(darkKey, out var dark))
    {
      BackgroundBox.SetAppThemeColor(BoxView.ColorProperty,
          (Color)light,
          (Color)dark);
    }
  }

  private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
  {
    BuildButtons();
  }

  private static void OnInitialSelectedLabelChanged(BindableObject bindable, object oldValue, object newValue)
  {
    var control = (SelectBar)bindable;
    control.ApplyInitialSelection();
  }

  private void BuildButtons()
  {
    ButtonContainer.Children.Clear();

    if (Items == null || !Items.Any())
      return;

    var outline = ThemeHelper.GetColor("Outline");
    var disableBackground = ThemeHelper.GetColor("DisabledBackground");
    var disableText = ThemeHelper.GetColor("DisabledText");
    foreach (var item in Items)
    {
      //item.Title = string.IsNullOrEmpty(item.Title) ? $"{item.Name}({item.Count})" : item.Title;
      //item.IsDisabled = item.Count <= 0;
      System.Diagnostics.Debug.WriteLine($"SelectBar Button: {item.Title} Disabled:{item.IsDisabled}");
      var btn = new Button
      {
        Text = item.Title,
        BackgroundColor = Colors.Transparent, // 非選択色
        TextColor = item.IsDisabled ? disableText : outline,
        BorderColor = item.IsDisabled ? disableBackground : outline,
        IsEnabled = !item.IsDisabled,
        BorderWidth = 1,
        WidthRequest = 100,
        CornerRadius = 8,
        Padding = new Thickness(16, 10),
        Margin = new Thickness(6),
        BindingContext = item,
      };
      btn.Clicked += OnButtonClicked;
      ButtonContainer.Children.Add(btn);
    }

    ApplyInitialSelection();
  }

  private void ApplyInitialSelection()
  {
    if (string.IsNullOrEmpty(InitialSelectedLabel)) return;

    var btn = ButtonContainer.Children
        .OfType<Button>()
        .FirstOrDefault(b =>
            b.BindingContext is SelectBarContents data &&
            data.Name == InitialSelectedLabel);

    if (btn != null)
      SelectButton(btn);
  }

  private void OnButtonClicked(object? sender, EventArgs e)
  {
    if (sender is not Button clickedButton) return;
    SelectButton(clickedButton);
  }

  private void SelectButton(Button clickedButton)
  {
    // 非選択色に戻す
    if (_selectedButton != null)
    {
      var outline = ThemeHelper.GetColor("Outline");
      _selectedButton.BackgroundColor = Colors.Transparent;
      _selectedButton.TextColor = outline;
      _selectedButton.BorderColor = outline;
      _selectedButton.BorderWidth = 1;
    }

    // 選択色
    clickedButton.BackgroundColor = ThemeHelper.GetColor("Primary");
    clickedButton.TextColor = ThemeHelper.GetColor("OnPrimary");
    clickedButton.BorderWidth = 0;

    _selectedButton = clickedButton;
    // Bindingされているデータを取得
    var data = clickedButton.BindingContext as SelectBarContents;
    if (data == null) return;
    // イベント発火
    System.Diagnostics.Debug.WriteLine($"Selected: {data.Name} title = {clickedButton.Text}");
    ItemSelected?.Invoke(this, new SelectBarEventArgs
    {
      SelectedName = data.Name,
    });

  }
}
public class SelectBarEventArgs : EventArgs
{
  public string SelectedName { get; set; } = string.Empty;
}
