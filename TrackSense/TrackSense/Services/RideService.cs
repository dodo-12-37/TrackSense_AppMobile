using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using TrackSense.Entities;
using TrackSense.Services.Bluetooth;
using TrackSense.Services.API.APIDTO;

namespace TrackSense.Services;

public class RideService
{
    private ICompletedRideLocalData _rideData;
    BluetoothService _bluetoothService;
    CompletedRide _currentRide;
    List<CompletedRideSummary> _completedRides = new();
    HttpClient httpClient;

    public RideService(ICompletedRideLocalData rideData, BluetoothService bluetoothService)
    {
        _rideData = rideData;
        _bluetoothService = bluetoothService;
        httpClient = new HttpClient();
    }

    internal async Task<bool> ReceiveRideDataFromDevice(CompletedRide rideData)
    {
        if (rideData is null)
        {
            throw new ArgumentNullException(nameof(rideData));
        }

        this._currentRide = rideData;

        return await this._bluetoothService.ConfirmRideStatsReception(0);
    }

    internal async Task ReceivePointDataFromDevice(CompletedRidePoint ridePoint)
    {
        if (this._currentRide is null)
        {
            throw new InvalidOperationException();
        }
        
        int numberOfPointsReceived = this._currentRide.CompletedRidePoints.Count;
        int totalNumberOfPoints = this._currentRide.Statistics.NumberOfPoints;

        if (ridePoint.RideStep == numberOfPointsReceived + 1)
        {
            try
            {
                bool isConfirmed = await this._bluetoothService.ConfirmRideStatsReception(ridePoint.RideStep);
                Debug.Write(ridePoint.RideStep);
                if (isConfirmed)
                {
                    Debug.Write("Conrimation : point #" + ridePoint.RideStep);
                    this._currentRide.CompletedRidePoints.Add(ridePoint);
                    Debug.Write("Ajout point #" + ridePoint.RideStep);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Erreur confirmation : " + e.Message);
            }
        }
        else if (ridePoint.RideStep == numberOfPointsReceived)
        {
            try
            {
                bool isConfirmed = await this._bluetoothService.ConfirmRideStatsReception(ridePoint.RideStep);
                Debug.Write("Deuxieme confirmation : point #" + ridePoint.RideStep);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Erreur confirmation : " + e.Message);
            }

        }

        if (ridePoint.RideStep == totalNumberOfPoints)
        {
            _rideData.AddCompletedRide(this._currentRide);
            this.PostCurrentRide();
            _currentRide = null;
        }
    }
    public void PostCurrentRide()
    {
        throw new NotImplementedException();
    }
    public async Task<List<CompletedRideSummary>> GetUserCompletedRides()
    {
        if (_completedRides?.Count > 0)
        {
            return _completedRides;
        }
       
        string userLogin = "admin";

        string url = $"https://binhnguyen05-001-site1.atempurl.com/api/users/{userLogin}/completedRides";

        var response = await httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            List<API.APIDTO.CompletedRideSummaryDTO> completedRides = await response.Content.ReadFromJsonAsync<List<API.APIDTO.CompletedRideSummaryDTO>>();
            _completedRides.AddRange(completedRides.Select(ride => ride.ToEntity()));
        }
       
        return _completedRides;
    }

    public List<CompletedRideSummary> GetCompletedRideSummariesFromLocalStorage()
    {
        List<CompletedRide> rides = this._rideData.ListCompletedRides();
        List<CompletedRideSummary> summaries = new();

        foreach (CompletedRide ride in rides)
        {
            summaries.Add(new CompletedRideSummary()
            {
                CompletedRideId = ride.CompletedRideId,
                PlannedRideName = "None",
                StartedAt = ride.CompletedRidePoints.First().Location.Timestamp.DateTime,
                Duration = ride.Statistics.Duration,
                Distance = ride.Statistics.Distance
            });
        }

        return summaries;
    }


    internal async Task<HttpResponseMessage> PostCompletedRideAsync(CompletedRide p_completedRide)
    {
        try
        {
            if (p_completedRide == null)
            {
                throw new ArgumentNullException(nameof(p_completedRide));
            }

            CompletedRideDTO completedRideDTO = new CompletedRideDTO(p_completedRide);

            string url = "https://binhnguyen05-001-site1.atempurl.com/api/CompletedRides";

            var content = new StringContent(JsonConvert.SerializeObject(completedRideDTO), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                // Request was successful, you can handle it accordingly
                return response;
            }
            else
            {
                // Handle different HTTP status codes here, e.g., client errors or server errors
                // You can throw specific exceptions or log messages based on the status code
                Console.WriteLine($"HTTP Error: {response.StatusCode}");
                throw new HttpRequestException($"HTTP Error: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., serialization errors) here
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }


    internal async Task<Entities.CompletedRide> GetCompletedRide(Guid completedRideId)
    {
        if (completedRideId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(completedRideId));
        }

        string url = $"https://binhnguyen05-001-site1.atempurl.com/api/CompletedRides/{completedRideId}";

        var response = await httpClient.GetAsync(url);

        CompletedRide completedRide = null;

        if (response.IsSuccessStatusCode)
        {
            API.APIDTO.CompletedRideDTO completedRideDTO = await response.Content.ReadFromJsonAsync<API.APIDTO.CompletedRideDTO>();
            completedRide =  completedRideDTO.ToEntity();
        }

        return completedRide;
    }

    internal void DeleteRidesFromLocalStorage()
    {
        this._rideData.DeleteAllCompletedRides();
    }

    public void InterruptReception()
    {
        this._currentRide = null;
    }

    internal CompletedRide GetCompletedRideFromLocalStorage(Guid completedRideId)
    {
        return this._rideData.GetCompletedRideById(completedRideId);
    }
}
