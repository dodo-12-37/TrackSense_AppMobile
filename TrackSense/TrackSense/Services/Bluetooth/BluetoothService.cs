using Microsoft.Maui.Controls;
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
            try
            {
                SetNotifications(p_id);
                BluetoothEvent BTEventConnection = new BluetoothEvent(BluetoothEventType.CONNECTION, true);
                observers.ForEach(o => o.OnNext(BTEventConnection));
            }
            catch (DeviceConnectionException e)
            {
                throw e;
            }
        }

        private async void SetNotifications(Guid p_id)
        {
            IDevice device = bluetoothDevices.SingleOrDefault(d => d.Id == p_id);

            if (device is null)
            {
                return;
            }

            await bluetoothLE.Adapter.ConnectToDeviceAsync(device);
            IDevice connectedDevice = bluetoothLE.Adapter.ConnectedDevices.SingleOrDefault();
            Guid completedRideServiceUID = new Guid("62ffab64-3646-4fb9-88d8-541deb961192");
            Guid isStatsReadyCharacUID = new Guid("9456444a-4b5f-11ee-be56-0242ac120002");
            Guid rideStatsUID = new Guid("51656aa8-b795-427f-a96c-c4b6c57430dd");
            Guid pointNumberDataUID = new Guid("c5799499-9053-4a9e-a2d5-b8814c5ff12b");
            Guid pointDataUID = new Guid("42154deb-5828-4876-8d4f-eaec38fa1ea7");

            IService completedRideService = await connectedDevice.GetServiceAsync(completedRideServiceUID);

            ICharacteristic isStatsReadyCharac = await completedRideService.GetCharacteristicAsync(isStatsReadyCharacUID);
            ICharacteristic rideStatsCharac = await completedRideService.GetCharacteristicAsync(rideStatsUID);
            ICharacteristic pointNumberCharac = await completedRideService.GetCharacteristicAsync(pointNumberDataUID);
            ICharacteristic pointDataCharac = await completedRideService.GetCharacteristicAsync(pointDataUID);


            isStatsReadyCharac.ValueUpdated += async (sender, args) =>
            {
                byte[] rideBytes = await rideStatsCharac.ReadAsync();
                string rideMessage = Encoding.UTF8.GetString(rideBytes);
                CompletedRideDTO completedRideDTO = new CompletedRideDTO(rideMessage);

                BluetoothEvent BTEventSendData = new BluetoothEvent(BluetoothEventType.SENDING_RIDE_STATS, true, completedRideDTO.ToEntity());
                observers.ForEach(o => o.OnNext(BTEventSendData));
            };
            await isStatsReadyCharac.StartUpdatesAsync();

            pointNumberCharac.ValueUpdated += async (sender, args) =>
            {
                byte[] pointBytes = await pointDataCharac.ReadAsync();
                string ridePointMessage = Encoding.UTF8.GetString(pointBytes);
                CompletedRidePointDTO pointDTO = new CompletedRidePointDTO(ridePointMessage);
                CompletedRidePoint completedRidePoint = pointDTO.ToEntity();

                BluetoothEvent BTEventSendData = new BluetoothEvent(BluetoothEventType.SENDING_RIDE_STATS, true, completedRidePoint);
                observers.ForEach(o => o.OnNext(BTEventSendData));
            };
            await pointNumberCharac.StartUpdatesAsync();
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
        internal async Task<bool> ConfirmRideStatsReception()
        {
            IDevice connectedDevice = this.GetConnectedDevice();
            Guid completedRideServiceUID = new Guid("62ffab64-3646-4fb9-88d8-541deb961192");
            Guid characteristicIsReadyUID = new Guid("9456444a-4b5f-11ee-be56-0242ac120002");

            IService completedRideService = await connectedDevice.GetServiceAsync(completedRideServiceUID);
            ICharacteristic characteristicIsReceived = await completedRideService.GetCharacteristicAsync(characteristicIsReadyUID);
            byte[] dataFalse = Encoding.UTF8.GetBytes("false");
            return await characteristicIsReceived.WriteAsync(dataFalse);
        }

        internal async Task<bool> ConfirmPointReception()
        {
            IDevice connectedDevice = this.GetConnectedDevice();
            Guid completedRideServiceUID = new Guid("62ffab64-3646-4fb9-88d8-541deb961192");
            Guid completedRidePointUID = new Guid("42154deb-5828-4876-8d4f-eaec38fa1ea7");

            IService completedRideService = await connectedDevice.GetServiceAsync(completedRideServiceUID);
            ICharacteristic completedRidePointCharac = await completedRideService.GetCharacteristicAsync(completedRidePointUID);
            byte[] dataFalse = Encoding.UTF8.GetBytes("ok");

            return await completedRidePointCharac.WriteAsync(dataFalse);
        }

        #region DEBUG

        public void SimulateRideReception()
        {
            Guid randomGuid = Guid.NewGuid();
            Guid plannedRideID = new Guid("00000000-0000-0000-0000-000000000000");
            string rideMessage = $"{randomGuid};{plannedRideID};30.12;18.65;2023-09-03T14:30:00;2023-09-03T16:30:00;00:45:00;32;2;0";
            CompletedRideDTO completedRideDTO = new CompletedRideDTO(rideMessage);

            BluetoothEvent BTEventSendData = new BluetoothEvent(BluetoothEventType.SENDING_RIDE_STATS, true, completedRideDTO.ToEntity());
            observers.ForEach(o => o.OnNext(BTEventSendData));
        }

        public void SimulatePointsReception()
        {
            Shell.Current.DisplayAlert("Réception de points", "Les points vont arriver", "Ok");
            string ridePoint1 = "1;71.8393483920;81.293029273720;71;21.23;19.28;2023-09-03T14:33:00;00:03:00";
            string ridePoint2 = "2;72.8393483920;82.293029273720;72;21.22;19.22;2023-09-03T14:35:00;00:05:00";

            CompletedRidePointDTO pointDTO1 = new CompletedRidePointDTO(ridePoint1);
            BluetoothEvent BTEventSendData = new BluetoothEvent(BluetoothEventType.SENDING_RIDE_POINT, true, pointDTO1.ToEntity());
            observers.ForEach(o => o.OnNext(BTEventSendData));

            CompletedRidePointDTO pointDTO2 = new CompletedRidePointDTO(ridePoint2);
            BluetoothEvent BTEventSendData2 = new BluetoothEvent(BluetoothEventType.SENDING_RIDE_POINT, true, pointDTO2.ToEntity());
            observers.ForEach(o => o.OnNext(BTEventSendData2));
        }

        #endregion
    }
}
