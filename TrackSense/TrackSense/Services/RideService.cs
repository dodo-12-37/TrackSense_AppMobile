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


        public RideService(RideData rideData)
        {
            _rideData = rideData;

            BluetoothObserver bluetoothObserver = new BluetoothObserver(this._bluetoothService,
                (value) =>
                {
                    if (value.Type != BluetoothEventType.SENDING_RIDE_DATA)
                    {
                        isConnected = value.Type == BluetoothEventType.CONNECTION;
                    }
                    else
                    {
                        _rideService.ReceiveRideData(value.rideData);
                    }
                });
        }

        internal void ReceiveRideData(string rideData)
        {
            //Enregistrer le rideData dans RideData
            Console.WriteLine(rideData);
            //Si connexion à internet, envoyer vers API
        }
    }
}
