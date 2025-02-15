using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.Content.Res;
using AndroidX.AppCompat.Widget;
using QuickDate.Activities.Base;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Authorization;
using QuickDateClient.Requests;
using System;
using System.Linq;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace QuickDate.Activities.Premium
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class PopularityActivity : BaseActivity
    {
        #region Variables Basic

        private Toolbar ActionBarToolBar;
        private TextView TxtVisitsCost, TxtMatchesCost, TxtLikesCost;
        private AppCompatButton BtnVisits, BtnMatches, BtnLikes;
        private HomeActivity GlobalContext;

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
                SetContentView(Resource.Layout.PopularityLayout);

                //Get Value And Set Toolbar
                InitComponent();
                InitBackground();
                InitToolbar();
                GetDataOption();

                GlobalContext = HomeActivity.GetInstance();
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
                TxtVisitsCost = FindViewById<TextView>(Resource.Id.visitsCost);
                TxtMatchesCost = FindViewById<TextView>(Resource.Id.matchesCost);
                TxtLikesCost = FindViewById<TextView>(Resource.Id.likesCost);

                BtnVisits = FindViewById<AppCompatButton>(Resource.Id.visitsButton);
                BtnMatches = FindViewById<AppCompatButton>(Resource.Id.matchesButton);
                BtnLikes = FindViewById<AppCompatButton>(Resource.Id.likesButton);
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
                ActionBarToolBar = (Toolbar)FindViewById(Resource.Id.toolbar);
                if (ActionBarToolBar != null)
                {
                    ActionBarToolBar.Title = GetText(Resource.String.Lbl_IncreasePopularity);
                    ActionBarToolBar.SetTitleTextColor(QuickDateTools.IsTabDark() ? AppSettings.TitleTextColorDark : AppSettings.TitleTextColor);
                    SetSupportActionBar(ActionBarToolBar);
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

        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    BtnVisits.Click += BtnVisitsOnClick;
                    BtnMatches.Click += BtnMatchesOnClick;
                    BtnLikes.Click += BtnLikesOnClick;
                }
                else
                {
                    BtnVisits.Click -= BtnVisitsOnClick;
                    BtnMatches.Click -= BtnMatchesOnClick;
                    BtnLikes.Click -= BtnLikesOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        //Likes
        private async void BtnLikesOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!AppSettings.EnableAppFree)
                {
                    var myBalance = ListUtils.MyUserInfo?.FirstOrDefault()?.Balance ?? "0.00";
                    if (!string.IsNullOrEmpty(myBalance) && myBalance != "0.00")
                    {
                        //Show a progress
                        AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));
                        //sent new api
                        var (apiStatus, respond) = await RequestsAsync.Users.ManagePopularityAsync("likes");
                        if (apiStatus == 200)
                        {
                            if (respond is AmountObject result)
                            {
                                myBalance = result.CreditAmount.ToString();

                                if (GlobalContext?.ProfileFragment.WalletNumber != null)
                                    GlobalContext.ProfileFragment.WalletNumber.Text = result.CreditAmount.ToString();

                                BtnLikes.Text = GetText(Resource.String.Lbl_IncreaseStarted);
                            }
                        }
                        else Methods.DisplayReportResult(this, respond);

                        AndHUD.Shared.Dismiss();
                    }
                    else
                    {
                        var window = new PopupController(this);
                        window.DisplayCreditWindow();
                    }
                }
                else
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));
                    //sent new api
                    var (apiStatus, respond) = await RequestsAsync.Users.ManagePopularityAsync("likes");
                    if (apiStatus == 200)
                    {
                        if (respond is AmountObject result)
                        {
                            //myBalance = result.CreditAmount.ToString();

                            if (GlobalContext?.ProfileFragment.WalletNumber != null)
                                GlobalContext.ProfileFragment.WalletNumber.Text = result.CreditAmount.ToString();

                            BtnLikes.Text = GetText(Resource.String.Lbl_IncreaseStarted);
                        }
                    }
                    else Methods.DisplayReportResult(this, respond);

                    AndHUD.Shared.Dismiss();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                AndHUD.Shared.Dismiss();
            }
        }

        //Matches
        private async void BtnMatchesOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!AppSettings.EnableAppFree)
                {
                    var myBalance = ListUtils.MyUserInfo?.FirstOrDefault()?.Balance ?? "0.00";
                    if (!string.IsNullOrEmpty(myBalance) && myBalance != "0.00")
                    {
                        //Show a progress
                        AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                        //sent new api
                        var (apiStatus, respond) = await RequestsAsync.Users.ManagePopularityAsync("matches");
                        if (apiStatus == 200)
                        {
                            if (respond is AmountObject result)
                            {
                                myBalance = result.CreditAmount.ToString();

                                if (GlobalContext?.ProfileFragment.WalletNumber != null)
                                    GlobalContext.ProfileFragment.WalletNumber.Text = result.CreditAmount.ToString();

                                BtnMatches.Text = GetText(Resource.String.Lbl_IncreaseStarted);
                            }
                        }
                        else Methods.DisplayReportResult(this, respond);

                        AndHUD.Shared.Dismiss();
                    }
                    else
                    {
                        var window = new PopupController(this);
                        window.DisplayCreditWindow();
                    }
                }
                else
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                    //sent new api
                    var (apiStatus, respond) = await RequestsAsync.Users.ManagePopularityAsync("matches");
                    if (apiStatus == 200)
                    {
                        if (respond is AmountObject result)
                        {
                            //myBalance = result.CreditAmount.ToString();

                            if (GlobalContext?.ProfileFragment.WalletNumber != null)
                                GlobalContext.ProfileFragment.WalletNumber.Text = result.CreditAmount.ToString();

                            BtnMatches.Text = GetText(Resource.String.Lbl_IncreaseStarted);
                        }
                    }
                    else Methods.DisplayReportResult(this, respond);

                    AndHUD.Shared.Dismiss();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                AndHUD.Shared.Dismiss();
            }
        }

        //Visits
        private async void BtnVisitsOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!AppSettings.EnableAppFree)
                {
                    var myBalance = ListUtils.MyUserInfo?.FirstOrDefault()?.Balance ?? "0.00";
                    if (!string.IsNullOrEmpty(myBalance) && myBalance != "0.00")
                    {
                        //Show a progress
                        AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                        //sent new api
                        var (apiStatus, respond) = await RequestsAsync.Users.ManagePopularityAsync("visits");
                        if (apiStatus == 200)
                        {
                            if (respond is AmountObject result)
                            {
                                myBalance = result.CreditAmount.ToString();

                                if (GlobalContext?.ProfileFragment != null)
                                    GlobalContext.ProfileFragment.WalletNumber.Text = result.CreditAmount.ToString();

                                BtnVisits.Text = GetText(Resource.String.Lbl_IncreaseStarted);
                            }
                        }
                        else Methods.DisplayReportResult(this, respond);

                        AndHUD.Shared.Dismiss();
                    }
                    else
                    {
                        var window = new PopupController(this);
                        window.DisplayCreditWindow();
                    }
                }
                else
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                    //sent new api
                    var (apiStatus, respond) = await RequestsAsync.Users.ManagePopularityAsync("visits");
                    if (apiStatus == 200)
                    {
                        if (respond is AmountObject result)
                        {
                            //myBalance = result.CreditAmount.ToString();

                            if (GlobalContext?.ProfileFragment != null)
                                GlobalContext.ProfileFragment.WalletNumber.Text = result.CreditAmount.ToString();

                            BtnVisits.Text = GetText(Resource.String.Lbl_IncreaseStarted);
                        }
                    }
                    else Methods.DisplayReportResult(this, respond);

                    AndHUD.Shared.Dismiss();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                AndHUD.Shared.Dismiss();
            }
        }

        #endregion

        private void GetDataOption()
        {
            try
            {
                var option = ListUtils.SettingsSiteList;
                if (option != null)
                {
                    TxtVisitsCost.Text = option.CostPerXvisits + " " + GetText(Resource.String.Lbl_Credits);
                    TxtMatchesCost.Text = option.CostPerXmatche + " " + GetText(Resource.String.Lbl_Credits);
                    TxtLikesCost.Text = option.CostPerXlike + " " + GetText(Resource.String.Lbl_Credits);
                }

                if (ListUtils.MyUserInfo.Count == 0)
                {
                    var sqlEntity = new SqLiteDatabase();
                    sqlEntity.GetDataMyInfo();

                }

                var data = ListUtils.MyUserInfo?.FirstOrDefault();
                if (data != null)
                {
                    int xLikes = Convert.ToInt32(data.XlikesCreatedAt);
                    int xMatches = Convert.ToInt32(data.XmatchesCreatedAt);
                    int xVisits = Convert.ToInt32(data.XvisitsCreatedAt);
                    if (xLikes != 0)
                    {
                        BtnLikes.Text = GetText(Resource.String.Lbl_IncreaseStarted);
                    }
                    else if (xMatches != 0)
                    {
                        BtnMatches.Text = GetText(Resource.String.Lbl_IncreaseStarted);
                    }
                    else if (xVisits != 0)
                    {
                        BtnVisits.Text = GetText(Resource.String.Lbl_IncreaseStarted);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

    }
}