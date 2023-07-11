using BluetoothLEConnection.Services;
using BluetoothLEConnection.ViewModel;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;

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
        builder.Services.AddSingleton<BluetoothDeviceService>();
		builder.Services.AddSingleton<BluetoothDeviceViewModel>();
		builder.Services.AddSingleton<MainPage>();

		return builder.Build();
	}
}
