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

        public int RideStep { get; set; }
        public Location Location { get; set; }
        public double Temperature { get; set; }
        public TimeSpan EffectiveTime { get; set; }

        public CompletedRidePointDTO(CompletedRidePoint entite)
        {
            RideStep = entite.RideStep;
            Location = entite.Location;
            Temperature = entite.Temperature;
            EffectiveTime = entite.EffectiveTime;
        }

        public CompletedRidePointDTO(string csvPoint)
        {
            csvPoint = csvPoint.Replace('.', ',');
            string[] point = csvPoint.Split(';');

            RideStep = int.Parse(point[0]);
            Location = new Location(double.Parse(point[1]), double.Parse(point[2]), double.Parse(point[3]));
            Temperature = double.Parse(point[4]);
            Location.Speed = double.Parse(point[5]);
            Location.Timestamp = DateTimeOffset.Parse(point[6]);
            EffectiveTime = TimeSpan.FromSeconds(double.Parse(point[7]));
        }

        public CompletedRidePointDTO()
        {
            ;
        }

        public CompletedRidePoint ToEntity()
        {
            return new CompletedRidePoint(RideStep, Location, Temperature, EffectiveTime);
        }
    }
}
