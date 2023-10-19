using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackSense.Services.API.APIDTO
{
    public class AvailabilityDTO
    {
        public bool IsAvailable { get; set; }
        public AvailabilityDTO()
        {
            ;
        }
        public AvailabilityDTO(Entities.Availability entity)
        {
            this.IsAvailable = entity.IsAvailable;
        }
    }
}
