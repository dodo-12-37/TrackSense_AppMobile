using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackSense.Models
{
    internal class CompletedRideStatisticsModel
    {
        public string CompletedRideId { get; set; }
        public double? AverageSpeed { get; set; } = 0;
        public double? MaximumSpeed { get; set; } = 0;
        public double? Distance { get; set; } = 0;
        public int? Calories { get; set; } = 0;
        public int? Falls { get; set; } = 0;
        public TimeSpan Duration { get; set; }

        public CompletedRideStatisticsModel()
        {

        }

        public CompletedRideStatisticsModel(CompletedRideStatistics p_statistics)
        {
            this.CompletedRideId = p_statistics.CompletedRideId;
            this.AverageSpeed = p_statistics.AverageSpeed;
            this.MaximumSpeed = p_statistics.MaximumSpeed;
            this.Distance = p_statistics.Distance;
            this.Calories = p_statistics.Calories;
            this.Duration = p_statistics.Duration;
            this.Falls = p_statistics.Falls;
        }

        public CompletedRideStatistics ToEntity()
        {
            return new CompletedRideStatistics
            {
                CompletedRideId = this.CompletedRideId,
                AverageSpeed = this.AverageSpeed,
                MaximumSpeed = this.MaximumSpeed,
                Distance = this.Distance,
                Calories = this.Calories,
                Duration = this.Duration,
                Falls = this.Falls
            };
        }
    }
}
