using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackSense.Entities;

namespace TrackSense.Services.API.APIDTO
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
