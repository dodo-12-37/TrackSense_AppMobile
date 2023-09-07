using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackSense.Entities
{
    public interface ICompletedRideLocalData
    {
        void AddCompletedRide(CompletedRide completedRide);
        List<CompletedRide> ListCompletedRides();
        CompletedRide GetCompletedRideById(Guid completedRideId);
        void DeleteCompletedRideById(Guid completedRideId);
        void DeleteAllCompletedRides();
    }
}
