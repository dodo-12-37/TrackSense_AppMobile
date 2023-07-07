using BluetoothLEConnection.Model;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothLEConnection.Services
{
    public class BluetoothService
    {
        IAdapter adapter;
        List<Bluetooth> bluetoothDevices = new();

        public BluetoothService(IAdapter p_adapter)
        {
            this.adapter = p_adapter;
        }

        public async Task<List<Bluetooth>> GetBluetoothDevices()
        {
            bluetoothDevices.Clear();
            List<IDevice> devicesList = new List<IDevice>();

            this.adapter.ScanTimeout = 5000;
            this.adapter.ScanMode = ScanMode.Balanced;
            
            this.adapter.DeviceDiscovered += (s, a) => devicesList.Add(a.Device);
            await adapter.StartScanningForDevicesAsync();

            foreach (IDevice device in devicesList)
            {
                Bluetooth bleDevice = new Bluetooth()
                {
                    Name = device.Name,
                    State = device.State == Plugin.BLE.Abstractions.DeviceState.Connected ? "Connecté" : "Deconnecté"
                };
                bluetoothDevices.Add(bleDevice);
            }

            return bluetoothDevices;
        }
    }
}
