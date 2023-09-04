using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using System.Text;
using TrackSense.Models;

namespace TrackSense.Services
{
    public class BluetoothService : IObservable<BluetoothEvent>
    {
        IBluetoothLE bluetoothLE;
        List<IDevice> bluetoothDevices = new();
        private List<IObserver<BluetoothEvent>> observers = new();

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
                    var bytes = args.Characteristic.Value;
                    string message = Encoding.UTF8.GetString(bytes);
                    BluetoothEvent BTEventSendData = new BluetoothEvent(BluetoothEventType.SENDING_RIDE_DATA, true, message);
                    observers.ForEach(o => o.OnNext(BTEventSendData));
                    // Action à effectuer à chaque notification
                    //   - Notifier observateur (CompletedRidesViewModel)
                    //     - Vérifier intégrité trajet
                    //     - Enregistrer trajet dans receivedData
                    Console.WriteLine(message);
                };

                await characteristicTest.StartUpdatesAsync();
                BluetoothEvent BTEventConnection = new BluetoothEvent(BluetoothEventType.CONNECTION, true);
                this.observers.ForEach(o => o.OnNext(BTEventConnection));

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

        public IDisposable Subscribe(IObserver<BluetoothEvent> observer)
        {
            if (observer is null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            this.observers.Add(observer);
            return new UnsubscriberBluetooth(this.observers, observer);
        }
    }
}
