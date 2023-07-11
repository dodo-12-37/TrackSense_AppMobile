using BluetoothLEConnection.ViewModel;

namespace BluetoothLEConnection;

public partial class MainPage : ContentPage
{
	public MainPage(BluetoothDeviceViewModel p_viewModel)
	{
		InitializeComponent();
		BindingContext = p_viewModel;
    }
}
