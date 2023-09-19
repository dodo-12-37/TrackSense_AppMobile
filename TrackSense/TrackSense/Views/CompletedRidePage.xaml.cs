using TrackSense.ViewModels;

namespace TrackSense.Views;

public partial class CompletedRidePage : ContentPage
{
	public CompletedRidePage(CompletedRideViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}