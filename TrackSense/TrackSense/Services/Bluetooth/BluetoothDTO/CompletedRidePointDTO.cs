using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackSense.Entities;

namespace TrackSense.Services.Bluetooth.BluetoothDTO
{
    public class CompletedRidePointDTO
    {
        public Location Location { get; set; }
        public int RideStep { get; set; }
        public double Temperature { get; set; }
        public DateTime DateTime { get; set; }

        public CompletedRidePoint ToEntity()
        {
            return new CompletedRidePoint(Location, RideStep, Temperature, DateTime);
        }
    }
}
