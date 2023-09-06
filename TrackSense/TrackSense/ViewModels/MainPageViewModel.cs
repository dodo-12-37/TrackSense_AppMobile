using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrackSense.Models;
using TrackSense.Services;
using TrackSense.Views;

namespace TrackSense.ViewModels;

public partial class MainPageViewModel : BaseViewModel
{
    BluetoothService _bluetoothService;
    RideService _rideService;

    [ObservableProperty]
    bool isConnected;

    [ObservableProperty]
    bool isReceivingData;

    public MainPageViewModel(BluetoothService btService, RideService rideService)
    {
        Title = "Accueil";
        _bluetoothService = btService;
        _rideService = rideService;

        BluetoothObserver bluetoothObserver = new BluetoothObserver(this._bluetoothService,
            (value) =>
            {
                switch (value.Type)
                {
                    case BluetoothEventType.CONNECTION:
                        isConnected = true;
                        break;
                    case BluetoothEventType.DECONNECTION:
                        isConnected = false;
                        break;
                    case BluetoothEventType.SENDING_RIDE_DATA:
                        isReceivingData = true;
                        this._rideService.ReceiveRideData(value.rideData);
                        isReceivingData = false;
                        break;
                    default:
                        break;
                }
            });
    }

    [RelayCommand]
    async Task GoToTrackSenseDevices()
    {
        if (!IsConnected)
        {
            await Shell.Current.GoToAsync(nameof(TrackSenseDevicesPage));
        }
    }
}
