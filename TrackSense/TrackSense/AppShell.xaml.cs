using TrackSense.Views;

namespace TrackSense
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(TrackSenseDevicesPage), typeof(TrackSenseDevicesPage));
            Routing.RegisterRoute(nameof(CompletedRidePage), typeof(CompletedRidePage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        }
    }
}