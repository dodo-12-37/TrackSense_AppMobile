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
        public string CompletedRideId { get; set; }
        public string LocationId { get; set; }
        public int RideStep { get; set; }
        public LocationDTO Location { get; set; }
        public double Temperature { get; set; }
        public DateTime Date { get; set; }
        public CompletedRidePointDTO(CompletedRidePoint entite)
        {
            CompletedRideId = string.Empty;
            RideStep = entite.RideStep;
            Location = new LocationDTO( entite.Location);
            Date = entite.Location.Timestamp.DateTime;
            Temperature = entite.Temperature;
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
