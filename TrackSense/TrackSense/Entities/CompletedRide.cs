using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackSense.Models;

namespace TrackSense.Entities
{
    public class CompletedRide
    {
        //[DefaultValue("b0f07b65-3055-4f99-bc09-91829ca16fdb")]
        public string UserLogin { get; set; } = null!;
        public Guid CompletedRideId { get; set; }
        public Guid PlannedRideId { get; set; }
        public List<CompletedRidePoint> CompletedRidePoints { get; set; }

        public List<CompletedRidePoint> CompletedRidePoints { get; set; } = new List<CompletedRidePointModel>();
        public CompletedRideStatistics Statistics { get; set; }

        public CompletedRideStatistics Statistics { get; set; } = null;
    }
}
