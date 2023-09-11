using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackSense.Models;

namespace TrackSense.ViewModels
{
    public partial class CompletedRideViewModel : BaseViewModel
    {
        [ObservableProperty]
        CompletedRide completedRide;

        public CompletedRideViewModel()
        {
            Title = "Completed Ride";
        }
    }
}
