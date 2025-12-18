using System.Collections.Specialized;
using System.ComponentModel;
using CharacomMaui.Presentation.Models;
using MauiApp = Microsoft.Maui.Controls.Application;

namespace CharacomMaui.Presentation.Components;

public partial class SelectBar : ContentView
{
  public SelectBar()
  {
    InitializeComponent();
  }

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

    control.BuildItems();
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
    BuildItems();
  }

  private static void OnInitialSelectedLabelChanged(BindableObject bindable, object oldValue, object newValue)
  {
    ((SelectBar)bindable).ApplyInitialSelection();
  }
  private readonly Dictionary<SelectBarContents, PropertyChangedEventHandler> _itemHandlers = [];
  private void BuildItems()
  {
    // 既存のハンドラを解除
    foreach (var kvp in _itemHandlers)
    {
      kvp.Key.PropertyChanged -= kvp.Value;
    }
    _itemHandlers.Clear();
    ItemContainer.Children.Clear();

    if (Items == null || !Items.Any())
      return;

    foreach (var item in Items)
    {
      var label = new Label
      {
        Text = item.Title,
        Style = (Style)Resources["SelectBarLabelStyle"],
      };
      var border = new Border
      {
        Style = (Style)Resources["SelectBarItemStyle"],
        Content = label,
        BindingContext = item,
        IsEnabled = !item.IsDisabled,
      };
      var tap = new TapGestureRecognizer();
      tap.Tapped += (_, _) => SelectItem(border);
      border.GestureRecognizers.Add(tap);

      PropertyChangedEventHandler handler = (_, e) =>
      {
        if (e.PropertyName == nameof(item.IsSelected) ||
        e.PropertyName == nameof(item.IsDisabled))
        {
          border.IsEnabled = !item.IsDisabled;
          UpdateVisualState(border, label, item);
        }

      };

      item.PropertyChanged += handler;
      _itemHandlers[item] = handler;

      UpdateVisualState(border, label, item);
      ItemContainer.Children.Add(border);
    }

    ApplyInitialSelection();
  }

  private void ApplyInitialSelection()
  {
    if (string.IsNullOrEmpty(InitialSelectedLabel)) return;

    var border = ItemContainer.Children
        .OfType<Border>()
        .FirstOrDefault(b =>
            b.BindingContext is SelectBarContents data &&
            data.Name == InitialSelectedLabel && !data.IsDisabled);

    if (border != null)
      SelectItem(border);
  }

  private void UpdateVisualState(Border border, Label label, SelectBarContents item)
  {
    if (item.IsDisabled)
    {
      VisualStateManager.GoToState(border, "Disabled");
      VisualStateManager.GoToState(label, "Disabled");
    }
    else if (item.IsSelected)
    {
      VisualStateManager.GoToState(border, "Selected");
      VisualStateManager.GoToState(label, "Selected");
    }
    else
    {
      VisualStateManager.GoToState(border, "Normal");
      VisualStateManager.GoToState(label, "Normal");
    }
  }
  private void SelectItem(Border selectedBorder)
  {
    if (selectedBorder.BindingContext is not SelectBarContents data)
      return;

    if (Items == null) return;
    foreach (var item in Items)
    {
      if (item.IsDisabled) continue;
      item.IsSelected = item.Name == data.Name;
    }

    ItemSelected?.Invoke(this, new SelectBarEventArgs
    {
      SelectedName = data.Name
    });
  }
  protected override void OnHandlerChanging(HandlerChangingEventArgs args)
  {
    base.OnHandlerChanging(args);
    if (args.NewHandler == null)
    {
      // コントロールが破棄される際のクリーンアップ
      foreach (var kvp in _itemHandlers)
      {
        kvp.Key.PropertyChanged -= kvp.Value;
      }
      _itemHandlers.Clear();
    }
  }
}
public class SelectBarEventArgs : EventArgs
{
  public string SelectedName { get; set; } = string.Empty;
}
