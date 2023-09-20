using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackSense.Models
{
    internal class CompletedRideModel
    {
        [DefaultValue("admin")]
        public string UserLogin { get; set; } = null!;

        [DefaultValue("b0f07b65-3055-4f99-bc09-91829ca16fdb")]
        public string CompletedRideId { get; set; } = null!;
        public PlannedRideModel PlannedRideModel { get; set; } = null;
        public List<CompletedRidePointModel> CompletedRidePointsModel { get; set; } = new List<CompletedRidePointModel>();

        public CompletedRideStatisticsModel Statistics { get; set; } = null;

        public CompletedRideModel()
        {
            ;
        }
    }
}
