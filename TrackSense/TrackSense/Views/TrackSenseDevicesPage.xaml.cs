using TrackSense.ViewModels;

namespace TrackSense.Views;

public partial class TrackSenseDevicesPage : ContentPage
{
	public TrackSenseDevicesPage(TrackSenseDevicesViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}