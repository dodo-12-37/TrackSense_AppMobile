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

            this.bluetoothLE.Adapter.ScanTimeout = 5000;
            this.bluetoothLE.Adapter.ScanMode = ScanMode.Balanced;

            var scanFilterOptions = new ScanFilterOptions();

            await bluetoothLE.Adapter.StartScanningForDevicesAsync();

            foreach (IDevice device in this.bluetoothDevices)
            {
                TrackSenseDevice bleDevice = new TrackSenseDevice()
                {
                    Name = device.Name,
                    State = device.State == DeviceState.Connected ? "Connecté" : "Deconnecté",
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
