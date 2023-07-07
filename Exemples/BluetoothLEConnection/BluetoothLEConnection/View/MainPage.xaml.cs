using BluetoothLEConnection.ViewModel;

namespace BluetoothLEConnection;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage(BluetoothViewModel p_viewModel)
	{
		InitializeComponent();
		BindingContext = p_viewModel;
    }
}
