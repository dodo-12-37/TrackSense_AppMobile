using Microsoft.Extensions.Logging;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE;
using TrackSense.ViewModels;
using TrackSense.Views;
using TrackSense.Services;
using TrackSense.Data;
using TrackSense.Entities;
using TrackSense.Services.Bluetooth;
using CommunityToolkit.Maui.Maps;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace TrackSense
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream("TrackSense.appsettings.json");

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            var builder = MauiApp.CreateBuilder();
            //builder.Configuration.AddConfiguration(config);
            var settings = config.GetRequiredSection("Settings").Get<Settings>();
            string APIKey = settings.BingApiKey;
            builder
                .UseMauiApp<App>()
                .UseMauiMaps()
                .UseMauiCommunityToolkitMaps(APIKey)
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

            builder.Services.AddSingleton<ICompletedRideLocalData, RideData>();

            builder.Services.AddSingleton<BluetoothService>();
            builder.Services.AddSingleton<RideService>();

            builder.Services.AddSingleton<TrackSenseDevicesViewModel>();
            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<CompletedRideViewModel>();

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<TrackSenseDevicesPage>();
            builder.Services.AddTransient<CompletedRidePage>();
            return builder.Build();
        }
    }
}