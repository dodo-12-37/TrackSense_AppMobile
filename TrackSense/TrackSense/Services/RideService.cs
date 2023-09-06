using CommunityToolkit.Mvvm.ComponentModel;
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
                (value) =>
                {
                    if (value.Type == BluetoothEventType.SENDING_RIDE_DATA)
                    {
                        this.ReceiveRideData(value.rideData);
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
