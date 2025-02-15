using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Google.Android.Gms.Ads.Admanager;
using ME.Itangqi.Waveloadingview;
using QuickDate.Activities.Blogs;
using QuickDate.Activities.Favorite;
using QuickDate.Activities.Friends;
using QuickDate.Activities.IDisliked;
using QuickDate.Activities.ILiked;
using QuickDate.Activities.InviteFriends;
using QuickDate.Activities.Likes;
using QuickDate.Activities.Live.Page;
using QuickDate.Activities.MyProfile;
using QuickDate.Activities.Premium;
using QuickDate.Activities.SettingsUser;
using QuickDate.Activities.Visits;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.Library.Anjo.Share;
using QuickDate.Library.Anjo.Share.Abstractions;
using QuickDate.SQLite;
using QuickDateClient;
using QuickDateClient.Classes.Authorization;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using System;
using System.Globalization;
using System.Linq;

namespace QuickDate.Activities.Tabbes.Fragment
{
    public class ProfileFragment : AndroidX.Fragment.App.Fragment
    {
        #region Variables Basic

        private HomeActivity GlobalContext;
        private TextView Username, Location, XtBoostMe, TxtLiked, TxtVisits;
        public TextView WalletNumber;
        public ImageView ProfileImage;
        private FrameLayout BoostButton;
        private LinearLayout WalletButton, locationSection, EditButton, FavoriteButton, HelpButton, InviteButton, BlogsButton, FriendsButton, LikedButton, DislikedButton, ShareButton, LikesButton, LiveButton, VisitsButton;
        private ImageView PopularityButton, UpgradeButton, SettingsButton;
        public FavoriteUserFragment FavoriteFragment;
        public FriendsFragment FriendsFragment;
        public LikedFragment LikedFragment;
        public DislikedFragment DislikedFragment;
        public TimerTime Time;
        private WaveLoadingView MWaveLoadingView;
        private AdManagerAdView AdManagerAdView;
        private string TxtUpgrade;
        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
            GlobalContext = HomeActivity.GetInstance();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View view = inflater.Inflate(Resource.Layout.TProfileLayout, container, false);
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

                //Get Value 
                InitComponent(view);

                AdManagerAdView = view.FindViewById<AdManagerAdView>(Resource.Id.multiple_ad_sizes_view);
                AdsGoogle.InitAdManagerAdView(AdManagerAdView);

                GetMyInfoData();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }


        public override void OnResume()
        {
            try
            {
                base.OnResume();
                AddOrRemoveEvent(true);
                AdsGoogle.LifecycleAdManagerAdView(AdManagerAdView, "Resume");
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
                base.OnPause();
                AddOrRemoveEvent(false);
                AdsGoogle.LifecycleAdManagerAdView(AdManagerAdView, "Pause");
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
                AdsGoogle.LifecycleAdManagerAdView(AdManagerAdView, "Destroy");
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
                var topLayout = view.FindViewById(Resource.Id.TopLayout);
                QuickDateTools.SetBackgroundTheme(topLayout);

                WalletButton = view.FindViewById<LinearLayout>(Resource.Id.walletSection);
                WalletNumber = view.FindViewById<TextView>(Resource.Id.walletnumber);

                PopularityButton = view.FindViewById<ImageView>(Resource.Id.popularitySection);
                UpgradeButton = view.FindViewById<ImageView>(Resource.Id.upgradeSection);
                SettingsButton = view.FindViewById<ImageView>(Resource.Id.SettingsButton);

                ProfileImage = view.FindViewById<ImageView>(Resource.Id.ImageUser);

                BoostButton = view.FindViewById<FrameLayout>(Resource.Id.BoostButton);
                BoostButton.Tag = "Off";
                XtBoostMe = view.FindViewById<TextView>(Resource.Id.tv_Boost);

                MWaveLoadingView = (WaveLoadingView)view.FindViewById(Resource.Id.waveLoadingView);
                MWaveLoadingView.Visibility = ViewStates.Gone;

                Username = view.FindViewById<TextView>(Resource.Id.username);
                Location = view.FindViewById<TextView>(Resource.Id.location);
                locationSection = view.FindViewById<LinearLayout>(Resource.Id.locationSection);
                EditButton = view.FindViewById<LinearLayout>(Resource.Id.btnEditProfile);

                LikesButton = view.FindViewById<LinearLayout>(Resource.Id.LikesSection);
                TxtLiked = view.FindViewById<TextView>(Resource.Id.LikesText);

                VisitsButton = view.FindViewById<LinearLayout>(Resource.Id.VisitsSection);
                TxtVisits = view.FindViewById<TextView>(Resource.Id.VisitsText);

                ShareButton = view.FindViewById<LinearLayout>(Resource.Id.ShareSection);

                LiveButton = view.FindViewById<LinearLayout>(Resource.Id.LiveLayout);
                FriendsButton = view.FindViewById<LinearLayout>(Resource.Id.FriendsLayout);
                FavoriteButton = view.FindViewById<LinearLayout>(Resource.Id.FavoritesLayout);
                LikedButton = view.FindViewById<LinearLayout>(Resource.Id.PeopleILikedLayout);
                DislikedButton = view.FindViewById<LinearLayout>(Resource.Id.PeopleIDislikeLayout);
                BlogsButton = view.FindViewById<LinearLayout>(Resource.Id.BlogsLayout);
                InviteButton = view.FindViewById<LinearLayout>(Resource.Id.InviteFriendsLayout);
                HelpButton = view.FindViewById<LinearLayout>(Resource.Id.HelpLayout);


                GlideImageLoader.LoadImage(Activity, UserDetails.Avatar, ProfileImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                if (AppSettings.EnableAppFree)
                {
                    WalletButton.Visibility = ViewStates.Invisible;
                    UpgradeButton.Visibility = ViewStates.Invisible;
                }

                if (!AppSettings.ShowLive)
                    LiveButton.Visibility = ViewStates.Gone;

                if (!AppSettings.EnableFriendSystem)
                    FriendsButton.Visibility = ViewStates.Gone;

                if (!AppSettings.PeopleILikedEnabled)
                    LikedButton.Visibility = ViewStates.Gone;

                if (!AppSettings.PeopleIDislikedEnabled)
                    DislikedButton.Visibility = ViewStates.Gone;

                if (!AppSettings.FavoriteEnabled)
                    FavoriteButton.Visibility = ViewStates.Gone;

                if (!AppSettings.BlogsEnabled)
                    BlogsButton.Visibility = ViewStates.Gone;

                if (!AppSettings.PremiumSystemEnabled)
                {
                    UpgradeButton.Visibility = ViewStates.Invisible;
                    UpgradeButton.Enabled = false;
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
                    WalletButton.Click += WalletButtonOnClick;
                    PopularityButton.Click += PopularityButtonOnClick;
                    UpgradeButton.Click += UpgradeButtonOnClick;
                    UpgradeButton.LongClick += UpgradeButtonOnLongClick;
                    EditButton.Click += EditButtonOnClick;
                    ProfileImage.Click += ProfileImageOnClick;
                    SettingsButton.Click += SettingsButtonOnClick;
                    FavoriteButton.Click += FavoriteButtonOnClick;
                    HelpButton.Click += HelpButtonOnClick;
                    InviteButton.Click += InviteButtonOnClick;
                    BlogsButton.Click += BlogsButtonOnClick;
                    BoostButton.Click += BoostButtonOnClick;
                    FriendsButton.Click += FriendsButtonOnClick;
                    LikedButton.Click += ILikedButtonOnClick;
                    DislikedButton.Click += DislikedButtonOnClick;
                    ShareButton.Click += ShareButtonOnClick;
                    LikesButton.Click += LikesButtonOnClick;
                    LiveButton.Click += LiveButtonOnClick;
                    VisitsButton.Click += VisitsButtonOnClick;
                }
                else
                {
                    WalletButton.Click -= WalletButtonOnClick;
                    PopularityButton.Click -= PopularityButtonOnClick;
                    UpgradeButton.Click -= UpgradeButtonOnClick;
                    UpgradeButton.LongClick -= UpgradeButtonOnLongClick;
                    EditButton.Click -= EditButtonOnClick;
                    ProfileImage.Click -= ProfileImageOnClick;
                    SettingsButton.Click -= SettingsButtonOnClick;
                    FavoriteButton.Click -= FavoriteButtonOnClick;
                    HelpButton.Click -= HelpButtonOnClick;
                    InviteButton.Click -= InviteButtonOnClick;
                    BlogsButton.Click -= BlogsButtonOnClick;
                    BoostButton.Click -= BoostButtonOnClick;
                    FriendsButton.Click -= FriendsButtonOnClick;
                    LikedButton.Click -= ILikedButtonOnClick;
                    DislikedButton.Click -= DislikedButtonOnClick;
                    ShareButton.Click -= ShareButtonOnClick;
                    LikesButton.Click -= LikesButtonOnClick;
                    LiveButton.Click -= LiveButtonOnClick;
                    VisitsButton.Click -= VisitsButtonOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void UpgradeButtonOnLongClick(object sender, View.LongClickEventArgs e)
        {
            Toast.MakeText(Context, TxtUpgrade, ToastLength.Short)?.Show();
        }

        #endregion

        #region Events

        private void VisitsButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                VisitsFragment visitsFragment = new VisitsFragment();
                GlobalContext.FragmentBottomNavigator.DisplayFragment(visitsFragment);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void LiveButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                LiveFragment liveFragment = new LiveFragment();
                GlobalContext.FragmentBottomNavigator.DisplayFragment(liveFragment);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void LikesButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var isPro = ListUtils.MyUserInfo?.FirstOrDefault()?.IsPro ?? "0";
                if (!AppSettings.EnableAppFree)
                {
                    if (isPro == "0")
                    {
                        var window = new PopupController(Activity);
                        window.DisplayPremiumWindow();
                    }
                    else
                    {
                        LikesFragment likesFragment = new LikesFragment();
                        GlobalContext.FragmentBottomNavigator.DisplayFragment(likesFragment);
                    }
                }
                else
                {
                    LikesFragment likesFragment = new LikesFragment();
                    GlobalContext.FragmentBottomNavigator.DisplayFragment(likesFragment);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Share url profile
        private async void ShareButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var dataUser = ListUtils.MyUserInfo?.FirstOrDefault();

                //Share Plugin same as video
                if (!CrossShare.IsSupported || dataUser == null) return;

                await CrossShare.Current.Share(new ShareMessage
                {
                    Title = InitializeQuickDate.WebsiteUrl + "/@" + dataUser.Username,
                    Text = GetText(Resource.String.Lbl_Check) + " " + QuickDateTools.GetNameFinal(dataUser) + " " + GetText(Resource.String.Lbl_ProfileOn) + " " + AppSettings.ApplicationName + "!",
                    Url = InitializeQuickDate.WebsiteUrl + "/@" + dataUser.Username
                });
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Update Avatar Async
        private void ProfileImageOnClick(object sender, EventArgs e)
        {
            try
            {
                GlobalContext.TypeAvatar = "Avatar";
                PixImagePickerUtils.OpenDialogGallery(GlobalContext);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }


        //Open I Disliked User 
        private void DislikedButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                DislikedFragment = new DislikedFragment();
                GlobalContext.FragmentBottomNavigator.DisplayFragment(DislikedFragment);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }


        //Open I Liked User 
        private void ILikedButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                LikedFragment = new LikedFragment();
                GlobalContext.FragmentBottomNavigator.DisplayFragment(LikedFragment);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }


        //Open Blogs
        private void BlogsButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(Context, typeof(BlogsActivity)));
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Open Friend 
        private void FriendsButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                FriendsFragment = new FriendsFragment();
                GlobalContext.FragmentBottomNavigator.DisplayFragment(FriendsFragment);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Open edit my info
        private void EditButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(Activity, typeof(EditProfileActivity)));
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Open Upgrade
        private void UpgradeButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var dataUser = ListUtils.MyUserInfo?.FirstOrDefault();
                if (dataUser?.IsPro == "1")
                {
                    var window = new DialogController(Activity);
                    window.OpenDialogYouArePremium(dataUser.ProTime, dataUser.ProType);
                }
                else
                {
                    var window = new PopupController(Activity);
                    window.DisplayPremiumWindow();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Open Popularity >> Very Low
        private void PopularityButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(Context, typeof(PopularityActivity)));
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Open Wallet
        private void WalletButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var window = new PopupController(Activity);
                window.DisplayCreditWindow();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Boost me
        private async void BoostButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var dataUser = ListUtils.MyUserInfo?.FirstOrDefault();
                if (dataUser != null)
                {
                    if (BoostButton?.Tag?.ToString() == "Off")
                    {
                        if (!AppSettings.EnableAppFree)
                        {
                            string myBalance = dataUser.Balance ?? "0.00";
                            if (!string.IsNullOrEmpty(myBalance) && myBalance != "0.00")
                            {
                                //sent new api
                                var (apiStatus, respond) = await RequestsAsync.Users.ManagePopularityAsync("boost").ConfigureAwait(false);
                                if (apiStatus == 200)
                                {
                                    if (respond is AmountObject result)
                                    {
                                        Activity?.RunOnUiThread(() =>
                                        {
                                            try
                                            {
                                                BoostButton.Tag = "Run";
                                                myBalance = result.CreditAmount.ToString();
                                                WalletNumber.Text = result.CreditAmount.ToString();

                                                var timeBoost = ListUtils.SettingsSiteList?.BoostExpireTime ?? "4";
                                                var timeBoostMilliseconds = Methods.Time.ConvertMinutesToMilliseconds(Convert.ToDouble(timeBoost));
                                                dataUser.BoostedTime = timeBoostMilliseconds.ToString(CultureInfo.InvariantCulture);
                                                dataUser.IsBoosted = "1";

                                                GetMyInfoData();
                                            }
                                            catch (Exception exception)
                                            {
                                                Methods.DisplayReportResultTrack(exception);
                                            }
                                        });
                                    }
                                }
                                else Methods.DisplayReportResult(Activity, respond);
                            }
                            else
                            {
                                var window = new PopupController(Activity);
                                window.DisplayCreditWindow();
                            }
                        }
                        else
                        {
                            //sent new api
                            var (apiStatus, respond) = await RequestsAsync.Users.ManagePopularityAsync("boost").ConfigureAwait(false);
                            if (apiStatus == 200)
                            {
                                if (respond is AmountObject result)
                                {
                                    Activity?.RunOnUiThread(() =>
                                    {
                                        try
                                        {
                                            BoostButton.Tag = "Run";
                                            //myBalance = result.CreditAmount.ToString();
                                            WalletNumber.Text = result.CreditAmount.ToString();

                                            var timeBoost = ListUtils.SettingsSiteList?.BoostExpireTime ?? "4";
                                            var timeBoostMilliseconds = Methods.Time.ConvertMinutesToMilliseconds(Convert.ToDouble(timeBoost));
                                            dataUser.BoostedTime = timeBoostMilliseconds.ToString(CultureInfo.InvariantCulture);
                                            dataUser.IsBoosted = "1";

                                            GetMyInfoData();
                                        }
                                        catch (Exception exception)
                                        {
                                            Methods.DisplayReportResultTrack(exception);
                                        }
                                    });
                                }
                            }
                            else Methods.DisplayReportResult(Activity, respond);
                        }
                    }
                    else
                    {
                        Toast.MakeText(Context, GetText(Resource.String.Lbl_YourBoostExpireInMinutes), ToastLength.Long)?.Show();
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Open Settings
        private void SettingsButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(Context, typeof(SettingsActivity)));
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Invite Friends
        private void InviteButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(Context, typeof(InviteFriendsActivity)));
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Help
        private void HelpButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var intent = new Intent(Context, typeof(LocalWebViewActivity));
                intent.PutExtra("URL", InitializeQuickDate.WebsiteUrl + "/contact");
                intent.PutExtra("Type", GetText(Resource.String.Lbl_Help));
                Activity.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Favorite
        private void FavoriteButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                FavoriteFragment = new FavoriteUserFragment();
                GlobalContext.FragmentBottomNavigator.DisplayFragment(FavoriteFragment);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        public async void GetMyInfoData()
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    return;
                }

                if (ListUtils.MyUserInfo.Count == 0)
                {
                    var sqlEntity = new SqLiteDatabase();
                    sqlEntity.GetDataMyInfo();

                }

                var dataUser = ListUtils.MyUserInfo?.FirstOrDefault();
                if (dataUser != null)
                {
                    LoadDataUser(dataUser);
                }

                var data = await ApiRequest.GetInfoData(Activity, UserDetails.UserId.ToString());
                if (data != null)
                {
                    LoadDataUser(data.Data);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void LoadDataUser(UserInfoObject data)
        {
            try
            {
                GlideImageLoader.LoadImage(Activity, data.Avater, ProfileImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                GlideImageLoader.LoadImage(Activity, data.Avater, GlobalContext.FragmentBottomNavigator.ProfileImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                Username.Text = QuickDateTools.GetNameFinal(data);
                Username.SetCompoundDrawablesWithIntrinsicBounds(0, 0, data.Verified == "1" ? Resource.Drawable.icon_checkmark_vector : 0, 0);

                if (!string.IsNullOrEmpty(data.CountryTxt))
                {
                    Location.Text = Methods.FunString.DecodeString(data.CountryTxt);
                }
                else
                    Location.Text = Activity.GetText(Resource.String.Lbl_Unknown);

                WalletNumber.Text = data.Balance.Replace(".00", "");

                if (data.LikesCount != null)
                    TxtLiked.Text = data.LikesCount.Value + " " + Activity.GetText(Resource.String.Lbl_Likes);
                else
                    TxtLiked.Text = data.Likes?.Count + " " + Activity.GetText(Resource.String.Lbl_Likes);

                if (data.VisitsCount != null)
                    TxtVisits.Text = data.VisitsCount.Value + " " + Activity.GetText(Resource.String.Lbl_Visits);
                else
                    TxtVisits.Text = data.Visits?.Count + " " + Activity.GetText(Resource.String.Lbl_Visits);

                if (data.IsPro == "1")
                {
                    switch (data.ProType)
                    {
                        case "1":
                            TxtUpgrade = GetText(Resource.String.Lbl_Weekly);
                            break;
                        case "2":
                            TxtUpgrade = GetText(Resource.String.Lbl_Monthly);
                            break;
                        case "3":
                            TxtUpgrade = GetText(Resource.String.Lbl_Yearly);
                            break;
                        case "4":
                            TxtUpgrade = GetText(Resource.String.Lbl_Lifetime);
                            break;
                        default:
                            TxtUpgrade = GetText(Resource.String.Lbl_Upgrade);
                            break;
                    }
                }
                else
                {
                    if (AppSettings.PremiumSystemEnabled)
                    {
                        TxtUpgrade = GetText(Resource.String.Lbl_Upgrade);
                        UpgradeButton.Visibility = ViewStates.Visible;
                    }
                }

                if (Convert.ToInt32(data.BoostedTime) > 0)
                {
                    var timeBoost = ListUtils.SettingsSiteList?.BoostExpireTime ?? "4";
                    var timeBoostSeconds = Methods.Time.ConvertMinutesToSeconds(Convert.ToDouble(timeBoost)); //240

                    double progressStart;
                    double progress = 100 / timeBoostSeconds; //0.4

                    if (Time == null)
                    {
                        double progressPlus = 100 / timeBoostSeconds;

                        Time = new TimerTime();
                        TimerTime.TimerCount = Time.GetTimer();
                        var plus1 = progressPlus;
                        TimerTime.TimerCount.Elapsed += (sender, args) =>
                        {
                            var plus = plus1;
                            Activity?.RunOnUiThread(() =>
                            {
                                try
                                {
                                    var (minutes, seconds) = Time.TimerCountOnElapsed();
                                    if ((minutes == "" || minutes == "0") && (seconds == "" || seconds == "0"))
                                    {
                                        Time.SetStopTimer();
                                        Time = null;
                                        TimerTime.TimerCount = null;

                                        data.BoostedTime = "0";
                                        XtBoostMe.Visibility = ViewStates.Invisible;
                                        XtBoostMe.Text = Context.GetText(Resource.String.Lbl_BoostMe);

                                        SetStopAnimationPopularity();
                                        progress = 0;
                                        progressStart = 0;
                                        MWaveLoadingView.CancelAnimation();

                                        BoostButton.Tag = "Off";
                                    }
                                    else
                                    {
                                        XtBoostMe.Text = minutes + ":" + seconds;
                                        XtBoostMe.Visibility = ViewStates.Visible;

                                        progress += plus;

                                        progressStart = Math.Round(progress, MidpointRounding.AwayFromZero);
                                        MWaveLoadingView.ProgressValue = Convert.ToInt32(progressStart);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Methods.DisplayReportResultTrack(e);
                                }
                            });
                        };
                    }

                    string countTime = Time.CheckCountTime(Convert.ToInt32(data.BoostedTime));
                    if (countTime != "0:0" && !countTime.Contains("-") && !string.IsNullOrEmpty(countTime))
                    {
                        int min = Convert.ToInt32(countTime.Split(":")[0]);
                        int sec = Convert.ToInt32(countTime.Split(":")[1]);
                        Time.SetMinutes(min);
                        Time.SetSeconds(sec);
                        Time.SetStartTimer();
                        XtBoostMe.Text = countTime;
                        XtBoostMe.Visibility = ViewStates.Visible;

                        var minSeconds = Methods.Time.ConvertMinutesToSeconds(Convert.ToDouble(min));

                        //start in here  
                        progress = (timeBoostSeconds - minSeconds) * 100 / timeBoostSeconds;

                        SetStartAnimationPopularity();
                    }
                    else
                    {
                        Time.SetStopTimer();
                        Time = null;
                        TimerTime.TimerCount = null;

                        XtBoostMe.Visibility = ViewStates.Invisible;
                        XtBoostMe.Text = Context.GetText(Resource.String.Lbl_BoostMe);

                        SetStopAnimationPopularity();

                        BoostButton.Tag = "Off";
                    }
                }
                else
                {
                    if (Time != null)
                    {
                        Time.SetStopTimer();
                        Time = null;
                        TimerTime.TimerCount = null;

                        XtBoostMe.Text = Context.GetText(Resource.String.Lbl_BoostMe);
                        XtBoostMe.Visibility = ViewStates.Invisible;
                        SetStopAnimationPopularity();

                        BoostButton.Tag = "Off";
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void SetStartAnimationPopularity()
        {
            try
            {
                BoostButton.Visibility = ViewStates.Invisible;
                MWaveLoadingView.Visibility = ViewStates.Visible;
                MWaveLoadingView.StartAnimation();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void SetStopAnimationPopularity()
        {
            try
            {
                BoostButton.Visibility = ViewStates.Visible;
                MWaveLoadingView.Visibility = ViewStates.Gone;

                MWaveLoadingView?.CancelAnimation();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }
}