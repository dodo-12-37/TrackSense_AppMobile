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

    public MainPageViewModel(BluetoothService btService, RideService rideService)
    {
        Title = "Accueil";
        _bluetoothService = btService;
        _rideService = rideService;

        //BluetoothObserver bluetoothObserver = new BluetoothObserver(this._bluetoothService,
        //    (value) => IsConnected = value.Type != BluetoothEventType.DECONNECTION);

        BluetoothObserver bluetoothObserver = new BluetoothObserver(this._bluetoothService,
            (value) =>
            {
                if (value.Type != BluetoothEventType.SENDING_RIDE_DATA)
                {
                    isConnected = value.Type == BluetoothEventType.CONNECTION;
                }
                else
                {
                    _rideService.ReceiveRideData(value.rideData);
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
