using BluetoothLEConnection.ViewModel;

namespace BluetoothLEConnection;

public partial class MainPage : ContentPage
{
	public MainPage(BluetoothViewModel p_viewModel)
	{
		InitializeComponent();
		BindingContext = p_viewModel;
    }
}
