using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Widget;
using AndroidHUD;
using AndroidX.Core.Content;
using Google.Android.Material.Dialog;
using Google.Android.Material.FloatingActionButton;
using Google.Places;
using Plugin.Geolocator;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using QuickDate.PlacesAsync.Adapters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Object = Java.Lang.Object;
using SearchView = AndroidX.AppCompat.Widget.SearchView;
using Task = System.Threading.Tasks.Task;

namespace QuickDate.PlacesAsync
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class LocationActivity : AndroidX.Fragment.App.FragmentActivity, IOnMapReadyCallback, IOnSuccessListener, IOnFailureListener
    {
        #region Variables Basic

        private double Lat, Lng;
        private LocationManager LocationManager;
        private GoogleMap Map;
        private string Provider, DeviceAddress, SearchText;
        private SearchView SearchView;
        private TextView MapIcon, ListIcon;
        private FloatingActionButton BtnSelect;
        private LinearLayout ListButton, MapButton;
        private PlacesAdapter MAdapter;
        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                SetTheme(QuickDateTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);

                Methods.App.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.MapLayout);

                //GoogleApiClient = new GoogleApiClient.Builder(this,this,this)
                //    .EnableAutoManage(this, 0, this)
                //    .AddApi(Android.Gms.Location.Places.PlacesClass.GEO_DATA_API)
                //    .AddApi(Android.Gms.Location.Places.PlacesClass.PLACE_DETECTION_API)
                //    .Build();

                InitializeLocationManager();

                switch (PlacesApi.IsInitialized)
                {
                    case false:
                        PlacesApi.Initialize(this, GetString(Resource.String.google_key));
                        break;
                }

                //Get Value And Set Toolbar
                InitComponent();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                AddOrRemoveEvent(true);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();
                AddOrRemoveEvent(false);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnStart()
        {
            try
            {
                base.OnStart();
                //GoogleApiClient.Connect();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnStop()
        {
            try
            {
                //GoogleApiClient.Disconnect();
                base.OnStop();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Functions

        private async void InitComponent()
        {
            try
            {
                MapIcon = FindViewById<TextView>(Resource.Id.map_icon);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, MapIcon, IonIconsFonts.Locate);

                ListIcon = FindViewById<TextView>(Resource.Id.list_icon);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ListIcon, IonIconsFonts.List);

                MapButton = FindViewById<LinearLayout>(Resource.Id.map_button);
                ListButton = FindViewById<LinearLayout>(Resource.Id.list_button);

                SearchView = FindViewById<SearchView>(Resource.Id.searchView);
                SearchView.SetQuery("", false);
                SearchView.SetIconifiedByDefault(false);
                SearchView.OnActionViewExpanded();
                SearchView.Iconified = false;
                SearchView.ClearFocus();

                //Change text colors
                var editText = (EditText)SearchView.FindViewById(Resource.Id.search_src_text);
                editText.SetHintTextColor(Color.Black);
                editText.SetTextColor(Color.ParseColor("#888888"));
                editText.Hint = GetText(Resource.String.Lbl_SearchForPlace);

                //Change Color Icon Search
                ImageView searchViewIcon = (ImageView)SearchView.FindViewById(Resource.Id.search_mag_icon);
                searchViewIcon.SetColorFilter(Color.ParseColor(AppSettings.MainColor));

                BtnSelect = FindViewById<FloatingActionButton>(Resource.Id.add_button);

                var mapFrag = SupportMapFragment.NewInstance();
                SupportFragmentManager.BeginTransaction().Add(Resource.Id.map, mapFrag, mapFrag.Tag)?.Commit();
                mapFrag.GetMapAsync(this);

                await GetPosition();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                switch (addEvent)
                {
                    // true +=  // false -=
                    case true:
                        MapButton.Click += IconMyLocationOnClick;
                        ListButton.Click += ListButtonOnClick;
                        BtnSelect.Click += BtnSelectOnClick;
                        SearchView.QueryTextChange += SearchViewOnQueryTextChange;
                        SearchView.QueryTextSubmit += SearchViewOnQueryTextSubmit;
                        break;
                    default:
                        MapButton.Click -= IconMyLocationOnClick;
                        ListButton.Click -= ListButtonOnClick;
                        BtnSelect.Click -= BtnSelectOnClick;
                        SearchView.QueryTextChange -= SearchViewOnQueryTextChange;
                        SearchView.QueryTextSubmit -= SearchViewOnQueryTextSubmit;
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Search View

        private async void SearchViewOnQueryTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            try
            {
                SearchText = e.NewText;

                if (string.IsNullOrEmpty(SearchText) || string.IsNullOrWhiteSpace(SearchText))
                    return;

                SearchView.ClearFocus();

                //Show a progress
                RunOnUiThread(() => { AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading)); });

                var latLng = await GetLocationFromAddress(SearchText.Replace(" ", ""));
                if (latLng != null)
                {
                    RunOnUiThread(() => { AndHUD.Shared.Dismiss(); });

                    DeviceAddress = SearchText;

                    Lat = latLng.Latitude;
                    Lng = latLng.Longitude;

                    // Creating a marker
                    MarkerOptions markerOptions = new MarkerOptions();

                    // Setting the position for the marker
                    markerOptions.SetPosition(latLng);

                    var addresses = await ReverseGeocodeCurrentLocation(latLng);
                    if (addresses != null)
                    {
                        DeviceAddress = addresses.GetAddressLine(0); // If any additional address line present than only, check with max available address lines by getMaxAddressLineIndex()
                        //string city = addresses.Locality;
                        //string state = addresses.AdminArea;
                        //string country = addresses.CountryName;
                        //string postalCode = addresses.PostalCode;
                        //string knownName = addresses.FeatureName; // Only if available else return NULL 

                        // Setting the title for the marker.
                        // This will be displayed on taping the marker
                        markerOptions.SetTitle(DeviceAddress);
                    }

                    // Clears the previously touched position
                    Map.Clear();

                    // Animating to the touched position
                    Map.AnimateCamera(CameraUpdateFactory.NewLatLng(latLng));

                    // Placing a marker on the touched position
                    Map.AddMarker(markerOptions);

                    CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                    builder.Target(latLng);
                    builder.Zoom(18);
                    builder.Bearing(155);
                    builder.Tilt(65);

                    CameraPosition cameraPosition = builder.Build();

                    CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                    Map.MoveCamera(cameraUpdate);
                }
                else
                {
                    RunOnUiThread(() => { AndHUD.Shared.Dismiss(); });


                    //Error Message  
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Error_DisplayAddress), ToastLength.Long)?.Show();
                }
            }
            catch (Exception exception)
            {
                RunOnUiThread(() => { AndHUD.Shared.Dismiss(); });
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void SearchViewOnQueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            try
            {
                SearchText = e.NewText;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Events

        private void ListButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                GetNearbyPlaces();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void BtnSelectOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent();
                intent.PutExtra("Address", DeviceAddress);
                intent.PutExtra("latLng", Lat + "," + Lng);
                SetResult(Result.Ok, intent);
                Finish();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private async void IconMyLocationOnClick(object sender, EventArgs e)
        {
            try
            {
                await GetPosition();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Permissions 

        //Permissions
        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                switch (requestCode)
                {
                    case 105 when grantResults.Length > 0 && grantResults[0] == Permission.Granted:
                        await GetPosition();
                        break;
                    case 105:
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long)?.Show();
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Location

        private void InitializeLocationManager()
        {
            try
            {
                LocationManager = (LocationManager)GetSystemService(LocationService);
                var criteriaForLocationService = new Criteria
                {
                    Accuracy = Accuracy.Fine
                };
                var acceptableLocationProviders = LocationManager.GetProviders(criteriaForLocationService, true);
                Provider = acceptableLocationProviders.Any() ? acceptableLocationProviders.First() : string.Empty;
                Console.WriteLine(Provider);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //Get Position GPS Current Location
        private async Task GetPosition()
        {
            try
            {
                switch ((int)Build.VERSION.SdkInt)
                {
                    // Check if we're running on Android 5.0 or higher
                    case < 23:
                        await CheckAndGetLocation();
                        break;
                    default:
                        {
                            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted && ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) == Permission.Granted)
                            {
                                await CheckAndGetLocation();
                            }
                            else
                            {
                                new PermissionsController(this).RequestPermission(105);
                            }

                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private async Task CheckAndGetLocation()
        {
            try
            {
                if (!LocationManager.IsProviderEnabled(LocationManager.GpsProvider))
                {

                }
                else
                {
                    var locator = CrossGeolocator.Current;
                    locator.DesiredAccuracy = 50;
                    var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(10000));
                    Console.WriteLine("Position Status: {0}", position.Timestamp);
                    Console.WriteLine("Position Latitude: {0}", position.Latitude);
                    Console.WriteLine("Position Longitude: {0}", position.Longitude);

                    Lat = position.Latitude;
                    Lng = position.Longitude;

                    var dd = locator.StopListeningAsync();
                    Console.WriteLine(dd);

                    OnLocationChanged();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private async Task<Address> ReverseGeocodeCurrentLocation(LatLng latLng)
        {
            try
            {
#pragma warning disable 618
                var locale = (int)Build.VERSION.SdkInt < 25 ? Resources?.Configuration?.Locale : Resources?.Configuration?.Locales?.Get(0) ?? Resources?.Configuration?.Locale;
#pragma warning restore 618
                Geocoder geocode = new Geocoder(this, locale);

                var addresses = await geocode.GetFromLocationAsync(latLng.Latitude, latLng.Longitude, 2); // Here 1 represent max location result to returned, by documents it recommended 1 to 5
                switch (addresses.Count)
                {
                    case > 0:
                        //string address = addresses[0].GetAddressLine(0); // If any additional address line present than only, check with max available address lines by getMaxAddressLineIndex()
                        //string city = addresses[0].Locality;
                        //string state = addresses[0].AdminArea;
                        //string country = addresses[0].CountryName;
                        //string postalCode = addresses[0].PostalCode;
                        //string knownName = addresses[0].FeatureName; // Only if available else return NULL 
                        break;
                    default:
                        //Error Message  
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Error_DisplayAddress), ToastLength.Short)?.Show();
                        break;
                }

                return addresses.FirstOrDefault();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        private async Task<LatLng> GetLocationFromAddress(string strAddress)
        {
#pragma warning disable 618
            var locale = (int)Build.VERSION.SdkInt < 25 ? Resources?.Configuration?.Locale : Resources?.Configuration?.Locales.Get(0) ?? Resources?.Configuration?.Locale;
#pragma warning restore 618
            Geocoder coder = new Geocoder(this, locale);

            try
            {
                var address = await coder.GetFromLocationNameAsync(strAddress, 2);
                switch (address)
                {
                    case null:
                        return null;
                }

                Address location = address[0];
                Lat = location.Latitude;
                Lng = location.Longitude;

                LatLng p1 = new LatLng(Lat, Lng);
                return p1;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            try
            {
                Map = googleMap;

                var makerOptions = new MarkerOptions();
                makerOptions.SetPosition(new LatLng(Lat, Lng));
                makerOptions.SetTitle(GetText(Resource.String.Lbl_Location));

                Map.AddMarker(makerOptions);
                Map.MapType = GoogleMap.MapTypeNormal;

                switch (QuickDateTools.IsTabDark())
                {
                    case true:
                        {
                            MapStyleOptions style = MapStyleOptions.LoadRawResourceStyle(this, Resource.Raw.map_dark);
                            Map.SetMapStyle(style);
                            break;
                        }
                }

                //Optional
                googleMap.UiSettings.ZoomControlsEnabled = true;
                googleMap.UiSettings.CompassEnabled = true;

                OnLocationChanged();

                googleMap.MoveCamera(CameraUpdateFactory.ZoomIn());

                LatLng location = new LatLng(Lat, Lng);

                CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                builder.Target(location);
                builder.Zoom(18);
                builder.Bearing(155);
                builder.Tilt(65);

                CameraPosition cameraPosition = builder.Build();

                CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                googleMap.MoveCamera(cameraUpdate);

                googleMap.MapClick += async (sender, e) =>
                {
                    try
                    {
                        LatLng latLng = e.Point;
                        var tapTextView = "Tapped: Point=" + e.Point;
                        Console.WriteLine(tapTextView);

                        Lat = latLng.Latitude;
                        Lng = latLng.Longitude;

                        // Creating a marker
                        MarkerOptions markerOptions = new MarkerOptions();

                        // Setting the position for the marker
                        markerOptions.SetPosition(e.Point);

                        var addresses = await ReverseGeocodeCurrentLocation(latLng);
                        if (addresses != null)
                        {
                            DeviceAddress = addresses.GetAddressLine(0); // If any additional address line present than only, check with max available address lines by getMaxAddressLineIndex()
                            //string city = addresses.Locality;
                            //string state = addresses.AdminArea;
                            //string country = addresses.CountryName;
                            //string postalCode = addresses.PostalCode;
                            //string knownName = addresses.FeatureName; // Only if available else return NULL 

                            // Setting the title for the marker.
                            // This will be displayed on taping the marker
                            markerOptions.SetTitle(DeviceAddress);
                        }

                        // Clears the previously touched position
                        googleMap.Clear();

                        // Animating to the touched position
                        googleMap.AnimateCamera(CameraUpdateFactory.NewLatLng(e.Point));

                        // Placing a marker on the touched position
                        googleMap.AddMarker(markerOptions);
                    }
                    catch (Exception exception)
                    {
                        Methods.DisplayReportResultTrack(exception);
                    }
                };

                googleMap.MapLongClick += (sender, e) =>
                {
                    try
                    {
                        var tapTextView = "Long Pressed: Point=" + e.Point;
                        Console.WriteLine(tapTextView);
                    }
                    catch (Exception exception)
                    {
                        Methods.DisplayReportResultTrack(exception);
                    }
                };

                googleMap.CameraChange += (sender, e) =>
                {
                    try
                    {
                        var cameraTextView = e.Position.ToString();
                        Console.WriteLine(cameraTextView);
                    }
                    catch (Exception exception)
                    {
                        Methods.DisplayReportResultTrack(exception);
                    }
                };
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private async void OnLocationChanged()
        {
            try
            {
                // Creating a marker
                MarkerOptions markerOptions = new MarkerOptions();

                LatLng latLng = new LatLng(Lat, Lng);
                // Setting the position for the marker
                markerOptions.SetPosition(latLng);
                markerOptions.SetTitle(GetText(Resource.String.Lbl_Title_Location));

#pragma warning disable 618
                var locale = (int)Build.VERSION.SdkInt < 25 ? Resources?.Configuration?.Locale : Resources?.Configuration?.Locales.Get(0) ?? Resources?.Configuration?.Locale;
#pragma warning restore 618
                Geocoder geocode = new Geocoder(this, locale);

                var addresses = await geocode.GetFromLocationAsync(latLng.Latitude, latLng.Longitude, 2); // Here 1 represent max location result to returned, by documents it recommended 1 to 5
                switch (addresses?.Count)
                {
                    case > 0:
                        DeviceAddress = addresses[0].GetAddressLine(0); // If any additional address line present than only, check with max available address lines by getMaxAddressLineIndex()
                                                                        //string city = addresses[0].Locality;
                                                                        //string state = addresses[0].AdminArea;
                                                                        //string country = addresses[0].CountryName;
                                                                        //string postalCode = addresses[0].PostalCode;
                                                                        //string knownName = addresses[0].FeatureName; // Only if available else return NULL 

                        // Setting the title for the marker.
                        // This will be displayed on taping the marker
                        markerOptions.SetSnippet(DeviceAddress);
                        break;
                }

                if (Map != null)
                {
                    Map.Clear();

                    Map.AddMarker(markerOptions);
                    //Map.SetOnInfoWindowClickListener(this); // Add event click on marker icon
                    Map.MapType = GoogleMap.MapTypeNormal;

                    var builder = CameraPosition.InvokeBuilder();
                    builder.Target(new LatLng(Lat, Lng));
                    var cameraPosition = builder.Zoom(17).Target(new LatLng(Lat, Lng)).Build();
                    cameraPosition.Zoom = 18;

                    var cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                    Map.MoveCamera(cameraUpdate);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Nearby Places

        private void GetNearbyPlaces()
        {
            try
            {
                var placesClient = PlacesApi.CreateClient(this);
                List<Place.Field> placeFields = new List<Place.Field> { Place.Field.Name, Place.Field.Address };

                FindCurrentPlaceRequest currentPlaceRequest = FindCurrentPlaceRequest.NewInstance(placeFields);
                var currentPlaceTask = placesClient.FindCurrentPlace(currentPlaceRequest);
                currentPlaceTask.AddOnSuccessListener(this, this);
                currentPlaceTask.AddOnFailureListener(this, this);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnSuccess(Object result)
        {
            try
            {
                var findCurrentPlaceResponse = (FindCurrentPlaceResponse)result;
                switch (findCurrentPlaceResponse)
                {
                    case null:
                        return;
                }
                MAdapter = new PlacesAdapter();
                MAdapter.PlacesList = new ObservableCollection<MyPlace>();
                foreach (var placeLikelihood in findCurrentPlaceResponse.PlaceLikelihoods)
                {
                    MAdapter.PlacesList.Add(new MyPlace
                    {
                        Address = placeLikelihood.Place.Address,
                        AddressComponents = placeLikelihood.Place.AddressComponents,
                        Attributions = placeLikelihood.Place.Attributions,
                        Id = placeLikelihood.Place.Id,
                        LatLng = placeLikelihood.Place.LatLng,
                        Name = placeLikelihood.Place.Name,
                        OpeningHours = placeLikelihood.Place.OpeningHours,
                        PhoneNumber = placeLikelihood.Place.PhoneNumber,
                        PhotoMetadatas = placeLikelihood.Place.PhotoMetadatas,
                        PlusCode = placeLikelihood.Place.PlusCode,
                        PriceLevel = placeLikelihood.Place.PriceLevel,
                        Rating = placeLikelihood.Place.Rating,
                        UserRatingsTotal = placeLikelihood.Place.UserRatingsTotal,
                        Viewport = placeLikelihood.Place.Viewport,
                        WebsiteUri = placeLikelihood.Place.WebsiteUri,
                    });
                }

                var dialogList = new MaterialAlertDialogBuilder(this);
                dialogList.SetTitle(Resource.String.Lbl_NearBy);
                dialogList.SetAdapter(MAdapter, (sender, args) =>
                {
                    try
                    {
                        var item = (MyPlace)MAdapter.GetItem(args.Which);
                        if (item != null)
                        {
                            Intent intent = new Intent();
                            intent.PutExtra("Address", item.Address);
                            intent.PutExtra("latLng", item.LatLng.Latitude + "," + item.LatLng.Longitude);
                            SetResult(Result.Ok, intent);
                            Finish();
                        }
                    }
                    catch (Exception exception)
                    {
                        Methods.DisplayReportResultTrack(exception);
                    }
                });
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            try
            {
                Methods.DisplayReportResultTrack(e);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

    }
}