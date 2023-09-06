using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackSense.Entities
{
    public class CompletedRide
    {
        public Guid CompletedRideId { get; set; }
        public Guid? PlannedRideId { get; set; }
        public List<CompletedRidePoint>? CompletedRidePoints { get; set; }
        public CompletedRideStatistics Statistics { get; set; }
    }
}
