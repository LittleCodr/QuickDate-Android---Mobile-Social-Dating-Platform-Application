using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.Content.Res;
using AndroidX.ViewPager2.Widget;
using Google.Android.Material.Tabs;
using QuickDate.Activities.Base;
using QuickDate.Activities.SearchFilter.Fragment;
using QuickDate.Activities.Tabbes;
using QuickDate.Adapters;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace QuickDate.Activities.SearchFilter
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class SearchFilterTabbedActivity : BaseActivity, TabLayoutMediator.ITabConfigurationStrategy
    {
        #region Variables Basic

        private MainTabAdapter Adapter;
        private ViewPager2 ViewPager;
        private TabLayout TabLayout;

        private FilterBackgroundFragment BackgroundTab;
        private BasicFragment BasicTab;
        private LooksFragment LooksTab;
        private MoreFragment MoreTab;
        private LifestyleFragment LifestyleTab;
        private static SearchFilterTabbedActivity Instance;
        private string TypePage = "";

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);
                SetTheme(QuickDateTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);

                // Create your application here
                SetContentView(Resource.Layout.SearchFilterTabbedLayout);

                Instance = this;
                TypePage = Intent?.GetStringExtra("TypePage") ?? "";

                LoadFilterOptionsData();

                //Get Value And Set Toolbar
                InitComponent();
                InitBackground();
                InitToolbar();

                AdsGoogle.Ad_RewardedInterstitial(this);
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

        protected override void OnDestroy()
        {
            try
            {
                Instance = null;
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                ViewPager = FindViewById<ViewPager2>(Resource.Id.viewpager);
                TabLayout = FindViewById<TabLayout>(Resource.Id.tabs);

                SetUpViewPager(ViewPager);
                new TabLayoutMediator(TabLayout, ViewPager, this).Attach();

                TabLayout.SetTabTextColors(QuickDateTools.IsTabDark() ? Color.White : Color.ParseColor("#131313"), Color.ParseColor(AppSettings.MainColor));

            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void InitToolbar()
        {
            try
            {
                var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    toolbar.Title = GetString(Resource.String.Lbl_Filter);
                    toolbar.SetTitleTextColor(QuickDateTools.IsTabDark() ? Color.White : Color.ParseColor("#131313"));
                    SetSupportActionBar(toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);

                    var icon = AppCompatResources.GetDrawable(this, AppSettings.FlowDirectionRightToLeft ? Resource.Drawable.icon_back_arrow_right : Resource.Drawable.icon_back_arrow_left);
                    icon?.SetTint(QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                    SupportActionBar.SetHomeAsUpIndicator(icon);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static SearchFilterTabbedActivity GetInstance()
        {
            try
            {
                return Instance;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        #endregion

        #region Set Tab

        private void SetUpViewPager(ViewPager2 viewPager)
        {
            try
            {
                Adapter = new MainTabAdapter(this);

                if (AppSettings.ShowFilterBasic)
                {
                    BasicTab = new BasicFragment();
                    Adapter.AddFragment(BasicTab, GetText(Resource.String.Lbl_Basics));
                }

                if (AppSettings.ShowFilterLooks)
                {
                    LooksTab = new LooksFragment();
                    Adapter.AddFragment(LooksTab, GetText(Resource.String.Lbl_Looks));
                }

                if (AppSettings.ShowFilterBackground)
                {
                    BackgroundTab = new FilterBackgroundFragment();

                    Adapter.AddFragment(BackgroundTab, GetText(Resource.String.Lbl_Background));
                }
                if (AppSettings.ShowFilterLifestyle)
                {
                    LifestyleTab = new LifestyleFragment();
                    Adapter.AddFragment(LifestyleTab, GetText(Resource.String.Lbl_Lifestyle));
                }

                if (AppSettings.ShowFilterMore)
                {
                    MoreTab = new MoreFragment();
                    Adapter.AddFragment(MoreTab, GetText(Resource.String.Lbl_More));
                }

                ViewPager.OffscreenPageLimit = Adapter.ItemCount;
                viewPager.CurrentItem = Adapter.ItemCount;
                viewPager.Orientation = ViewPager2.OrientationHorizontal;
                viewPager.Adapter = Adapter;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }


        public void OnConfigureTab(TabLayout.Tab tab, int position)
        {
            try
            {
                tab.SetText(Adapter.GetFragment(position));
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion Set Tab

        #region Event

        public void ActionButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                // check current state of a Switch (true or false).
                if (BasicTab != null)
                {
                    var sliderValues = BasicTab.AgeSeekBar.Values;

                    UserDetails.AgeMin = BasicTab.AgeMin = sliderValues.FirstOrDefault()?.IntValue() ?? 0;
                    UserDetails.AgeMax = BasicTab.AgeMax = sliderValues.LastOrDefault()?.IntValue() ?? 0;

                    UserDetails.Gender = BasicTab.Gender;
                    UserDetails.SwitchState = BasicTab.SwitchState;
                    UserDetails.Located = BasicTab.DistanceCount.ToString();
                }

                if (BackgroundTab != null)
                {
                    UserDetails.Language = BackgroundTab.Language;
                    UserDetails.Ethnicity = BackgroundTab.IdEthnicity.ToString();
                    UserDetails.Religion = BackgroundTab.IdReligion.ToString();
                }

                if (LifestyleTab != null)
                {
                    UserDetails.RelationShip = LifestyleTab.IdRelationShip.ToString();
                    UserDetails.Smoke = LifestyleTab.IdSmoke.ToString();
                    UserDetails.Drink = LifestyleTab.IdDrink.ToString();
                }

                if (LooksTab != null)
                {
                    UserDetails.Body = LooksTab.IdBody.ToString();
                    UserDetails.FromHeight = LooksTab.FromHeight;
                    UserDetails.ToHeight = LooksTab.ToHeight;
                }

                if (MoreTab != null)
                {
                    UserDetails.Interest = MoreTab.Interest;
                    UserDetails.Education = MoreTab.IdEducation.ToString();
                    UserDetails.Pets = MoreTab.IdPets.ToString();
                }

                SaveFilterOptions();

                var mainContext = HomeActivity.GetInstance();
                if (TypePage == "TrendingFragment")
                {
                    if (mainContext.TrendingFragment?.MAdapter != null)
                    {
                        var checkList = mainContext.TrendingFragment.MAdapter?.TrendingList?.Where(q => q.Type == ItemType.Users).ToList();
                        if (checkList?.Count > 0)
                        {
                            mainContext.TrendingFragment.MAdapter.TrendingList.RemoveAll(checkList);
                            mainContext.TrendingFragment.MAdapter.NotifyDataSetChanged();
                        }

                        var emptyStateChecker = mainContext.TrendingFragment.MAdapter?.TrendingList?.FirstOrDefault(a => a.Type == ItemType.EmptyPage);
                        if (emptyStateChecker != null)
                        {
                            mainContext.TrendingFragment.MAdapter.TrendingList.Remove(emptyStateChecker);
                            mainContext.TrendingFragment.MAdapter.NotifyDataSetChanged();
                        }

                        if (mainContext.TrendingFragment.MainScrollEvent != null)
                            mainContext.TrendingFragment.MainScrollEvent.IsLoading = false;

                        mainContext.TrendingFragment.SwipeRefreshLayout.Refreshing = true;
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => mainContext.TrendingFragment.LoadUsersAsync() });
                    }
                }
                else if (TypePage == "SearchFragment")
                {
                    if (mainContext.TrendingFragment?.SearchFragment?.MAdapter != null)
                    {
                        mainContext.TrendingFragment?.SearchFragment?.StartSearch();
                    }
                }

                Finish();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        private void LoadFilterOptionsData()
        {
            try
            {
                var dbDatabase = new SqLiteDatabase();
                dbDatabase.Get_data_Filter_Options();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void SaveFilterOptions()
        {
            try
            {
                var filterOptions = new DataTables.FilterOptionsTb
                {
                    AgeMin = UserDetails.AgeMin,
                    AgeMax = UserDetails.AgeMax,
                    Gender = UserDetails.Gender,
                    IsOnline = UserDetails.SwitchState,
                    Distance = UserDetails.Located,
                    Language = UserDetails.Language,
                    Ethnicity = UserDetails.Ethnicity,
                    Religion = UserDetails.Religion,
                    RelationShip = UserDetails.RelationShip,
                    Smoke = UserDetails.Smoke,
                    Drink = UserDetails.Drink,
                    Body = UserDetails.Body,
                    FromHeight = UserDetails.FromHeight,
                    ToHeight = UserDetails.ToHeight,
                    Interest = UserDetails.Interest,
                    Education = UserDetails.Education,
                    Pets = UserDetails.Pets
                };

                var dbDatabase = new SqLiteDatabase();
                dbDatabase.InsertOrUpdateFilter_Options(filterOptions);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void ResetAllFilters(string page)
        {
            try
            {
                if (page == "BasicTab")
                {
                    UserDetails.AgeMin = UserDetails.FilterOptionAgeMin;
                    UserDetails.AgeMax = UserDetails.FilterOptionAgeMax;
                    UserDetails.Gender = UserDetails.FilterOptionGender;
                    UserDetails.Location = "";
                    UserDetails.SwitchState = UserDetails.FilterOptionIsOnline;
                    UserDetails.Located = UserDetails.FilterOptionDistance;

                    BasicTab?.SetLocalData();
                }

                if (page == "BackgroundTab")
                {
                    UserDetails.Language = UserDetails.FilterOptionLanguage;
                    UserDetails.Ethnicity = "";
                    UserDetails.Religion = "";

                    BackgroundTab?.SetLocalData();
                }

                if (page == "LifestyleTab")
                {
                    UserDetails.RelationShip = "";
                    UserDetails.Smoke = "";
                    UserDetails.Drink = "";


                    LifestyleTab?.SetLocalData();
                }

                if (page == "LooksTab")
                {
                    UserDetails.Body = "";
                    UserDetails.FromHeight = UserDetails.FilterOptionFromHeight;
                    UserDetails.ToHeight = UserDetails.FilterOptionToHeight;

                    LooksTab?.SetLocalData();
                }

                if (page == "MoreTab")
                {
                    UserDetails.Interest = "";
                    UserDetails.Education = "";
                    UserDetails.Pets = "";

                    MoreTab?.SetLocalData();
                }

                SaveFilterOptions();
                LoadFilterOptionsData();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

    }
}