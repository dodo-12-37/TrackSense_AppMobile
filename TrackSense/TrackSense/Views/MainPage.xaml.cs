using TrackSense.ViewModels;

namespace TrackSense.Views
{
    public partial class MainPage : ContentPage
    {

        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is MainPageViewModel mainPageViewModel)
            {
                await mainPageViewModel.GetCompletedRidesAsync();
            }
        }

    }
}