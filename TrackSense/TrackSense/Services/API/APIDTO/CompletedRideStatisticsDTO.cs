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
        public double MaximumSpeed { get; set; }
        public double AverageSpeed { get; set; }
        public double Distance { get; set; }
        public TimeSpan Duration { get; set; }
        public int NumberOfPoints { get; set; }
        public int Calories { get; set; }
        public int Falls { get; set; }

        public CompletedRideStatisticsDTO()
        {
            ;
        }

        public CompletedRideStatisticsDTO(CompletedRideStatistics entite)
        {
            MaximumSpeed = entite.MaximumSpeed;
            AverageSpeed = entite.AverageSpeed;
            Distance = entite.Distance;
            Duration = entite.Duration;
            NumberOfPoints = entite.NumberOfPoints;
            Calories = entite.Calories;
            Falls = entite.Falls;
        }

        internal CompletedRideStatistics ToEntity()
        {
            return new CompletedRideStatistics()
            {
                MaximumSpeed = this.MaximumSpeed,
                AverageSpeed = this.AverageSpeed,
                Distance = this.Distance,
                Duration = this.Duration,
                NumberOfPoints = this.NumberOfPoints,
                Calories = this.Calories,
                Falls = this.Falls
            };
        }
    }
}
