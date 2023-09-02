using Microsoft.Extensions.Logging;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE;
using TrackSense.ViewModels;
using TrackSense.Views;
using TrackSense.Services;

namespace TrackSense
{
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

#if DEBUG
		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<IBluetoothLE>(CrossBluetoothLE.Current);
            builder.Services.AddSingleton<IAdapter>(CrossBluetoothLE.Current.Adapter);

            builder.Services.AddSingleton<BluetoothService>();

            builder.Services.AddSingleton<TrackSenseDevicesViewModel>();
            builder.Services.AddSingleton<MainPageViewModel>();

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<TrackSenseDevicesPage>();
            return builder.Build();
        }
    }
}