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

        public CompletedRidePointDTO(CompletedRidePoint entite)
        {
            Location = entite.Location;
            RideStep = entite.RideStep;
            Temperature = entite.Temperature;
            DateTime = entite.DateTime;
        }

        public CompletedRidePointDTO()
        {
            ;
        }

        public CompletedRidePoint ToEntity()
        {
            return new CompletedRidePoint(Location, RideStep, Temperature, DateTime);
        }
    }
}
