using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackSense.Entities;

namespace TrackSense.Services.API.APIDTO
{
    public class CompletedRideSummaryDTO
    {
        public string CompletedRideId { get; set; }
        public string PlannedRideName { get; set; }
        public DateTime StartedAt { get; set; }
        public TimeSpan Duration { get; set; }
        public int Distance { get; set; }

        public CompletedRideSummaryDTO()
        {
            ;
        }


        public CompletedRideSummary ToEntity()
        {
            return new CompletedRideSummary()
            {
                CompletedRideId = new Guid(this.CompletedRideId),
                PlannedRideName = this.PlannedRideName,
                StartedAt = this.StartedAt,
                Duration = this.Duration,
                Distance = this.Distance
            };  
        }
    }
}
