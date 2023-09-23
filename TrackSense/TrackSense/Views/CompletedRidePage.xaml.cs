using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.UI.Maui;
using NetTopologySuite.Geometries;
using TrackSense.Models;
using TrackSense.ViewModels;
using Color = Mapsui.Styles.Color;

namespace TrackSense.Views;

public partial class CompletedRidePage : ContentPage
{
	public CompletedRidePage(CompletedRideViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

	public void DisplayMap(List<CompletedRidePoint> points)
	{
        MapControl mapControl = new Mapsui.UI.Maui.MapControl();
        mapControl.Map?.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());

        ILayer lineStringLayer = CreateLineStringLayer(points, CreateLineStringStyle());
        mapControl.Map.Layers.Add(lineStringLayer);

        double boxSize = lineStringLayer.Extent!.Width > lineStringLayer.Extent.Height ? lineStringLayer.Extent.Width : lineStringLayer.Extent.Height;
        double resolution = boxSize / 256;
        mapControl.Map.Home = n => n.CenterOnAndZoomTo(lineStringLayer.Extent!.Centroid, resolution);

        mapControl.Map.Navigator.RotationLock = true;
        mapContainer.Children.Add(mapControl);
    }

    private ILayer CreateLineStringLayer(List<CompletedRidePoint> points, IStyle? style = null)
    {
        LineString lineString = new LineString(points.Select(p => SphericalMercator.FromLonLat(p.Location.Longitude, p.Location.Latitude).ToCoordinate()).ToArray());

        return new MemoryLayer
        {
            Features = new[] { new GeometryFeature { Geometry = lineString } },
            Name = "LineStringLayer",
            Style = style

        };
    }

    private IStyle CreateLineStringStyle()
    {
        return new VectorStyle
        {
            Fill = null,
            Outline = null,
#pragma warning disable CS8670 // Object or collection initializer implicitly dereferences possibly null member.
            Line = { Color = Color.FromString("Red"), Width = 4 }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is CompletedRideViewModel viewModel)
        {
            CompletedRide rideToDisplay = viewModel.CompletedRide;
            DisplayMap(rideToDisplay.CompletedRidePoints);
        }
    }
}