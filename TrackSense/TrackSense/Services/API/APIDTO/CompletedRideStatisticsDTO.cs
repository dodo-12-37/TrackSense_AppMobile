//using Javax.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackSense.Entities;

namespace TrackSense.Services.API.APIDTO
{
    public class CompletedRideStatisticsDTO
    {
        public string CompletedRideId { get; set; }
        public double AverageSpeed { get; set; }
        public double MaximumSpeed { get; set; }
        public int Falls { get; set; }
        public int Calories { get; set; }
        public double Distance { get; set; }
        public TimeSpan Duration { get; set; }

        public CompletedRideStatisticsDTO()
        {
            ;
        }

        public CompletedRideStatisticsDTO(CompletedRideStatistics p_completedRideStatistics)
        {
            this.AverageSpeed = p_completedRideStatistics.AverageSpeed;
            this.MaximumSpeed = p_completedRideStatistics.MaximumSpeed;
            this.Falls = p_completedRideStatistics.Falls;
            this.Calories = p_completedRideStatistics.Calories;
            this.Distance = p_completedRideStatistics.Distance;
            this.Duration = p_completedRideStatistics.Duration;
        }

        internal CompletedRideStatistics ToEntity()
        {
            return new CompletedRideStatistics()
            {
                MaximumSpeed = this.MaximumSpeed,
                AverageSpeed = this.AverageSpeed,
                Distance = this.Distance,
                Duration = this.Duration,
                Calories = this.Calories,
                Falls = this.Falls
            };
        }
    }
}
