using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrackSense.Views;

namespace TrackSense.ViewModels;

public partial class MainPageViewModel : BaseViewModel
{
    public ObservableObject TrackSenseDevice { get; set; }

    public MainPageViewModel()
    {
        Title = "Accueil";
    }

    [RelayCommand]
    async Task GoToTrackSenseDevices()
    {
        if (TrackSenseDevice == null)
        {
            await Shell.Current.GoToAsync(nameof(TrackSenseDevicesPage));
        }
    }
}
