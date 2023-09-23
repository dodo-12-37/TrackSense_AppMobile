using CommunityToolkit.Mvvm.ComponentModel;
using TrackSense.Models;

namespace TrackSense.ViewModels
{
    [QueryProperty("CompletedRide", "CompletedRide")]
    public partial class CompletedRideViewModel : BaseViewModel
    {
        [ObservableProperty]
        CompletedRide completedRide;

        public CompletedRideViewModel()
        {
            Title = "Statistiques";
        }
    }
}
