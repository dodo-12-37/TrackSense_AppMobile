namespace TrackSense.Models;

public class TrackSenseDevice
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsConnected { get; set; }
    public string Address { get; set; }
}
