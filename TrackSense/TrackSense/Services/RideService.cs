using CommunityToolkit.Mvvm.ComponentModel;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using TrackSense.Entities;
using TrackSense.Services.Bluetooth;

namespace TrackSense.Services;

public class RideService
{
    private ICompletedRideLocalData _rideData;
    BluetoothService _bluetoothService;
    CompletedRide _currentRide;

    public RideService(ICompletedRideLocalData rideData, BluetoothService bluetoothService)
    {
        _rideData = rideData;
        _bluetoothService = bluetoothService;
    }

    internal void ReceiveRideData(CompletedRide rideData)
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

        bool isConfirmed = false;

        while (!isConfirmed)
        {
            isConfirmed = this._bluetoothService.ConfirmRideStatsReception().Result;
        }
    }

    internal void ReceivePoint(CompletedRidePoint ridePoint)
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
            bool isConfirmed = false;

            while (!isConfirmed)
            {
                isConfirmed = this._bluetoothService.ConfirmPointReception().Result;
            }
        }

        if (ridePoint.RideStep == totalNumberOfPoints)
        {
            _rideData.AddCompletedRide(this._currentRide);
            this.PostCurrentRide();
            _currentRide = null;
        }
    }

    private void PostCurrentRide()
    {
        //Vérifier la connectivité du cellulaire

        //Envoyer les données au serveur

    }
}
