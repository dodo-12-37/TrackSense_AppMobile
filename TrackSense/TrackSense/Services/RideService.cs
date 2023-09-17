﻿using CommunityToolkit.Mvvm.ComponentModel;
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

    internal void ReceiveRideDataFromDevice(CompletedRide rideData)
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

        this._bluetoothService.ConfirmRideStatsReception();
    }

    internal void ReceivePointDataFromDevice(CompletedRidePoint ridePoint)
    {
        if (this._currentRide is null)
        {
            throw new InvalidOperationException();
        }
        
        int numberOfPointsReceived = this._currentRide.CompletedRidePoints.Count;
        int totalNumberOfPoints = this._currentRide.Statistics.NumberOfPoints;

        if (ridePoint.RideStep == numberOfPointsReceived + 1)
        {
            this._currentRide.CompletedRidePoints.Add(ridePoint);
            this._bluetoothService.ConfirmRideStatsReception();
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

        string userLogin = "test";

        string url = $"https://localhost:7044/api/users/{userLogin}/completedRides";

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

        string url = $"https://localhost:7044/api/completedRides/{completedRideId}";

        var response = httpClient.GetAsync(url).Result;

        CompletedRide completedRide = null;

        if (response.IsSuccessStatusCode)
        {
            API.APIDTO.CompletedRideDTO completedRideSummaryDTO = await response.Content.ReadFromJsonAsync<API.APIDTO.CompletedRideDTO>();
            completedRide = completedRideSummaryDTO.ToEntity();
        }

        return completedRide;
    }
}
