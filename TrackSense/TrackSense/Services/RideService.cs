using CommunityToolkit.Mvvm.ComponentModel;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackSense.Data;

namespace TrackSense.Services
{
    public class RideService
    {
        private RideData _rideData;
        BluetoothService _bluetoothService;

        public RideService(RideData rideData, BluetoothService bluetoothService)
        {
            _rideData = rideData;
            _bluetoothService = bluetoothService;

            BluetoothObserver bluetoothObserver = new BluetoothObserver(this._bluetoothService,
                async (value) =>
                {
                    if (value.Type == BluetoothEventType.SENDING_RIDE_DATA)
                    {
                        this.ReceiveRideData(value.rideData);
                        //IDevice connectedDevice = this._bluetoothService.GetConnectedDevice();
                        //Guid completedRideServiceUID = new Guid("62ffab64-3646-4fb9-88d8-541deb961192");
                        //IService completedRideService = await connectedDevice.GetServiceAsync(completedRideServiceUID);
                        //Guid characteristicIsReceivedUID = new Guid("08382f5e-4b60-11ee-be56-0242ac120002");
                        //ICharacteristic characteristicIsReceived = await completedRideService.GetCharacteristicAsync(characteristicIsReceivedUID);
                        //byte[] dataFalse = { 0x00 };
                        //await characteristicIsReceived.WriteAsync(dataFalse);
                    }
                });
        }

        internal void ReceiveRideData(string rideData)
        {
            //Enregistrer le rideData dans RideData
            Console.WriteLine("Receiving ride data...");

            Console.WriteLine(rideData);
            //Si connexion à internet, envoyer vers API
        }
    }
}
