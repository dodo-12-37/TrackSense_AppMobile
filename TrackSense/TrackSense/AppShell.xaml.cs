using TrackSense.Views;

namespace TrackSense
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(TrackSenseDevicesPage), typeof(TrackSenseDevicesPage));
        }
    }
}