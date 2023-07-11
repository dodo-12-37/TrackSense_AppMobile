using BluetoothLEConnection.Model;
using BluetoothLEConnection.Services;
using BluetoothLEConnection.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothLEConnection.ViewModel
{
    public partial class BluetoothViewModel: BaseViewModel
    {
        IBluetoothLE bluetoothLE;

        BluetoothService bluetoothService;

        [ObservableProperty]
        bool isRefreshing;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsConnected))]
        Bluetooth connectedDevice;

        public bool IsConnected => this.ConnectedDevice is not null;

        public ObservableCollection<Bluetooth> Devices { get; } = new();


        public BluetoothViewModel(BluetoothService p_bluetoothService, IBluetoothLE p_bluetoothLE)
        {
            this.Title = "Bluetooth";
            this.bluetoothService = p_bluetoothService;
            this.bluetoothLE = p_bluetoothLE;
        }

        [RelayCommand]
        async Task BluetoothConnectivityTestAsync()
        {
            await this.RequestBluetooth();

            if (IsBusy)
            {
                return;
            }

            try
            {
                BluetoothState state = this.bluetoothLE.State;

                if (state is BluetoothState.Off)
                {
                    await Shell.Current.DisplayAlert("Erreur connexion bluetooth", "Veuillez vérifier votre connexion bluetooth", "Ok");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Connexion bluetooth active", "Le bluetooth du téléphone est bien activé", "Ok");
                }

                IsBusy = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!", $"Message: {ex.Message}", "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task RequestBluetooth()
        {
            var status = PermissionStatus.Unknown;

            status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status == PermissionStatus.Granted)
            {
                return;
            }

            if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
            {
                await Shell.Current.DisplayAlert("Needs permissions", "Needed for scan", "Ok");
            }

            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        [RelayCommand]
        async Task ScanForBluetoothDevicesAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                await RequestBluetooth();

                BluetoothState state = this.bluetoothLE.State;

                if (state is BluetoothState.Off)
                {
                    await Shell.Current.DisplayAlert("Erreur connexion bluetooth", "Veuillez vérifier votre connexion bluetooth", "Ok");
                    return;
                }

                List<Bluetooth> deviceList = await this.bluetoothService.GetBluetoothDevices();

                if (this.Devices.Count > 0)
                {
                    this.Devices.Clear();
                }

                foreach (Bluetooth device in deviceList)
                {
                    this.Devices.Add(device);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!", $"Message: {ex.Message}", "Ok");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        async Task ConnectToDeviceAsync(Guid p_id)
        {
            if (IsBusy)
            {
                return;
            }
            
            try
            {
                IsBusy = true;
                await bluetoothService.ConnectToBluetoothDevice(p_id);
                IDevice device = bluetoothService.GetConnectedDevice();

                if (device is not null)
                {
                    this.ConnectedDevice = this.Devices.SingleOrDefault(d => d.Id == device.Id);
                    if (this.ConnectedDevice is not null)
                    {
                        this.ConnectedDevice.State = "Connecté";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!", $"Message: {ex.Message}", "Ok");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        async Task DisplayDeviceMessageAsync()
        {
            IDevice connectedDevice = bluetoothService.GetConnectedDevice();
            Guid serviceTestUID = new Guid("4fafc201-1fb5-459e-8fcc-c5c9c331914b");
            Guid characteristicTestUID = new Guid("beb5483e-36e1-4688-b7f5-ea07361b26a8");

            IService serviceTest = await connectedDevice.GetServiceAsync(serviceTestUID);
            ICharacteristic characteristicTest = await serviceTest.GetCharacteristicAsync(characteristicTestUID);

            byte[] messageByte = await characteristicTest.ReadAsync();

            string message =Encoding.UTF8.GetString(messageByte);

            await Shell.Current.DisplayAlert("Message", $"Affichage du message app embarquee : {message}", "Ok");
        }

        [RelayCommand]
        async Task SendMessageToDeviceAsync()
        {
            IDevice connectedDevice = bluetoothService.GetConnectedDevice();
            Guid serviceTestUID = new Guid("4fafc201-1fb5-459e-8fcc-c5c9c331914b");
            Guid characteristicTestUID = new Guid("beb5483e-36e1-4688-b7f5-ea07361b26a8");

            IService serviceTest = await connectedDevice.GetServiceAsync(serviceTestUID);
            ICharacteristic characteristicTest = await serviceTest.GetCharacteristicAsync(characteristicTestUID);

            string messageToSend = "TrackSense mobile app !";
            byte[] messageToByte = Encoding.UTF8.GetBytes(messageToSend);

            var result = await characteristicTest.WriteAsync(messageToByte);

            string resultMessage = result ? "Envoi réussi" : "Problème avec l'envoi";

            await Shell.Current.DisplayAlert("Résultat", resultMessage, "OK");
        }
    }
}
