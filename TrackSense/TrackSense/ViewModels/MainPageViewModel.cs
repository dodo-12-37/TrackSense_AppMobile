using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TrackSense.Models;
using TrackSense.Services;
using TrackSense.Services.Bluetooth;
using TrackSense.Views;

namespace TrackSense.ViewModels;

public partial class MainPageViewModel : BaseViewModel
{
    BluetoothService _bluetoothService;
    RideService _rideService;
    IConnectivity _connectivity;
    public ObservableCollection<CompletedRideSummary> CompletedRideSummaries { get; } = new();

    [ObservableProperty]
    bool isConnected;

    [ObservableProperty]
    bool isReceivingData;

    [ObservableProperty]
    bool isRefreshing;

    public MainPageViewModel(BluetoothService btService, RideService rideService, IConnectivity connectivity)
    {
        Title = "Accueil";
        _bluetoothService = btService;
        _rideService = rideService;
        _connectivity = connectivity;

        BluetoothObserver bluetoothObserver = new BluetoothObserver(this._bluetoothService,
            async (value) =>
            {
                switch (value.Type)
                {
                    case BluetoothEventType.CONNECTION:
                        IsConnected = true;
                        break;
                    case BluetoothEventType.DECONNECTION:
                        IsConnected = false;
                        this._rideService.InterruptReception();
                        break;
                    case BluetoothEventType.SENDING_RIDE_STATS:
                        await this._rideService.ReceiveRideDataFromDevice(value.RideData);
                        IsReceivingData = this._bluetoothService.IsReceiving;
                        break;
                    case BluetoothEventType.SENDING_RIDE_POINT:
                        await this._rideService.ReceivePointDataFromDevice(value.RidePoint);
                        IsReceivingData = this._bluetoothService.IsReceiving;
                        break;
                    default:
                        break;
                }
            });

        ////SimulateGetRideFromAPI();
        //this.SimulatePostRideToAPI();
    }

    private void SimulateGetRideFromAPI()
    {
        TrackSense.Entities.CompletedRideSummary ride = new TrackSense.Entities.CompletedRideSummary()
        {
            CompletedRideId = Guid.NewGuid(),
            PlannedRideName = "Faux trajet",
            StartedAt = DateTime.Now,
            Distance = 13,
            Duration = TimeSpan.FromMinutes(48),
        };

        TrackSense.Entities.CompletedRideSummary ride2 = new TrackSense.Entities.CompletedRideSummary()
        {
            CompletedRideId = Guid.NewGuid(),
            PlannedRideName = "Faux trajet",
            StartedAt = DateTime.Now,
            Distance = 23,
            Duration = TimeSpan.FromMinutes(63),
        };

        CompletedRideSummaries.Add(new CompletedRideSummary(ride));
        CompletedRideSummaries.Add(new CompletedRideSummary(ride2));
    }

    async private void SimulatePostRideToAPI()
    {
        Entities.CompletedRide completedRide = this.GenerateFakeCompletedRide();
        await this.PostCompletedRideAsync(completedRide);

    }
    [RelayCommand]
    async Task PostCompletedRideAsync(Entities.CompletedRide p_completedRide)
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;

            var result = await _rideService.PostCompletedRideAsync(p_completedRide);

            if (result.IsSuccessStatusCode)
            {
                // Handle success, e.g., show a success message to the user
                await Shell.Current.DisplayAlert("Success", "Completed ride posted successfully.", "Ok");
            }
            else
            {
                // Handle failure, e.g., show an error message to the user
                await Shell.Current.DisplayAlert("Error", "Failed to post completed ride.", "Ok");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);

            // Handle and display the exception to the user
            await Shell.Current.DisplayAlert("Error", $"An error occurred while posting the completed ride: {ex.Message}", "Ok");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task GoToSettingsAsync()
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    [RelayCommand]
    async Task GoToTrackSenseDevices()
    {
        this.IsConnected = this._bluetoothService.GetConnectedDevice() is not null;

        if (!this.IsConnected)
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

        try
        {
            if (await CheckInternetConnexion())
            {
                Entities.CompletedRide completedRide = await _rideService.GetCompletedRide(rideSummary.CompletedRideId);
                //Entities.CompletedRide completedRide = GenerateFakeCompletedRide();
                //Entities.CompletedRide completedRide = _rideService.GetCompletedRideFromLocalStorage(rideSummary.CompletedRideId

                //Référence le shell, donc pas bonne pratique, il faudrait une interface.
                await Shell.Current.GoToAsync($"{nameof(CompletedRidePage)}", true,
                    new Dictionary<string, object>
                    {
                            {"CompletedRide", new Models.CompletedRide(completedRide) }
                    });
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            await Shell.Current.DisplayAlert("Oups", "Une erreur est survenue lors de la récupération du trajet", "Ok");
        }

    }

    [RelayCommand]
    public async Task GetCompletedRidesAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {

            if (await CheckInternetConnexion())
            {
                IsBusy = true;
                IsRefreshing = true;

                List<TrackSense.Entities.CompletedRideSummary> completedRides = await _rideService.GetUserCompletedRides();
                //List<TrackSense.Entities.CompletedRideSummary> completedRides = _rideService.GetCompletedRideSummariesFromLocalStorage();


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

    private async Task<bool> CheckInternetConnexion()
    {
        bool internetIsAvailable = _connectivity.NetworkAccess == NetworkAccess.Internet;

        if (!internetIsAvailable)
        {
            await Shell.Current.DisplayAlert("Problème de connexion à internet", "Veuillez vérifier votre connexion à internet puis réessayer", "Ok");
        }

        return internetIsAvailable;
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
        var rideId = Guid.NewGuid();
        return new Entities.CompletedRide()
        {
            UserLogin = "debug",
            CompletedRideId = rideId,
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
                        Speed = 16,

                        Timestamp = DateTime.Now.AddSeconds(3)
                    },
                    Temperature = 23.4,
                    EffectiveTime = TimeSpan.FromMinutes(1)
                }/*,
                new Entities.CompletedRidePoint()
                {
                    RideStep = 2,
                    Location = new Location()
                    {
                        Latitude = 46.785474,
                        Longitude = -71.285702,
                        Altitude = 12,
                        Speed = 6,

                        Timestamp = DateTime.Now.AddSeconds(6)
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
                        Speed = 10,

                        Timestamp = DateTime.Now.AddSeconds(9)
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
                        Speed = 12,

                        Timestamp = DateTime.Now.AddSeconds(12)
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
                        Speed = 15,

                        Timestamp = DateTime.Now.AddSeconds(16)
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
                        Speed = 17,

                        Timestamp = DateTime.Now.AddSeconds(19)
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
                        Speed = 5,
                        Accuracy = 0,
                        Timestamp = DateTime.Now.AddSeconds(23)
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
                        Speed = 13,

                        Timestamp = DateTime.Now.AddSeconds(27)
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
                        Speed = 12,
                        Timestamp = DateTime.Now.AddSeconds(30)
                    },
                    Temperature = 23.5,
                    EffectiveTime = TimeSpan.FromMinutes(1)
                }*/
            }
           
        };
    }
    #endregion
}
