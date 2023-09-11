using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackSense.Models
{
    public class CompletedRidePoint
    {
        public int RideStep { get; set; }
        public Location Location { get; set; }
        public double Temperature { get; set; }
        public TimeSpan EffectiveTime { get; set; }

        public CompletedRidePoint(int rideStep, Location location, double temperature, TimeSpan effectiveTime)
        {
            RideStep = rideStep;
            Location = location;
            Temperature = temperature;
            EffectiveTime = effectiveTime;
        }
        public CompletedRidePoint()
        {
            ;
        }

        public CompletedRidePoint(TrackSense.Entities.CompletedRidePoint entite)
        {
            RideStep = entite.RideStep;
            Location = entite.Location;
            Temperature = entite.Temperature;
            EffectiveTime = entite.EffectiveTime;
        }


        public CompletedRidePoint ToEntity()
        {
            return new CompletedRidePoint(RideStep, Location, Temperature, EffectiveTime);
        }
    }
}
