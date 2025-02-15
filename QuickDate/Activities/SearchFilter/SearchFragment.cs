using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Bumptech.Glide.Util;
using Com.Google.Android.Gms.Ads;
using Plugin.Geolocator;
using QuickDate.Activities.Friends.Adapter;
using QuickDate.Activities.Tabbes;
using QuickDate.ButtomSheets;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.Library.Anjo.IntegrationRecyclerView;
using QuickDateClient.Classes.Global;
using QuickDateClient.Classes.Users;
using QuickDateClient.Requests;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace QuickDate.Activities.SearchFilter
{
    public class SearchFragment : AndroidX.Fragment.App.Fragment, TextView.IOnEditorActionListener
    {
        #region Variables Basic

        private LinearLayout SearchLinearLayout;
        private AutoCompleteTextView SearchBox;
        private TextView FilterButton, IconClose;
        private string SearchText = "";
        private ImageView BackIcon;

        public UsersAdapter MAdapter;
        private SwipeRefreshLayout SwipeRefreshLayout;
        private RecyclerView MRecycler;
        private GridLayoutManager LayoutManager;
        private ViewStub EmptyStateLayout;
        private View Inflated;
        private HomeActivity MainContext;
        private AdView MAdView;
        private RecyclerViewOnScrollListener MainScrollEvent;
        private int CountOffset;

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            MainContext = HomeActivity.GetInstance();
            HasOptionsMenu = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                // Use this to return your custom view for this Fragment
                View view = inflater.Inflate(Resource.Layout.SearchLayout, container, false);
                return view;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            try
            {
                base.OnViewCreated(view, savedInstanceState);

                //Get Value And Set Toolbar
                InitComponent(view);
                SetRecyclerViewAdapters();

                SwipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;
                MAdapter.OnItemClick += MAdapterOnItemClick;

                AdsGoogle.Ad_Interstitial(Activity);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);

            }
        }

        public override void OnResume()
        {
            try
            {
                AdsGoogle.LifecycleAdView(MAdView, "Resume");
                base.OnResume();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnPause()
        {
            try
            {
                AdsGoogle.LifecycleAdView(MAdView, "Pause");
                base.OnPause();
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
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public override void OnDestroy()
        {
            try
            {
                AdsGoogle.LifecycleAdView(MAdView, "Destroy");
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                BackIcon = view.FindViewById<ImageView>(Resource.Id.backArrow);
                BackIcon.SetImageResource(AppSettings.FlowDirectionRightToLeft ? Resource.Drawable.icon_back_arrow_right : Resource.Drawable.icon_back_arrow_left);
                BackIcon.ImageTintList = ColorStateList.ValueOf(QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                BackIcon.Click += BackIconOnClick;

                SearchLinearLayout = (LinearLayout)view.FindViewById(Resource.Id.searchLinearLayout);

                SearchBox = view.FindViewById<AutoCompleteTextView>(Resource.Id.searchBox);
                SearchBox.SetOnEditorActionListener(this);
                SearchBox.AfterTextChanged += SearchBoxOnAfterTextChanged;

                //Change text colors
                SearchBox.SetHintTextColor(Color.ParseColor(AppSettings.MainColor));
                SearchBox.SetTextColor(QuickDateTools.IsTabDark() ? Color.White : Color.Black);

                IconClose = (TextView)view.FindViewById(Resource.Id.IconClose);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconClose, IonIconsFonts.Close);
                IconClose.Click += IconCloseOnClick;
                IconClose.Visibility = ViewStates.Gone;

                FilterButton = (TextView)view.FindViewById(Resource.Id.IconFilter);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, FilterButton, IonIconsFonts.Options);
                FilterButton.Click += FilterButtonOnClick;

                MRecycler = (RecyclerView)view.FindViewById(Resource.Id.recyler);
                EmptyStateLayout = view.FindViewById<ViewStub>(Resource.Id.viewStub);

                SwipeRefreshLayout = (SwipeRefreshLayout)view.FindViewById(Resource.Id.swipeRefreshLayout);
                SwipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
                SwipeRefreshLayout.Refreshing = false;
                SwipeRefreshLayout.Enabled = true;
                SwipeRefreshLayout.SetProgressBackgroundColorSchemeColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#424242") : Color.ParseColor("#f7f7f7"));

                MAdView = view.FindViewById<AdView>(Resource.Id.adView);
                AdsGoogle.InitAdView(MAdView, MRecycler);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void SetRecyclerViewAdapters()
        {
            try
            {
                MAdapter = new UsersAdapter(Activity, MainContext, "Likes")
                {
                    UserList = new ObservableCollection<UserInfoObject>()
                };
                LayoutManager = new GridLayoutManager(Activity, 2);
                MRecycler.SetLayoutManager(LayoutManager);
                MRecycler.HasFixedSize = true;
                MRecycler.SetItemViewCacheSize(10);
                MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
                var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                var preLoader = new RecyclerViewPreloader<UserInfoObject>(Activity, MAdapter, sizeProvider, 10);
                MRecycler.AddOnScrollListener(preLoader);
                MRecycler.SetAdapter(MAdapter);

                RecyclerViewOnScrollListener xamarinRecyclerViewOnScrollListener = new RecyclerViewOnScrollListener(LayoutManager);
                MainScrollEvent = xamarinRecyclerViewOnScrollListener;
                MainScrollEvent.LoadMoreEvent += MainScrollEventOnLoadMoreEvent;
                MRecycler.AddOnScrollListener(xamarinRecyclerViewOnScrollListener);
                MainScrollEvent.IsLoading = false;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        private void BackIconOnClick(object sender, EventArgs e)
        {
            try
            {
                MainContext.FragmentBottomNavigator.OnBackStackClickFragment();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void SearchBoxOnAfterTextChanged(object sender, AfterTextChangedEventArgs e)
        {
            try
            {
                if (SearchBox.Text.Length > 0 && !string.IsNullOrWhiteSpace(SearchBox.Text))
                {
                    IconClose.Visibility = ViewStates.Visible;
                }
                else
                    IconClose.Visibility = ViewStates.Gone;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void IconCloseOnClick(object sender, EventArgs e)
        {
            try
            {
                SearchText = SearchBox.Text = "";

                SearchBox.ClearFocus();

                EmptyStateLayout.Visibility = ViewStates.Gone;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //show Filter
        private void FilterButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (AppSettings.ShowFilterBasic && !AppSettings.ShowFilterLooks && !AppSettings.ShowFilterBackground && !AppSettings.ShowFilterLifestyle && !AppSettings.ShowFilterMore)
                {
                    Bundle bundle = new Bundle();
                    bundle.PutString("TypePage", "SearchFragment");
                    var searchFilter = new SearchFilterBottomDialogFragment
                    {
                        Arguments = bundle
                    };

                    searchFilter.Show(Activity.SupportFragmentManager, "searchFilter");
                }
                else
                {
                    var intent = new Intent(Context, typeof(SearchFilterTabbedActivity));
                    intent.PutExtra("TypePage", "SearchFragment");
                    Context.StartActivity(intent);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
        //Scroll
        private void MainScrollEventOnLoadMoreEvent(object sender, EventArgs e)
        {
            try
            {
                //Code get last id where LoadMore >>
                var item = MAdapter.UserList.LastOrDefault();
                if (item != null && !string.IsNullOrEmpty(item.Id.ToString()) && !MainScrollEvent.IsLoading)
                {
                    CountOffset += 1;
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => LoadUsersAsync(CountOffset.ToString()) });
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void MAdapterOnItemClick(object sender, UsersAdapterClickEventArgs e)
        {
            try
            {
                var item = MAdapter.GetItem(e.Position);
                if (item != null)
                {
                    QuickDateTools.OpenProfile(Activity, "Close", item, e.Image);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void SwipeRefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                //Get Data Api
                MAdapter.UserList.Clear();
                MAdapter.NotifyDataSetChanged();

                MainScrollEvent.IsLoading = false;

                CountOffset = 0;
                StartApiService();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Load Data Api 

        private void StartApiService(string offset = "0")
        {
            if (!Methods.CheckConnectivity())
                Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => LoadUsersAsync(offset) });
        }

        public async Task LoadUsersAsync(string offset = "0")
        {
            if (MainScrollEvent != null && MainScrollEvent.IsLoading)
                return;

            if (Methods.CheckConnectivity())
            {
                if (MainScrollEvent != null)
                    MainScrollEvent.IsLoading = true;
                int countList = MAdapter.UserList.Count;

                if (UserDetails.Lat == "" && UserDetails.Lng == "")
                {
                    var locator = CrossGeolocator.Current;
                    locator.DesiredAccuracy = 50;
                    var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(10000));
                    Console.WriteLine("Position Status: {0}", position.Timestamp);
                    Console.WriteLine("Position Latitude: {0}", position.Latitude);
                    Console.WriteLine("Position Longitude: {0}", position.Longitude);

                    UserDetails.Lat = position.Latitude.ToString(CultureInfo.InvariantCulture);
                    UserDetails.Lng = position.Longitude.ToString(CultureInfo.InvariantCulture);
                }

                UserDetails.Location = ListUtils.MyUserInfo?.FirstOrDefault()?.Location;

                var dictionary = new Dictionary<string, string>
                {
                    {"limit", "20"},
                    {"offset", offset},
                    {"_gender", UserDetails.Gender},
                    {"_located", UserDetails.Located},
                    {"_location", UserDetails.Location},
                    {"_age_from", UserDetails.AgeMin.ToString()},
                    {"_age_to",  UserDetails.AgeMax.ToString()},
                    {"_lat", UserDetails.Lat},
                    {"_lng", UserDetails.Lng},
                    {"_body", UserDetails.Body ?? ""},
                    {"_ethnicity", UserDetails.Ethnicity ?? ""},
                    {"_religion", UserDetails.Religion ?? ""},
                    {"_drink", UserDetails.Drink ?? ""},
                    {"_smoke", UserDetails.Smoke ?? ""},
                    {"_education", UserDetails.Education ?? ""},
                    {"_pets", UserDetails.Pets ?? ""},
                    {"_relationship", UserDetails.RelationShip ?? ""},
                    {"_language", UserDetails.Language ?? UserDetails.FilterOptionLanguage},
                    {"_interest", UserDetails.Interest ?? ""},
                    {"_height_from", UserDetails.FromHeight ?? UserDetails.FilterOptionFromHeight},
                    {"_height_to", UserDetails.ToHeight ?? UserDetails.FilterOptionToHeight},
                    {"username", SearchText},
                };

                var (apiStatus, respond) = await RequestsAsync.Users.SearchAsync(dictionary);
                if (apiStatus != 200 || respond is not ListUsersObject result || result.Data == null)
                {
                    if (MainScrollEvent != null) MainScrollEvent.IsLoading = false;
                    Methods.DisplayReportResult(Activity, respond);
                }
                else
                {
                    var respondList = result.Data.Count;
                    if (respondList > 0)
                    {
                        foreach (var item in from item in result.Data let check = MAdapter.UserList.FirstOrDefault(a => a.Id == item.Id) where check == null select item)
                        {
                            if (UserDetails.SwitchState)
                            {
                                var online = QuickDateTools.GetStatusOnline(item.Lastseen, item.Online);
                                if (!online) continue;
                                MAdapter.UserList.Add(item);
                            }
                            else
                            {
                                MAdapter.UserList.Add(item);
                            }
                        }

                        if (countList > 0)
                        {
                            Activity?.RunOnUiThread(() => { MAdapter.NotifyItemRangeInserted(countList, MAdapter.UserList.Count - countList); });
                        }
                        else
                        {
                            Activity?.RunOnUiThread(() => { MAdapter.NotifyDataSetChanged(); });
                        }
                    }
                    else
                    {
                        if (MAdapter.UserList.Count > 10 && !MRecycler.CanScrollVertically(1))
                            Toast.MakeText(Activity, Activity.GetText(Resource.String.Lbl_NoMoreUsers), ToastLength.Short)?.Show();
                    }
                }

                Activity?.RunOnUiThread(ShowEmptyPage);
            }
            else
            {
                Inflated = EmptyStateLayout.Inflate();
                EmptyStateInflater x = new EmptyStateInflater();
                x.InflateLayout(Inflated, EmptyStateInflater.Type.NoConnection);
                if (!x.EmptyStateButton.HasOnClickListeners)
                {
                    x.EmptyStateButton.Click += null;
                    x.EmptyStateButton.Click += EmptyStateButtonOnClick;
                }

                Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                if (MainScrollEvent != null) MainScrollEvent.IsLoading = false;
            }
            if (MainScrollEvent != null) MainScrollEvent.IsLoading = false;
        }

        public void ShowEmptyPage()
        {
            try
            {
                MainScrollEvent.IsLoading = false;
                SwipeRefreshLayout.Refreshing = false;

                if (MAdapter.UserList.Count > 0)
                {
                    MRecycler.Visibility = ViewStates.Visible;
                    EmptyStateLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    MRecycler.Visibility = ViewStates.Gone;

                    Inflated ??= EmptyStateLayout.Inflate();

                    EmptyStateInflater x = new EmptyStateInflater();
                    x.InflateLayout(Inflated, EmptyStateInflater.Type.NoUsers);
                    if (!x.EmptyStateButton.HasOnClickListeners)
                    {
                        x.EmptyStateButton.Click += null;
                    }
                    EmptyStateLayout.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception e)
            {
                MainScrollEvent.IsLoading = false;
                SwipeRefreshLayout.Refreshing = false;
                Methods.DisplayReportResultTrack(e);
            }
        }

        //No Internet Connection 
        private void EmptyStateButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartApiService();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        private void HideKeyboard()
        {
            try
            {
                var inputManager = (InputMethodManager)Context?.GetSystemService(Android.Content.Context.InputMethodService);
                inputManager?.HideSoftInputFromWindow(SearchBox.WindowToken, HideSoftInputFlags.None);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void StartSearch()
        {
            try
            {
                if (string.IsNullOrEmpty(SearchText))
                    return;

                HideKeyboard();

                SwipeRefreshLayout.Refreshing = true;

                EmptyStateLayout.Visibility = ViewStates.Gone;
                MRecycler.Visibility = ViewStates.Visible;

                MAdapter.UserList.Clear();
                MAdapter.NotifyDataSetChanged();

                CountOffset = 0;
                StartApiService();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            if (actionId == ImeAction.Search)
            {
                try
                {
                    SearchText = v?.Text ?? "";

                    if (string.IsNullOrEmpty(SearchText))
                        return true;

                    SearchBox.ClearFocus();
                    v?.ClearFocus();

                    StartSearch();
                }
                catch (Exception exception)
                {
                    Methods.DisplayReportResultTrack(exception);
                }

                return true;
            }

            return false;
        }
    }
}
