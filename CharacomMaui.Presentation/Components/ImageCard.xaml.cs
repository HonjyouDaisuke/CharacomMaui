namespace CharacomMaui.Presentation.Components;

public partial class ImageCard : ContentView
{
    public ImageCard()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(nameof(Name), typeof(string), typeof(ImageCard), string.Empty);

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public static readonly BindableProperty IdProperty =
        BindableProperty.Create(nameof(Id), typeof(string), typeof(ImageCard), string.Empty);

    public string Id
    {
        get => (string)GetValue(IdProperty);
        set => SetValue(IdProperty, value);
    }

    public static readonly BindableProperty TypeProperty =
        BindableProperty.Create(nameof(Type), typeof(string), typeof(ImageCard), string.Empty);

    public string Type
    {
        get => (string)GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }

    public static readonly BindableProperty ImageProperty =
        BindableProperty.Create(nameof(Image), typeof(ImageSource), typeof(ImageCard), default(ImageSource));

    public ImageSource Image
    {
        get => (ImageSource)GetValue(ImageProperty);
        set => SetValue(ImageProperty, value);
    }

}