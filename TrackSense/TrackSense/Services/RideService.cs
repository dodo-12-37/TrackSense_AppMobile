using CommunityToolkit.Mvvm.ComponentModel;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using TrackSense.Entities;
using TrackSense.Services.Bluetooth;

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

        List<CompletedRide> storedRide = this._rideData.ListCompletedRides();
        if (storedRide.Count > 0)
        {
            this._rideData.DeleteAllCompletedRides();
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

    //public List<CompletedRideSummary> GetCompletedRideSummariesFromLocalStorage()
    //{
    //    List<CompletedRide> rides = this._rideData.ListCompletedRides();
    //    List<CompletedRideSummary> summaries = new();

    //    foreach (CompletedRide ride in rides)
    //    {

    //    }

    //}

    private void PostCurrentRide()
    {
        //Vérifier la connectivité du cellulaire

        //Envoyer les données au serveur

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
}
