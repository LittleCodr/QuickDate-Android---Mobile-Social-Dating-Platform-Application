using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Bumptech.Glide.Util;
using Com.Google.Android.Gms.Ads;
using QuickDate.Activities.HotOrNot.Adapters;
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
using System.Linq;
using System.Threading.Tasks;
using Exception = System.Exception;
using Fragment = AndroidX.Fragment.App.Fragment;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace QuickDate.Activities.HotOrNot
{
    public class HotOrNotFragment : Fragment
    {
        #region Variables Basic

        public HotOrNotUserAdapter MAdapter;
        private SwipeRefreshLayout SwipeRefreshLayout;
        private RecyclerView MRecycler;
        private LinearLayoutManager LayoutManager;
        private ViewStub EmptyStateLayout;
        private View Inflated;
        private HomeActivity MainContext;
        private AdView MAdView;
        private RecyclerViewOnScrollListener MainScrollEvent;
        private TextView ActionButton;


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
                View view = inflater.Inflate(Resource.Layout.RecyclerDefaultLayout, container, false);
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
                InitToolbar(view);
                SetRecyclerViewAdapters();

                SwipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;

                ActionButton.Click += ActionButtonOnClick;

                //Get Data  
                GetData();

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
                MRecycler = (RecyclerView)view.FindViewById(Resource.Id.recyler);
                EmptyStateLayout = view.FindViewById<ViewStub>(Resource.Id.viewStub);

                SwipeRefreshLayout = (SwipeRefreshLayout)view.FindViewById(Resource.Id.swipeRefreshLayout);
                SwipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
                SwipeRefreshLayout.Refreshing = true;
                SwipeRefreshLayout.Enabled = true;
                SwipeRefreshLayout.SetProgressBackgroundColorSchemeColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#424242") : Color.ParseColor("#f7f7f7"));
                SwipeRefreshLayout.SetPadding(0, 0, 0, 0);

                ActionButton = (TextView)view.FindViewById(Resource.Id.toolbar_title);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ActionButton, IonIconsFonts.Options);
                ActionButton.SetTextColor(AppSettings.TitleTextColor);
                ActionButton.SetTextSize(ComplexUnitType.Sp, 20f);
                ActionButton.Visibility = ViewStates.Visible;

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
                MAdapter = new HotOrNotUserAdapter(Activity)
                {
                    UsersDateList = new ObservableCollection<UserInfoObject>()
                };

                MAdapter.HotItemClick += MAdapterOnHotItemClick;
                MAdapter.NotItemClick += MAdapterOnNotItemClick;
                MAdapter.ImageItemClick += MAdapterOnImageItemClick;

                LayoutManager = new LinearLayoutManager(Context);
                //LayoutManager.SetSpanSizeLookup(new MySpanSizeLookup(4, 1, 1)); //5, 1, 2 
                MRecycler.SetLayoutManager(LayoutManager);
                //MRecycler.HasFixedSize = true;
                //MRecycler.SetItemViewCacheSize(10);
                //MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
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

        private void InitToolbar(View view)
        {
            try
            {
                var toolbar = view.FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    string title = Context.GetString(Resource.String.Lbl_HotORNot);
                    MainContext.SetToolBar(toolbar, title, true);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        private void MAdapterOnHotItemClick(object sender, HotOrNotUserAdapterClickEventArgs e)
        {
            try
            {
                var item = MAdapter.GetItem(e.Position);
                if (item != null)
                {
                    if (Methods.CheckConnectivity())
                    {
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.AddHotAsync(item.Id.ToString()) });

                        var index = MAdapter.UsersDateList.IndexOf(item);
                        if (index != -1)
                        {
                            MAdapter.UsersDateList.RemoveAt(index);
                            MAdapter.NotifyItemRemoved(index);
                        }
                    }
                    else
                    {
                        Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void MAdapterOnNotItemClick(object sender, HotOrNotUserAdapterClickEventArgs e)
        {
            try
            {
                var item = MAdapter.GetItem(e.Position);
                if (item != null)
                {
                    if (Methods.CheckConnectivity())
                    {
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.SetNotAsync(item.Id.ToString()) });

                        var index = MAdapter.UsersDateList.IndexOf(item);
                        if (index != -1)
                        {
                            MAdapter.UsersDateList.RemoveAt(index);
                            MAdapter.NotifyItemRemoved(index);
                        }
                    }
                    else
                    {
                        Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Open profile user  >> Hot Or NotL
        private void MAdapterOnImageItemClick(object sender, HotOrNotUserAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position > -1)
                {
                    var item = MAdapter.GetItem(e.Position);
                    if (item != null)
                    {
                        QuickDateTools.OpenProfile(Activity, "Close", item, e.Image);
                    }
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
                MAdapter.UsersDateList.Clear();
                MAdapter.NotifyDataSetChanged();

                MainScrollEvent.IsLoading = false;

                StartApiService();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void MainScrollEventOnLoadMoreEvent(object sender, EventArgs e)
        {
            try
            {
                //Event Scroll #LastChat
                var item = MAdapter.UsersDateList.LastOrDefault();
                if (item != null && MAdapter.UsersDateList.Count > 10 && !MainScrollEvent.IsLoading)
                    StartApiService(item.Id.ToString());
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Get User By >> filter
        private void ActionButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                UsersFilterBottomDialogFragment bottomDialogFragment = new UsersFilterBottomDialogFragment("HotOrNot");
                bottomDialogFragment.Show(ChildFragmentManager, bottomDialogFragment.Tag);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Load Data Api 

        private void GetData()
        {
            try
            {
                var checkList = MainContext?.TrendingFragment?.MAdapter?.TrendingList?.FirstOrDefault(q => q.Type == ItemType.HotOrNot);
                if (checkList?.HotOrNotList.Count > 0)
                {
                    MAdapter.UsersDateList = new ObservableCollection<UserInfoObject>(checkList.HotOrNotList);
                    MAdapter.NotifyDataSetChanged();

                    SwipeRefreshLayout.Refreshing = false;
                }

                var lastId = MAdapter.UsersDateList.LastOrDefault()?.Id ?? 0;
                StartApiService(lastId.ToString());
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void StartApiService(string offset = "0")
        {
            if (!Methods.CheckConnectivity())
                Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => LoadHotOrNotAsync(offset) });
        }

        public async Task LoadHotOrNotAsync(string offset = "0")
        {
            if (MainScrollEvent.IsLoading)
                return;

            if (Methods.CheckConnectivity())
            {
                MainScrollEvent.IsLoading = true;
                int countList = MAdapter.UsersDateList.Count;
                var dictionary = new Dictionary<string, string>
                {
                    {"limit", "20"},
                    {"offset", offset},
                    {"genders", UserDetails.HotOrNotFilterGender},
                    {"online", UserDetails.HotOrNotFilterIsOnline ? "1" : "0"},
                    {"birthday", UserDetails.HotOrNotFilterBirthday},
                    //{"lat", UserDetails.Lat},
                    //{"lng", UserDetails.Lng},
                };

                var (apiStatus, respond) = await RequestsAsync.Users.HotOrNotAsync(dictionary);
                if (apiStatus != 200 || respond is not ListUsersObject result || result.Data == null)
                {
                    MainScrollEvent.IsLoading = false;
                    Methods.DisplayReportResult(Activity, respond);
                }
                else
                {
                    var respondList = result.Data.Count;
                    if (respondList > 0)
                    {
                        foreach (var item in from item in result.Data let check = MAdapter.UsersDateList.FirstOrDefault(a => a.Id == item.Id) where check == null select item)
                        {
                            MAdapter.UsersDateList.Add(item);
                        }

                        if (countList > 0)
                            Activity?.RunOnUiThread(() => MAdapter.NotifyItemRangeInserted(countList, MAdapter.UsersDateList.Count - countList));
                        else
                            Activity?.RunOnUiThread(() => MAdapter.NotifyDataSetChanged());
                    }
                    else
                    {
                        if (MAdapter.UsersDateList.Count > 10 && !MRecycler.CanScrollVertically(1))
                            Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_NoMoreUsers), ToastLength.Short)?.Show();
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
                MainScrollEvent.IsLoading = false;
            }
            MainScrollEvent.IsLoading = false;
        }

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

        private void ShowEmptyPage()
        {
            try
            {
                MainScrollEvent.IsLoading = false;
                SwipeRefreshLayout.Refreshing = false;

                if (MAdapter.UsersDateList.Count > 0)
                {
                    MRecycler.Visibility = ViewStates.Visible;
                    EmptyStateLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    MRecycler.Visibility = ViewStates.Gone;

                    if (Inflated == null)
                        Inflated = EmptyStateLayout.Inflate();

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

        #endregion

        public void ApplyFilter()
        {
            try
            {
                MAdapter.UsersDateList.Clear();
                MAdapter.NotifyDataSetChanged();

                EmptyStateLayout.Visibility = ViewStates.Gone;

                StartApiService();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }


    }
}