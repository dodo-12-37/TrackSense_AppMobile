using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackSense.Entities;

namespace TrackSense.Services.API.APIDTO
{
    public class LocationDTO
    {
        public int LocationId { get; set; } = 0;
        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; } = 0;
        public double? Altitude { get; set; } = 0;
        public double? Speed { get; set; } = 0;

        public LocationDTO() 
        {
            Latitude = 0;
        }
        public LocationDTO(Location p_location)
        {
            this.LocationId = 0;
            this.Latitude = p_location.Latitude;
            this.Longitude = p_location.Longitude;
            this.Altitude = p_location.Altitude;
            this.Speed = p_location.Speed;
        }

        public Location ToEntity()
        {
            return new Location()
            {  
                Latitude = this.Latitude,
                Longitude = this.Longitude,
                Altitude = this.Altitude,
                Speed = this.Speed
            };

        }
    }
}
