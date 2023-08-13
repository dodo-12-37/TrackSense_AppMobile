using BluetoothLEConnection.Services;
using BluetoothLEConnection.ViewModel;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Microsoft.Maui.Networking;

namespace BluetoothLEConnection;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton<IBluetoothLE>(CrossBluetoothLE.Current);
        builder.Services.AddSingleton<IAdapter>(CrossBluetoothLE.Current.Adapter);
		builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
		builder.Services.AddSingleton<UserService>();
		//builder.Services.AddSingleton<UserViewModel>();
        builder.Services.AddSingleton<BluetoothDeviceService>();
		builder.Services.AddSingleton<BluetoothDeviceViewModel>();
		builder.Services.AddSingleton<MainPage>();

		return builder.Build();
	}
}
