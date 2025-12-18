using System.Collections.Specialized;
using System.ComponentModel;
using CharacomMaui.Presentation.Models;
using MauiApp = Microsoft.Maui.Controls.Application;

namespace CharacomMaui.Presentation.Components;

public partial class SelectBar : ContentView
{
  /// <summary>
  /// Initializes a new instance of SelectBar and loads its visual components.
  /// </summary>
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

  /// <summary>
  /// Handle changes to the Items bindable property by updating collection-change subscriptions and rebuilding the displayed items.
  /// </summary>
  /// <param name="bindable">The SelectBar whose Items property changed.</param>
  /// <param name="oldValue">The previous Items value.</param>
  /// <param name="newValue">The new Items value; if it implements <see cref="INotifyCollectionChanged"/>, its CollectionChanged will be observed.</param>
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
  /// <summary>
  /// Invoked when the ThemeKey bindable property changes and updates the control's background to reflect the new theme key.
  /// </summary>
  /// <param name="bindable">The SelectBar instance whose ThemeKey changed.</param>
  /// <param name="oldVal">The previous ThemeKey value.</param>
  /// <param name="newVal">The new ThemeKey value.</param>
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

  /// <summary>
  /// Rebuilds the control's item visuals in response to changes in the Items collection.
  /// </summary>
  /// <param name="sender">The source of the collection change event.</param>
  /// <param name="e">Details about what changed in the collection.</param>
  private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
  {
    BuildItems();
  }

  /// <summary>
  /// Applies the control's initial selection when the InitialSelectedLabel property changes.
  /// </summary>
  /// <param name="bindable">The SelectBar instance whose InitialSelectedLabel changed.</param>
  /// <param name="oldValue">The previous label value (may be null or empty).</param>
  /// <param name="newValue">The new label value (may be null or empty).</param>
  private static void OnInitialSelectedLabelChanged(BindableObject bindable, object oldValue, object newValue)
  {
    ((SelectBar)bindable).ApplyInitialSelection();
  }
  private readonly Dictionary<SelectBarContents, PropertyChangedEventHandler> _itemHandlers = [];
  /// <summary>
  /// Rebuilds the visual item elements from the current Items collection and wires their runtime behavior.
  /// </summary>
  /// <remarks>
  /// Clears any previously attached per-item PropertyChanged handlers and UI children, creates a bordered label element for each item in Items, attaches a tap handler and a PropertyChanged handler to keep visual state and enabled state in sync, and then applies the configured initial selection.
  /// </remarks>
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

  /// <summary>
  /// Selects the item whose Name equals <see cref="InitialSelectedLabel"/> and is not disabled.
  /// </summary>
  /// <remarks>Does nothing when <see cref="InitialSelectedLabel"/> is null or empty, or when no matching enabled item is found.</remarks>
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

  /// <summary>
  /// Sets the visual state of the provided Border and Label according to the item's disabled or selected state, giving disabled precedence over selected.
  /// </summary>
  /// <param name="border">The Border whose visual state will be updated.</param>
  /// <param name="label">The Label whose visual state will be updated.</param>
  /// <param name="item">The data item whose IsDisabled and IsSelected flags determine the visual state.</param>
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
  /// <summary>
  /// Selects the item represented by the provided Border's BindingContext and raises the ItemSelected event.
  /// </summary>
  /// <param name="selectedBorder">A Border whose BindingContext is the SelectBarContents to select; if the BindingContext is not a SelectBarContents the method does nothing.</param>
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
  /// <summary>
  /// Cleans up per-item PropertyChanged subscriptions when the control's handler is being removed.
  /// </summary>
  /// <param name="args">Event data for the handler changing event.</param>
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