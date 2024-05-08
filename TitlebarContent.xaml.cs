namespace TitlebarSample;

public partial class TitlebarContent : ContentView
{
	public TitlebarContent(TitlebarViewModel titlebarViewModel)
	{
		InitializeComponent();
		this.BindingContext = titlebarViewModel;
	}
}