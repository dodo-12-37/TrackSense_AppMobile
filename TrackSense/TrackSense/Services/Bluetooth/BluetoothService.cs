using Microsoft.Maui.Controls;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using System.Diagnostics;
using System.Reflection;
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
        CompletedRideDTO _completedRideDTO;
        bool isBusy = false;
        ICharacteristic _notificationCharacteristic;
        ICharacteristic _dataCharacteristic;

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

            bluetoothLE.Adapter.ScanTimeout = 1000;
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
                IDevice device = bluetoothDevices.SingleOrDefault(d => d.Id == p_id);
                if (device is not null)
                {
                    await bluetoothLE.Adapter.ConnectToDeviceAsync(device);

                    SetNotifications(p_id);
                    BluetoothEvent BTEventConnection = new BluetoothEvent(BluetoothEventType.CONNECTION, true);
                    observers.ForEach(o => o.OnNext(BTEventConnection));
                }
            }
            catch (DeviceConnectionException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async void SetNotifications(Guid p_id)
        {

            IDevice connectedDevice = bluetoothLE.Adapter.ConnectedDevices.SingleOrDefault();
            Guid completedRideServiceUID = new Guid("62ffab64-3646-4fb9-88d8-541deb961192");
            //Guid isStatsReadyCharacUID = new Guid("9456444a-4b5f-11ee-be56-0242ac120002");
            Guid rideStatsUID = new Guid("51656aa8-b795-427f-a96c-c4b6c57430dd");
            Guid rideNotificationUID = new Guid("61656aa8-b795-427f-a96c-c4b6c57430dd");
            //Guid pointNumberDataUID = new Guid("c5799499-9053-4a9e-a2d5-b8814c5ff12b");
            //Guid pointDataUID = new Guid("42154deb-5828-4876-8d4f-eaec38fa1ea7");

            IService completedRideService = await connectedDevice.GetServiceAsync(completedRideServiceUID);

            //ICharacteristic isStatsReadyCharac = await completedRideService.GetCharacteristicAsync(isStatsReadyCharacUID);
            //ICharacteristic rideStatsCharac = await completedRideService.GetCharacteristicAsync(rideStatsUID);
            //ICharacteristic rideNotificationCharac = await completedRideService.GetCharacteristicAsync(rideNotificationUID);
            _dataCharacteristic = await completedRideService.GetCharacteristicAsync(rideStatsUID);
            _notificationCharacteristic = await completedRideService.GetCharacteristicAsync(rideNotificationUID);
            //ICharacteristic pointNumberCharac = await completedRideService.GetCharacteristicAsync(pointNumberDataUID);
            //ICharacteristic pointDataCharac = await completedRideService.GetCharacteristicAsync(pointDataUID);


            _notificationCharacteristic.ValueUpdated += async (sender, args) =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        byte[] status = args.Characteristic.Value;
                        string statusString = Encoding.UTF8.GetString(status);
                        if (statusString == "sending" && !isBusy)
                        {
                            isBusy = true;
                            byte[] rideBytes = await _dataCharacteristic.ReadAsync();
                            string rideMessage = Encoding.UTF8.GetString(rideBytes);

                            if (_completedRideDTO is null)
                            {
                                CompletedRideDTO completedRideDTO = new CompletedRideDTO(rideMessage);

                                BluetoothEvent BTEventSendData = new BluetoothEvent(BluetoothEventType.SENDING_RIDE_STATS, true, completedRideDTO.ToEntity());
                                observers.ForEach(o => o.OnNext(BTEventSendData));
                                this._completedRideDTO = completedRideDTO;
                                Debug.WriteLine("Ride ajouté : " + completedRideDTO.CompletedRideId);
                            }
                            else
                            {
                                CompletedRidePointDTO pointDTO = new CompletedRidePointDTO(rideMessage);
                                Entities.CompletedRidePoint completedRidePoint = pointDTO.ToEntity();

                                BluetoothEvent BTEventSendData = new BluetoothEvent(BluetoothEventType.SENDING_RIDE_POINT, true, completedRidePoint);
                                observers.ForEach(o => o.OnNext(BTEventSendData));
                                Debug.WriteLine("Point ajouté : " + pointDTO.RideStep);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Erreur callback : " + e.Message);
                    }
                    finally
                    {
                        isBusy = false;
                    }
                });
            };
            await _notificationCharacteristic.StartUpdatesAsync();
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
            //IDevice connectedDevice = this.GetConnectedDevice();
            Guid completedRideServiceUID = new Guid("62ffab64-3646-4fb9-88d8-541deb961192");
            //Guid characteristicIsReadyUID = new Guid("9456444a-4b5f-11ee-be56-0242ac120002");
            Guid rideNotificationUID = new Guid("61656aa8-b795-427f-a96c-c4b6c57430dd");
            byte[] confirmationString = Encoding.UTF8.GetBytes("ok");

            //IService completedRideService = await connectedDevice.GetServiceAsync(completedRideServiceUID);
            try
            {
                //ICharacteristic notificationCharacteristic = await completedRideService.GetCharacteristicAsync(rideNotificationUID);
                //await notificationCharacteristic.WriteAsync(confirmationString);
                Debug.WriteLine("Confirmation réception point #");
                bool result = await _notificationCharacteristic.WriteAsync(confirmationString);
                return result;

            }
            //catch (TargetInvocationException ex)
            //{
            //    Exception innerException = ex.InnerException;
            //    Debug.WriteLine("Erreur : " + innerException.Message);
            //    //throw ex;
            //}
            catch (Exception e)
            {
                Debug.WriteLine("Erreur ConfirmRideStatsReception : " + e.Message);
                throw e;
            }
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
