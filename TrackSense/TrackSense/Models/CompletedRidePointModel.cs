using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackSense.Models
{
    internal class CompletedRidePointModel
    {
        [DefaultValue("b0f07b65-3055-4f99-bc09-91829ca16fdb")]
        public string CompletedRideId { get; set; } = null!;
        public LocationModel Location { get; set; } = new LocationModel();
        public int? RideStep { get; set; }
        public double? Temperature { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;

        public CompletedRidePointModel()
        {
            ;
        }

    }
}
