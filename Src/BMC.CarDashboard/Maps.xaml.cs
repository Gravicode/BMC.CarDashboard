using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Services.Maps;
using Windows.Devices.Geolocation;
using System.Threading.Tasks;
using Windows.UI;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BMC.CarDashboard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Maps : Page
    {
        public Maps()
        {
            this.InitializeComponent();

            Setup();
        }

        void Setup()
        {
            MapControl1.MapServiceToken = "4FFzCTHPchUsgC7dR2Sf~nS5pRgtfRyreXtn_1mpHxQ~AsPlPpTyoodRinJclFBMnOZe8JrcWJEa8AcxQP4EDwh0WPcM581UyJECSqZMYNU_";
            BtnMyLocation.Click += async(a, b) => {
                var myloc = await GetMyLocation();
                MapControl1.Center = new Geopoint(myloc);
                MapControl1.ZoomLevel = 12;
                MapControl1.LandmarksVisible = true;
            };
            CmbDriveBy.Items.Clear();
            CmbDriveBy.Items.Add("Driving");
            CmbDriveBy.Items.Add("Walking");
            CmbDriveBy.SelectedIndex = 0;
            BtnExit.Click += (a, b) => { if (this.Frame.CanGoBack) this.Frame.GoBack(); else this.Frame.Navigate(typeof(MainPage)); };

            BtnGo.Click += async (a, b) =>
            {
                if (string.IsNullOrEmpty(TxtFrom.Text) || string.IsNullOrEmpty(TxtTo.Text)) return;
                var from = await GetLocationByAddress(TxtFrom.Text);
                var to = await GetLocationByAddress(TxtTo.Text);
                MapControl1.Center = new Geopoint(from);
                MapControl1.ZoomLevel = 12;
                MapControl1.LandmarksVisible = true;
                Progress1.Visibility = Visibility.Visible;
                GoFindRoute(from, to).GetAwaiter().GetResult();
                Progress1.Visibility = Visibility.Collapsed;
            };
        }
        async Task GoFindRoute(BasicGeoposition startLocation, BasicGeoposition endLocation)
        {
            // Start at Microsoft in Redmond, Washington.
            //BasicGeoposition startLocation = new BasicGeoposition() { Latitude = 47.643, Longitude = -122.131 };

            // End at the city of Seattle, Washington.
            //BasicGeoposition endLocation = new BasicGeoposition() { Latitude = 47.604, Longitude = -122.329 };


            // Get the route between the points.
            MapRouteFinderResult routeResult;
            if (CmbDriveBy.SelectedIndex == 0)
            {
                routeResult =
                      await MapRouteFinder.GetDrivingRouteAsync(
                      new Geopoint(startLocation),
                      new Geopoint(endLocation),
                      MapRouteOptimization.Time,
                      MapRouteRestrictions.None);
            }
            else
            {
                routeResult =
                     await MapRouteFinder.GetWalkingRouteAsync(
                     new Geopoint(startLocation),
                     new Geopoint(endLocation)
                    );
            }

            if (routeResult.Status == MapRouteFinderStatus.Success)
            {
                // Use the route to initialize a MapRouteView.
                MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                viewOfRoute.RouteColor = Colors.Yellow;
                viewOfRoute.OutlineColor = Colors.Black;

                MapControl1.Routes.Clear();
                // Add the new MapRouteView to the Routes collection
                // of the MapControl.
                MapControl1.Routes.Add(viewOfRoute);

                // Fit the MapControl to the route.
                await MapControl1.TrySetViewBoundsAsync(
                      routeResult.Route.BoundingBox,
                      null,
                      Windows.UI.Xaml.Controls.Maps.MapAnimationKind.None);
                System.Text.StringBuilder routeInfo = new System.Text.StringBuilder();

                // Display summary info about the route.
                routeInfo.Append("Total estimated time (minutes) = ");
                routeInfo.Append(routeResult.Route.EstimatedDuration.TotalMinutes.ToString());
                routeInfo.Append("\nTotal length (kilometers) = ");
                routeInfo.Append((routeResult.Route.LengthInMeters / 1000).ToString());

                // Display the directions.
                routeInfo.Append("\n\nDIRECTIONS\n");

                foreach (MapRouteLeg leg in routeResult.Route.Legs)
                {
                    foreach (MapRouteManeuver maneuver in leg.Maneuvers)
                    {
                        routeInfo.AppendLine(maneuver.InstructionText);
                    }
                }

                // Load the text box.
                TxtRoute.Text = routeInfo.ToString();
            }
            else
            {
                TxtRoute.Text =
           "A problem occurred: " + routeResult.Status.ToString();
            }
        }
        async Task<BasicGeoposition> GetLocationByAddress(string addressToGeocode="My Location")
        {
            if (addressToGeocode == "My Location") return await GetMyLocation();

            // The nearby location to use as a query hint.
            BasicGeoposition queryHint = new BasicGeoposition();
            queryHint.Latitude = -6.121435;
            queryHint.Longitude = 106.774124;
            Geopoint hintPoint = new Geopoint(queryHint);

            // Geocode the specified address, using the specified reference point
            // as a query hint. Return no more than 3 results.
            MapLocationFinderResult result =
                  await MapLocationFinder.FindLocationsAsync(
                                    addressToGeocode,
                                    hintPoint,
                                    3);

            // If the query returns results, display the coordinates
            // of the first result.
            if (result.Status == MapLocationFinderStatus.Success)
            {
                return new BasicGeoposition() { Latitude = result.Locations[0].Point.Position.Latitude, Longitude = result.Locations[0].Point.Position.Longitude };
                /*
                tbOutputText.Text = "result = (" +
                      result.Locations[0].Point.Position.Latitude.ToString() + "," +
                      result.Locations[0].Point.Position.Longitude.ToString() + ")";*/
            }
            return new BasicGeoposition();
        }

        async Task<BasicGeoposition> GetMyLocation()
        {
            // Set your current location.
            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:

                    // Get the current location.
                    Geolocator geolocator = new Geolocator();
                    Geoposition pos = await geolocator.GetGeopositionAsync();
                    Geopoint myLocation = pos.Coordinate.Point;

                    return new BasicGeoposition() { Latitude = myLocation.Position.Latitude, Longitude = myLocation.Position.Longitude }; ;
                   

                case GeolocationAccessStatus.Denied:
                    throw new Exception("Location access denied.");
                    // Handle the case  if access to location is denied.
                    //break;

                case GeolocationAccessStatus.Unspecified:
                    throw new Exception("Location access : unknown error.");

                    // Handle the case if  an unspecified error occurs.
                    //break;
            }
            return new BasicGeoposition();
        }
    }
}
