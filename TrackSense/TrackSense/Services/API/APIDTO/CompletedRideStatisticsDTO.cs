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
        public string CompletedRideId { get; set; } = string.Empty;

        public double AvgSpeed { get; set; } = 0;

        public double MaxSpeed { get; set; } = 0;

        public int Falls { get; set; } = 0;

        public int Calories { get; set; } = 0;

        public double Distance { get; set; } = 0;

        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(1);

        public CompletedRideStatisticsDTO()
        {
            ;
        }

        public CompletedRideStatisticsDTO(CompletedRideStatistics p_completedRideStatistics)
        {
            this.AvgSpeed = p_completedRideStatistics.AverageSpeed;
            this.MaxSpeed = p_completedRideStatistics.MaximumSpeed;
            this. Falls = p_completedRideStatistics.Falls;
            this.Calories = p_completedRideStatistics.Calories;
            this.Distance = p_completedRideStatistics.Distance;
            this.Duration = p_completedRideStatistics.Duration;
        }

        internal CompletedRideStatistics ToEntity()
        {
            return new CompletedRideStatistics()
            {
                MaximumSpeed = this.MaxSpeed,
                AverageSpeed = this.AvgSpeed,
                Distance = this.Distance,
                Duration = this.Duration,
                Calories = this.Calories,
                Falls = this.Falls
            };
        }
    }
}
