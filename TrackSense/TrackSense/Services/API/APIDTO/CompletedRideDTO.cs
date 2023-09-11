using TrackSense.Entities;

namespace TrackSense.Services.API.APIDTO;

public class CompletedRideDTO
{
    public Guid CompletedRideId { get; set; }
    public Guid PlannedRideId { get; set; }
    public List<CompletedRidePointDTO> CompletedRidePoints { get; set; }
    public CompletedRideStatisticsDTO Statistics { get; set; }

    public CompletedRideDTO()
    {
        ;
    }

    public CompletedRideDTO(CompletedRide entite)
    {
        if (entite is null)
        {
            throw new ArgumentNullException(nameof(entite));
        }

        CompletedRideId = entite.CompletedRideId;
        PlannedRideId = entite.PlannedRideId;
        CompletedRidePoints = entite.CompletedRidePoints.Select(entite => new CompletedRidePointDTO(entite)).ToList();
        Statistics = new CompletedRideStatisticsDTO(entite.Statistics);
    }

    public CompletedRide ToEntity()
    {
        List<CompletedRidePoint> listPoints = CompletedRidePoints.Select(pointDTO
            => pointDTO.ToEntity()).ToList();

        return new CompletedRide()
        {
            CompletedRideId = CompletedRideId,
            PlannedRideId = PlannedRideId,
            CompletedRidePoints = listPoints,
            Statistics = Statistics.ToEntity()
        };
    }

}
