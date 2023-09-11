using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackSense.Entities
{
    public class CompletedRideSummary
    {
        public Guid CompletedRideId { get; set; }
        public string PlannedRideName { get; set; }
        public DateTime StartedAt { get; set; }
        public TimeSpan Duration { get; set; }
        public int Distance { get; set; }
    }
}
