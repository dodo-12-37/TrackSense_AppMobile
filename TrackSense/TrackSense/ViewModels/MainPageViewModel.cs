using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrackSense.Models;
using TrackSense.Views;

namespace TrackSense.ViewModels;

public partial class MainPageViewModel : BaseViewModel
{
    public MainPageViewModel()
    {
        Title = "Accueil";
    }

    [RelayCommand]
    async Task GoToTrackSenseDevices()
    {
        await Shell.Current.GoToAsync(nameof(TrackSenseDevicesPage));
    }
}
