using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using System.Diagnostics;
using System.Text;
using TrackSense.Entities;
using TrackSense.Models;
using TrackSense.Services.Bluetooth.BluetoothDTO;

namespace TrackSense.Services.Bluetooth
{
    public class BluetoothService : IObservable<BluetoothEvent>
    {
        IBluetoothLE bluetoothLE;
        List<IDevice> bluetoothDevices = new();
        private List<IObserver<BluetoothEvent>> observers = new();
        public bool IsConnected
        {
            get
            {
                return GetBluetoothDevices() is not null;
            }
        }

        public BluetoothService(IBluetoothLE bluetoothLE)
        {
            this.bluetoothLE = bluetoothLE;
        }

        public async Task<List<TrackSenseDevice>> GetBluetoothDevices()
        {
            if (bluetoothDevices.Count == 0)
            {
                bluetoothLE.Adapter.DeviceDiscovered += (s, a) =>
                {
                    if (!bluetoothDevices.Contains(a.Device))
                    {
                        bluetoothDevices.Add(a.Device);
                    }
                };
            }

            bluetoothDevices.Clear();

            List<TrackSenseDevice> devicesList = new List<TrackSenseDevice>();

            bluetoothLE.Adapter.ScanTimeout = 3000;
            bluetoothLE.Adapter.ScanMode = ScanMode.Balanced;

            await bluetoothLE.Adapter.StartScanningForDevicesAsync();

            foreach (IDevice device in bluetoothDevices)
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
            IDevice device = bluetoothDevices.SingleOrDefault(d => d.Id == p_id);

            if (device is null)
            {
                return;
            }

            try
            {
                await bluetoothLE.Adapter.ConnectToDeviceAsync(device);
                IDevice connectedDevice = bluetoothLE.Adapter.ConnectedDevices.SingleOrDefault();
                Guid completedRideServiceUID = new Guid("62ffab64-3646-4fb9-88d8-541deb961192");
                Guid isReadyCharacUID = new Guid("9456444a-4b5f-11ee-be56-0242ac120002");
                Guid rideStatsUID = new Guid("51656aa8-b795-427f-a96c-c4b6c57430dd");
                Guid pointNumberDataUID = new Guid("c5799499-9053-4a9e-a2d5-b8814c5ff12b");
                Guid pointDataUID = new Guid("42154deb-5828-4876-8d4f-eaec38fa1ea7");

                IService completedRideService = await connectedDevice.GetServiceAsync(completedRideServiceUID);

                ICharacteristic isReadyCharac = await completedRideService.GetCharacteristicAsync(isReadyCharacUID);
                ICharacteristic rideStatsCharac = await completedRideService.GetCharacteristicAsync(rideStatsUID);
                ICharacteristic pointNumberCharac = await completedRideService.GetCharacteristicAsync(pointNumberDataUID);
                ICharacteristic pointDataCharac = await completedRideService.GetCharacteristicAsync(pointDataUID);


                isReadyCharac.ValueUpdated += async (sender, args) =>
                {
                    var bytes = args.Characteristic.Value;
                    byte[] rideBytes = await rideStatsCharac.ReadAsync();
                    string rideMessage = Encoding.UTF8.GetString(rideBytes);
                    CompletedRideDTO completedRideDTO = new CompletedRideDTO(rideMessage);

                    BluetoothEvent BTEventSendData = new BluetoothEvent(BluetoothEventType.SENDING_RIDE_STATS, true, completedRideDTO.ToEntity());
                    observers.ForEach(o => o.OnNext(BTEventSendData));
                };
                await isReadyCharac.StartUpdatesAsync();

                pointNumberCharac.ValueUpdated += async (sender, args) =>
                {
                    byte[] pointBytes = await pointDataCharac.ReadAsync();
                    string ridePoint = Encoding.UTF8.GetString(pointBytes);
                    List<CompletedRidePointDTO> pointsDTOList = new List<CompletedRidePointDTO>();
                    List<CompletedRidePoint> pointsList = pointsDTOList.Select(p => p.ToEntity()).ToList();

                    BluetoothEvent BTEventSendData = new BluetoothEvent(BluetoothEventType.SENDING_RIDE_STATS, true, pointsList);
                    observers.ForEach(o => o.OnNext(BTEventSendData));
                };
                await pointNumberCharac.StartUpdatesAsync();

                BluetoothEvent BTEventConnection = new BluetoothEvent(BluetoothEventType.CONNECTION, true);
                observers.ForEach(o => o.OnNext(BTEventConnection));
            }
            catch (DeviceConnectionException e)
            {
                throw e;
            }
        }

        public IDevice GetConnectedDevice()
        {
            return bluetoothLE.Adapter.ConnectedDevices.SingleOrDefault();
        }

        public bool BluetoothIsOn()
        {
            return bluetoothLE.State == BluetoothState.On;
        }

        public IDisposable Subscribe(IObserver<BluetoothEvent> observer)
        {
            if (observer is null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            observers.Add(observer);
            return new UnsubscriberBluetooth(observers, observer);
        }

        #region DEBUG

        public void SimulateRideReception()
        {
            Guid randomGuid = Guid.NewGuid();
            string rideMessage = $"{randomGuid};N;20.12;30;200;90;2023-09-03T14:30:00;2023-09-03T16:30:00;1;0";
            CompletedRideDTO completedRideDTO = new CompletedRideDTO(rideMessage);

            BluetoothEvent BTEventSendData = new BluetoothEvent(BluetoothEventType.SENDING_RIDE_STATS, true, completedRideDTO.ToEntity());
            observers.ForEach(o => o.OnNext(BTEventSendData));
        }

        public void SimulatePointsReception()
        {
            Shell.Current.DisplayAlert("Heu...", "Ce bouton n'est pas encore prêt", "Ok");
            //string ridePoint = "";
            //List<CompletedRidePointDTO> pointsDTOList = new List<CompletedRidePointDTO>();
            //List<CompletedRidePoint> pointsList = pointsDTOList.Select(p => p.ToEntity()).ToList();

            //BluetoothEvent BTEventSendData = new BluetoothEvent(BluetoothEventType.SENDING_RIDE_STATS, true, pointsList);
            //observers.ForEach(o => o.OnNext(BTEventSendData));
        }

        #endregion
    }
}
