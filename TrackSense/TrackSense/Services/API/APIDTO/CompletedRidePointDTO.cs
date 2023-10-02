using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackSense.Entities;

namespace TrackSense.Services.API.APIDTO
{
    public class CompletedRidePointDTO
    {
        public string CompletedRideId { get; set; }=string.Empty!;
        public LocationDTO Location { get; set; } = null!;
        public int RideStep { get; set; } =0;
        public double Temperature { get; set; } = 0;
        public DateTime DateTime { get; set; }
        public CompletedRidePointDTO(CompletedRidePoint entite)
        {
            RideStep = entite.RideStep;
            Location = new LocationDTO( entite.Location);
            Temperature = entite.Temperature;
            DateTime = entite.Location.Timestamp.DateTime;
        }

        public CompletedRidePointDTO()
        {
            ;
        }

        public CompletedRidePoint ToEntity()
        {
            return new CompletedRidePoint()
            {
                Location = this.Location.ToEntity(),
                RideStep= this.RideStep,
                Temperature = this.Temperature,
            };
        }
    }
}
