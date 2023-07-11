using BluetoothLEConnection.Model;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothLEConnection.Services
{
    public class BluetoothDeviceService
    {
        IAdapter adapter;
        List<IDevice> bluetoothDevices = new();


        public BluetoothDeviceService(IAdapter p_adapter)
        {
            this.adapter = p_adapter;
        }

        public async Task<List<Bluetooth>> GetBluetoothDevices()
        {
            if (this.bluetoothDevices.Count == 0)
            {
                this.adapter.DeviceDiscovered += (s, a) =>
                {
                    if (!this.bluetoothDevices.Contains(a.Device))
                    {
                        bluetoothDevices.Add(a.Device);
                    }
                };
            }

            bluetoothDevices.Clear();

            List<Bluetooth> devicesList = new List<Bluetooth>();

            this.adapter.ScanTimeout = 5000;
            this.adapter.ScanMode = ScanMode.Balanced;

            var scanFilterOptions = new ScanFilterOptions();

            await adapter.StartScanningForDevicesAsync();

            foreach (IDevice device in this.bluetoothDevices)
            {
                Bluetooth bleDevice = new Bluetooth()
                {
                    Name = device.Name,
                    State = device.State == Plugin.BLE.Abstractions.DeviceState.Connected ? "Connecté" : "Deconnecté",
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
                await this.adapter.ConnectToDeviceAsync(device);
            }
            catch (DeviceConnectionException e)
            {
                throw e;
            }
        }

        public IDevice GetConnectedDevice()
        {
            //var device = adapter.GetSystemConnectedOrPairedDevices().FirstOrDefault();
            return adapter.ConnectedDevices.SingleOrDefault();
        }
    }
}
