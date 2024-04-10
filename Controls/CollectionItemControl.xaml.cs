namespace CollectionManagmentSystem.Controls;

public partial class CollectionItemControl : ContentView
{
	public static readonly BindableProperty ProductIdProperty = BindableProperty.Create(nameof(ProductId), typeof(int), typeof(CollectionItemControl), null);
	public static readonly BindableProperty NameProperty = BindableProperty.Create(nameof(Name), typeof(string), typeof(CollectionItemControl), string.Empty);
	public static readonly BindableProperty PriceProperty = BindableProperty.Create(nameof(Price), typeof(double), typeof(CollectionItemControl), null);
	public static readonly BindableProperty RatingProperty = BindableProperty.Create(nameof(Rating), typeof(int), typeof(CollectionItemControl), null);
	public static readonly BindableProperty StatusProperty = BindableProperty.Create(nameof(Status), typeof(string), typeof(CollectionItemControl), string.Empty);
    public int ProductId
	{
		get => (int)GetValue(ProductIdProperty);
		set => SetValue(ProductIdProperty, value);
	}

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public double Price
    {
        get => (double)GetValue(PriceProperty);
        set => SetValue(PriceProperty, value);
    }

	public int Rating
	{
		get => (int)GetValue(RatingProperty);
		set => SetValue(RatingProperty, value);
	}

	public string Status
	{
		get => (string)GetValue(StatusProperty);
		set => SetValue(StatusProperty, value);
	}

    public CollectionItemControl()
	{
		InitializeComponent();
	}

    public event EventHandler EditItemClicked;

    private void EditItem_Clicked(object sender, EventArgs e)
	{
		EditItemClicked?.Invoke(this, EventArgs.Empty);
	}
}