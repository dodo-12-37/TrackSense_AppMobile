using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using TrackSense.Models;

namespace TrackSense.Services
{
    public class BluetoothService
    {
        IBluetoothLE bluetoothLE;
        List<IDevice> bluetoothDevices = new();

        public BluetoothService(IBluetoothLE bluetoothLE)
        {
            this.bluetoothLE = bluetoothLE;
        }

        public async Task<List<TrackSenseDevice>> GetBluetoothDevices()
        {
            if (this.bluetoothDevices.Count == 0)
            {
                this.bluetoothLE.Adapter.DeviceDiscovered += (s, a) =>
                {
                    if (!this.bluetoothDevices.Contains(a.Device))
                    {
                        bluetoothDevices.Add(a.Device);
                    }
                };
            }

            bluetoothDevices.Clear();

            List<TrackSenseDevice> devicesList = new List<TrackSenseDevice>();

            this.bluetoothLE.Adapter.ScanTimeout = 3000;
            this.bluetoothLE.Adapter.ScanMode = ScanMode.Balanced;

            await bluetoothLE.Adapter.StartScanningForDevicesAsync();

            foreach (IDevice device in this.bluetoothDevices)
            {
                TrackSenseDevice bleDevice = new TrackSenseDevice()
                {
                    Name = device.Name,
                    isConnected = device.State == DeviceState.Connected,
                    Id = device.Id
                };
                devicesList.Add(bleDevice);
            }

            return devicesList;
        }

        public async Task ConnectToBluetoothDevice(Guid p_id)
        {
            IDevice device = this.bluetoothDevices.SingleOrDefault(d => d.Id == p_id);

            if (device is null)
            {
                return;
            }

            try
            {
                await this.bluetoothLE.Adapter.ConnectToDeviceAsync(device);
                IDevice connectedDevice = this.bluetoothLE.Adapter.ConnectedDevices.SingleOrDefault();
                Guid serviceTestUID = new Guid("4fafc201-1fb5-459e-8fcc-c5c9c331914b");
                Guid characteristicTestUID = new Guid("beb5483e-36e1-4688-b7f5-ea07361b26a8");

                IService serviceTest = await connectedDevice.GetServiceAsync(serviceTestUID);
                ICharacteristic characteristicTest = await serviceTest.GetCharacteristicAsync(characteristicTestUID);

                characteristicTest.ValueUpdated += (sender, args) =>
                {
                    var bytes = args.Characteristic.Value.ToString();
                    string message = BitConverter.ToString(bytes);
                    // Action à effectuer à chaque notification (envoyer le trajet vers l'API) - voir pattern Strategy
                    Console.WriteLine(message);
                };

                await characteristicTest.StartUpdatesAsync();

            }
            catch (DeviceConnectionException e)
            {
                throw e;
            }
        }

        public IDevice GetConnectedDevice()
        {
            //var device = adapter.GetSystemConnectedOrPairedDevices().FirstOrDefault();
            return bluetoothLE.Adapter.ConnectedDevices.SingleOrDefault();
        }

        public bool BluetoothIsOn()
        {
            return this.bluetoothLE.State == BluetoothState.On;
        }
    }
}
