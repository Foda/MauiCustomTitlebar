namespace TitlebarSample
{
    public partial class MainPage : ContentPage
    {
        public MainPage(TitlebarViewModel titlebarViewModel)
        {
            InitializeComponent();

            this.BindingContext = titlebarViewModel;
        }
    }
}
