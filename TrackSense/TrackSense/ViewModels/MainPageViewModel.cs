using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TrackSense.Entities.Exceptions;
using TrackSense.Models;
using TrackSense.Services;
using TrackSense.Services.Bluetooth;
using TrackSense.Views;

namespace TrackSense.ViewModels;

public partial class MainPageViewModel : BaseViewModel
{
    BluetoothService _bluetoothService;
    RideService _rideService;
    public ObservableCollection<CompletedRideSummary> CompletedRideSummaries { get; } = new();

    [ObservableProperty]
    bool isConnected;

    [ObservableProperty]
    bool isReceivingData;

    [ObservableProperty]
    bool isRefreshing;

    public MainPageViewModel(BluetoothService btService, RideService rideService)
    {
        Title = "Accueil";
        _bluetoothService = btService;
        _rideService = rideService;

        BluetoothObserver bluetoothObserver = new BluetoothObserver(this._bluetoothService,
            async (value) =>
            {
                switch (value.Type)
                {
                    case BluetoothEventType.CONNECTION:
                        isConnected = true;
                        break;
                    case BluetoothEventType.DECONNECTION:
                        isConnected = false;
                        this._rideService.InterruptReception();
                        break;
                    case BluetoothEventType.SENDING_RIDE_STATS:
                        isReceivingData = true;
                        await this._rideService.ReceiveRideDataFromDevice(value.RideData);
                        break;
                    case BluetoothEventType.SENDING_RIDE_POINT:
                        isReceivingData = true;
                        await this._rideService.ReceivePointDataFromDevice(value.RidePoint);
                        isReceivingData = false;
                        break;
                    default:
                        break;
                }
            });

        SimulateGetRideFromAPI();
    }

    private void SimulateGetRideFromAPI()
    {
        TrackSense.Entities.CompletedRideSummary ride = new TrackSense.Entities.CompletedRideSummary()
        {
            CompletedRideId = Guid.NewGuid(),
            PlannedRideName = "Ride1",
            StartedAt = DateTime.Now,
            Distance = 13,
            Duration = TimeSpan.FromMinutes(48),
        };

        TrackSense.Entities.CompletedRideSummary ride2 = new TrackSense.Entities.CompletedRideSummary()
        {
            CompletedRideId = Guid.NewGuid(),
            PlannedRideName = null,
            StartedAt = DateTime.Now,
            Distance = 23,
            Duration = TimeSpan.FromMinutes(63),
        };

        CompletedRideSummaries.Add(new CompletedRideSummary(ride));
        CompletedRideSummaries.Add(new CompletedRideSummary(ride2));
    }

    [RelayCommand]
    async Task GoToSettingsAsync()
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    [RelayCommand]
    async Task GoToTrackSenseDevices()
    {
        if (!IsConnected)
        {
            await Shell.Current.GoToAsync(nameof(TrackSenseDevicesPage));
        }
    }

    [RelayCommand]
    async Task GoToDetailsAsync(CompletedRideSummary rideSummary)
    {
        if (rideSummary is null)
        {
            return;
        }

        //Entities.CompletedRide completedRide = await _rideService.GetCompletedRide(rideSummary.CompletedRideId);
        Entities.CompletedRide completedRide = GenerateFakeCompletedRide();

        //Référence le shell, donc pas bonne pratique, il faudrait une interface.
        await Shell.Current.GoToAsync($"{nameof(CompletedRidePage)}", true,
            new Dictionary<string, object>
            {
                    {"CompletedRide", new Models.CompletedRide(completedRide) }
            });
    }

    [RelayCommand]
    async Task GetCompletedRidesAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            IsRefreshing = true;

            //List<TrackSense.Entities.CompletedRideSummary> completedRides = await _rideService.GetUserCompletedRides();
            List<TrackSense.Entities.CompletedRideSummary> completedRides = _rideService.GetCompletedRideSummariesFromLocalStorage();


            if (CompletedRideSummaries.Count != 0)
            {
                CompletedRideSummaries.Clear();
            }

            foreach (TrackSense.Entities.CompletedRideSummary ride in completedRides)
            {
                CompletedRideSummaries.Add(new CompletedRideSummary(ride)); // si on a trop de données, ne pas utiliser cette méthode car lève un évenement pour chaque ajout
                // si on a trop de données, créer une nouvelle liste ou une nouvelle ObservableCollection et l'assigner à CompletedRides ou trouver des
                //library helpers qui ont des observableRange collections qui feront de l'ajout de batch
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            await Shell.Current.DisplayAlert("Erreur", $"Une erreur est survenue lors de la récupération des trajets: {ex.Message}", "Ok");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    async Task SimulateRideReceptionAsync()
    {
        this._bluetoothService.SimulateRideReception();
    }

    [RelayCommand]
    async Task SimulatePointsReceptionAsync()
    {
        this._bluetoothService.SimulatePointsReception();
    }

    #region DEBUG
    private Entities.CompletedRide GenerateFakeCompletedRide()
    {
        return new Entities.CompletedRide()
        {
            CompletedRideId = Guid.NewGuid(),
            PlannedRideId = Guid.NewGuid(),
            CompletedRidePoints = new List<Entities.CompletedRidePoint>()
            {
                new Entities.CompletedRidePoint()
                {
                    RideStep = 1,
                    Location = new Location()
                    {
                        Latitude = 46.785014,
                        Longitude = -71.286721,
                        Altitude = 0,
                        Speed = 0,
                        Accuracy = 0,
                        Timestamp = DateTime.Now
                    },
                    Temperature = 23.4,
                    EffectiveTime = TimeSpan.FromMinutes(1)
                },
                new Entities.CompletedRidePoint()
                {
                    RideStep = 2,
                    Location = new Location()
                    {
                        Latitude = 46.785474,
                        Longitude = -71.285702,
                        Altitude = 12,
                        Speed = 0,
                        Accuracy = 0,
                        Timestamp = DateTime.Now
                    },
                    Temperature = 23.5,
                    EffectiveTime = TimeSpan.FromMinutes(1)
                },
                new Entities.CompletedRidePoint()
                {
                    RideStep = 3,
                    Location = new Location()
                    {
                        Latitude = 46.785506,
                        Longitude = -71.284882,
                        Altitude = 12,
                        Speed = 0,
                        Accuracy = 0,
                        Timestamp = DateTime.Now
                    },
                    Temperature = 23.5,
                    EffectiveTime = TimeSpan.FromMinutes(1)
                },
                                new Entities.CompletedRidePoint()
                {
                    RideStep = 4,
                    Location = new Location()
                    {
                        Latitude = 46.785222,
                        Longitude = -71.284562,
                        Altitude = 0,
                        Speed = 0,
                        Accuracy = 0,
                        Timestamp = DateTime.Now
                    },
                    Temperature = 23.4,
                    EffectiveTime = TimeSpan.FromMinutes(1)
                },
                new Entities.CompletedRidePoint()
                {
                    RideStep = 5,
                    Location = new Location()
                    {
                        Latitude = 46.785855,
                        Longitude = -71.282086,
                        Altitude = 12,
                        Speed = 0,
                        Accuracy = 0,
                        Timestamp = DateTime.Now
                    },
                    Temperature = 23.5,
                    EffectiveTime = TimeSpan.FromMinutes(1)
                },
                new Entities.CompletedRidePoint()
                {
                    RideStep = 6,
                    Location = new Location()
                    {
                        Latitude = 46.785722,
                        Longitude = -71.281048,
                        Altitude = 12,
                        Speed = 0,
                        Accuracy = 0,
                        Timestamp = DateTime.Now
                    },
                    Temperature = 23.5,
                    EffectiveTime = TimeSpan.FromMinutes(1)
                },
                                new Entities.CompletedRidePoint()
                {
                    RideStep = 7,
                    Location = new Location()
                    {
                        Latitude = 46.786405,
                        Longitude = -71.279496,
                        Altitude = 0,
                        Speed = 0,
                        Accuracy = 0,
                        Timestamp = DateTime.Now
                    },
                    Temperature = 23.4,
                    EffectiveTime = TimeSpan.FromMinutes(1)
                },
                new Entities.CompletedRidePoint()
                {
                    RideStep = 8,
                    Location = new Location()
                    {
                        Latitude = 46.786886,
                        Longitude = -71.277390,
                        Altitude = 12,
                        Speed = 0,
                        Accuracy = 0,
                        Timestamp = DateTime.Now
                    },
                    Temperature = 23.5,
                    EffectiveTime = TimeSpan.FromMinutes(1)
                },
                new Entities.CompletedRidePoint()
                {
                    RideStep = 9,
                    Location = new Location()
                    {
                        Latitude = 46.785694,
                        Longitude = -71.276348,
                        Altitude = 12,
                        Speed = 0,
                        Accuracy = 0,
                        Timestamp = DateTime.Now
                    },
                    Temperature = 23.5,
                    EffectiveTime = TimeSpan.FromMinutes(1)
                }
            },
            Statistics = new Entities.CompletedRideStatistics()
            {
                AverageSpeed = 14.56,
                MaximumSpeed = 27.34,
                Distance = 10560,
                Duration = TimeSpan.FromMinutes(10),
                NumberOfPoints = 9,
                Calories = 100,
                Falls = 0
            }
        };
    }
    #endregion
}
