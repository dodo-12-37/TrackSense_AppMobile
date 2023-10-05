using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackSense.Entities;
using TrackSense.Services;
using TrackSense.Services.Bluetooth;

namespace TrackSense.ViewModels
{
    public partial class SettingsViewModel : BaseViewModel
    {
        RideService _rideService;
        BluetoothService _bluetoothService;

        public SettingsViewModel(RideService rideService, BluetoothService bluetoothService)
        {
            Title = "Paramètres";
            _rideService = rideService;
            _bluetoothService = bluetoothService;
        }

        [RelayCommand]
        async Task DeleteRidesFromStorageAsync()
        {
            List<CompletedRide> allRides = _rideService.GetAllCompletedRides();
            int totalNumberOfRides = allRides.Count;

            bool choice = await Shell.Current.DisplayAlert("Supprimer les trajets", $"Cette action supprimera {totalNumberOfRides} trajet(s) enregistré(s) dans la mémoire du téléphone. Êtes-vous sûr ?", "Oui", "Non");
            if (choice)
            {
                _rideService.DeleteRidesFromLocalStorage();
            }
        }

        [RelayCommand]
        async Task DisconnectFromDeviceAsync()
        {
            this._bluetoothService.DisconnectDevice();
        }
    }
}
