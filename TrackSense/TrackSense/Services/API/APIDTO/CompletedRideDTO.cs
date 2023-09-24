using TrackSense.Entities;

namespace TrackSense.Services.API.APIDTO;

public class CompletedRideDTO
{
    public string CompletedRideId { get; set; }
    public string UserLogin { get; set; }
    public string PlannedRideId { get; set; }
    public virtual IEnumerable<CompletedRidePointDTO> CompletedRidePoints { get; set; }
    public virtual CompletedRideStatisticsDTO CompletedRideStatistic { get; set; }
    public CompletedRideDTO()
    {
        ;
    }

    public CompletedRideDTO(Entities.CompletedRide p_completedRide)
    {
        if (p_completedRide == null)
        {
            throw new NullReferenceException(nameof(p_completedRide));
        }
        if (p_completedRide.UserLogin == null)
        {
            throw new NullReferenceException(nameof(p_completedRide.UserLogin));
        }
        if (p_completedRide.CompletedRideId == Guid.Empty)
        {
            throw new InvalidOperationException("Id du CompletedRide ne doit pas être null ni vide");
        }

        this.UserLogin = p_completedRide.UserLogin;
        this.CompletedRideId = p_completedRide.CompletedRideId.ToString();
        if (p_completedRide.PlannedRideId != Guid.Empty)
        {

            this.PlannedRideId = p_completedRide.PlannedRideId.ToString();
        }

        this.CompletedRidePoints = p_completedRide.CompletedRidePoints.Select(entite => new CompletedRidePointDTO(entite) 
                                                                                            { CompletedRideId = this.CompletedRideId});

        this.CompletedRideStatistic = new CompletedRideStatisticsDTO(p_completedRide.Statistics);
        this.CompletedRideStatistic.CompletedRideId = this.CompletedRideId;

    }

    public Entities.CompletedRide ToEntity()
    {
        return new CompletedRide()
        {
            CompletedRideId = new Guid(this.CompletedRideId!),
            CompletedRidePoints = this.CompletedRidePoints.Select(p => p.ToEntity()).ToList(),
            UserLogin = this.UserLogin,
            PlannedRideId = this.PlannedRideId == null ?
                            Guid.Empty
                            :new Guid(this.PlannedRideId),
            Statistics = this.CompletedRideStatistic?.ToEntity(),
        };
    }

}
