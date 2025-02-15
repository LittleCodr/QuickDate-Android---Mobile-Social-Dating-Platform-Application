using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Content.Res;
using AndroidX.Core.Content;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Com.Google.Android.Play.Core.Install.Model;
using Google.Android.Material.Dialog;
using Newtonsoft.Json;
using QuickDate.Activities.Chat;
using QuickDate.Activities.Live.Utils;
using QuickDate.Activities.SettingsUser;
using QuickDate.Activities.Tabbes.Fragment;
using QuickDate.Activities.UserProfile;
using QuickDate.ButtomSheets;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.Library.OneSignalNotif;
using QuickDate.Library.OneSignalNotif.Models;
using QuickDate.PaymentUtil.Utils;
using QuickDate.SQLite;
using QuickDateClient;
using QuickDateClient.Classes.Chat;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickDate.Services;
using ActivityResult = Com.Google.Android.Play.Core.Install.Model.ActivityResult;
using Exception = System.Exception;
using Permission = Android.Content.PM.Permission;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace QuickDate.Activities.Tabbes
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Keyboard | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenLayout | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.UiMode)]
    public class HomeActivity : PaymentBaseActivity
    {
        #region Variables Basic

        public CardMachFragment CardFragment;
        public TrendingFragment TrendingFragment;
        public NotificationsFragment NotificationsFragment;
        public ProfileFragment ProfileFragment;
        public LastChatFragment LastChatFragment;
        public LinearLayout NavigationTabBar;
        public CustomNavigationController FragmentBottomNavigator;

        public string TypeAvatar;
        public static long CountNotificationsStatic, CountMessagesStatic;
        private static HomeActivity Instance;
        public TracksCounter TracksCounter;
        private PowerManager.WakeLock Wl;
        private readonly Handler ExitHandler = new Handler(Looper.MainLooper);
        private bool RecentlyBackPressed;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);

                Methods.App.FullScreenApp(this);

                Delegate.SetLocalNightMode(QuickDateTools.IsTabDark() ? AppCompatDelegate.ModeNightYes : AppCompatDelegate.ModeNightNo);
                SetTheme(QuickDateTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);

                //AddFlagsWakeLock();

                // Create your application here
                SetContentView(Resource.Layout.TabbedMainLayout);

                Instance = this;

                TracksCounter = new TracksCounter(this);

                if (AppSettings.EnableAppFree)
                    AppSettings.PremiumSystemEnabled = false;

                //Get Value
                InitBackground();
                SetupBottomNavigationView();
                InitBackPressed("HomeActivity");

                Task.Run(() => { MainApplication.GetInstance().SecondRunExcite(this); });

                GetMyInfoData();

                string osNotificationObject = Intent?.GetStringExtra("OsNotificationObject") ?? "";
                if (!string.IsNullOrEmpty(osNotificationObject))
                {
                    var dataNotification = JsonConvert.DeserializeObject<OsObject.OsNotificationObject>(osNotificationObject);
                    if (dataNotification == null)
                        return;

                    var intent = new Intent(this, typeof(UserProfileActivity));

                    switch (dataNotification.Type)
                    {
                        case "got_new_match":
                            intent.PutExtra("EventPage", "HideButton");
                            break;
                        case "like":
                            intent.PutExtra("EventPage", "likeAndClose");
                            break;
                        default:
                            intent.PutExtra("EventPage", "Close");
                            break;
                    }

                    intent.PutExtra("DataType", "OneSignal");
                    intent.PutExtra("ItemUser", JsonConvert.SerializeObject(dataNotification.UserData));
                    StartActivity(intent);
                }

                SetService();
                InitBuy();

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                {
                    if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.PostNotifications) == Permission.Granted)
                    {
                        if (string.IsNullOrEmpty(UserDetails.DeviceId))
                            OneSignalNotification.Instance.RegisterNotificationDevice(this);
                    }
                    else
                    {
                        RequestPermissions(new[]
                        {
                            Manifest.Permission.PostNotifications
                        }, 16248);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(UserDetails.DeviceId))
                        OneSignalNotification.Instance.RegisterNotificationDevice(this);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            try
            {
                base.OnConfigurationChanged(newConfig);

                var currentNightMode = newConfig.UiMode & UiMode.NightMask;
                switch (currentNightMode)
                {
                    case UiMode.NightNo:
                        // Night mode is not active, we're using the light theme
                        MainSettings.ApplyTheme(MainSettings.LightMode);
                        break;
                    case UiMode.NightYes:
                        // Night mode is active, we're using dark theme
                        MainSettings.ApplyTheme(MainSettings.DarkMode);
                        break;
                }

                Delegate.SetLocalNightMode(QuickDateTools.IsTabDark() ? AppCompatDelegate.ModeNightYes : AppCompatDelegate.ModeNightNo);
                SetTheme(QuickDateTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);

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
                SetWakeLock();
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
                CardFragment?.SaveSwipeCount();
                OffWakeLock();
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

        public static HomeActivity GetInstance()
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

        protected override void OnDestroy()
        {
            try
            {
                if (AppSettings.ShowInAppBilling && InitializeQuickDate.IsExtended)
                    BillingSupport?.Destroy();

                ProfileFragment?.Time?.SetStopTimer();

                OffWakeLock();

                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Functions

        public void SetToolBar(Toolbar toolbar, string title, bool setBackground, bool showIconBack = true)
        {
            try
            {
                if (toolbar != null)
                {
                    if (!string.IsNullOrEmpty(title))
                        toolbar.Title = title;

                    toolbar.SetTitleTextColor(QuickDateTools.IsTabDark() ? AppSettings.TitleTextColorDark : AppSettings.TitleTextColor);
                    SetSupportActionBar(toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(showIconBack);
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

        private void SetupBottomNavigationView()
        {
            try
            {
                NavigationTabBar = FindViewById<LinearLayout>(Resource.Id.ntb_horizontal);

                FragmentBottomNavigator = new CustomNavigationController(this);
                CardFragment = new CardMachFragment();

                if (AppSettings.ShowTrending)
                    TrendingFragment = new TrendingFragment();

                NotificationsFragment = new NotificationsFragment();
                LastChatFragment = new LastChatFragment();
                ProfileFragment = new ProfileFragment();

                FragmentBottomNavigator.FragmentListTab0.Add(CardFragment);

                if (AppSettings.ShowTrending)
                    FragmentBottomNavigator.FragmentListTab1.Add(TrendingFragment);

                FragmentBottomNavigator.FragmentListTab2.Add(NotificationsFragment);
                FragmentBottomNavigator.FragmentListTab3.Add(LastChatFragment);
                FragmentBottomNavigator.FragmentListTab4.Add(ProfileFragment);

                FragmentBottomNavigator.ShowFragment0();
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
                    BackPressed();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Events

        public void ShowMessagesBox(UserInfoObject dataUser)
        {
            try
            {
                Intent intent = new Intent(this, typeof(MessagesBoxActivity));
                intent.PutExtra("UserId", dataUser.Id.ToString());
                intent.PutExtra("TypeChat", "LastChat");
                intent.PutExtra("UserItem", JsonConvert.SerializeObject(dataUser));

                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    StartActivity(intent);
                }
                else
                {
                    //Check to see if any permission in our group is available, if one, then all are
                    if (PermissionsController.CheckPermissionStorage(this))
                    {
                        StartActivity(intent);

                    }
                    else
                        new PermissionsController(this).RequestPermission(100);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void OpenAddPhotoFragment()
        {
            try
            {
                var addPhoto = new AddPhotoBottomDialogFragment();
                addPhoto.Show(SupportFragmentManager, "addPhoto");
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Permissions && Result

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                //Get Location And Get Data Api
                if (requestCode == 1050)
                {
                    CardFragment?.CheckAndGetLocation();
                }
                else if (requestCode == PixImagePickerActivity.RequestCode && resultCode == Result.Ok)
                {
                    var listPath = JsonConvert.DeserializeObject<ResultIntentPixImage>(data.GetStringExtra("ResultPixImage") ?? "");
                    if (listPath?.List?.Count > 0)
                    {
                        var filepath = listPath.List.FirstOrDefault();
                        if (!string.IsNullOrEmpty(filepath))
                        {
                            if (TypeAvatar == "Avatar")
                            {
                                GlideImageLoader.LoadImage(this, filepath, ProfileFragment?.ProfileImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.UpdateAvatarApi(this, filepath) });
                            }
                            else
                            {
                                //sent api  
                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.UploadMediaFileUserAsync(filepath) });
                            }

                            var reviewMediaFiles = ListUtils.SettingsSiteList?.ReviewMediaFiles;
                            if (reviewMediaFiles == "1") //Uploaded successfully, file will be reviewed
                                Toast.MakeText(this, GetText(Resource.String.Lbl_UploadedSuccessfullyWithReviewed), ToastLength.Long)?.Show();
                            else
                                Toast.MakeText(this, GetText(Resource.String.Lbl_UploadedSuccessfully), ToastLength.Long)?.Show();
                        }
                    }
                }
                else if (requestCode == 4711)
                    switch (resultCode) // The switch block will be triggered only with flexible update since it returns the install result codes
                    {
                        case Result.Ok:
                            // In app update success
                            if (UpdateManagerApp.AppUpdateTypeSupported == AppUpdateType.Immediate)
                                Toast.MakeText(this, "App updated", ToastLength.Short)?.Show();
                            break;
                        case Result.Canceled:
                            Toast.MakeText(this, "In app update cancelled", ToastLength.Short)?.Show();
                            break;
                        case (Result)ActivityResult.ResultInAppUpdateFailed:
                            Toast.MakeText(this, "In app update failed", ToastLength.Short)?.Show();
                            break;
                    }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                switch (requestCode)
                {
                    case 105 when grantResults.Length > 0 && grantResults[0] == Permission.Granted:
                        //Get Location And Get Data Api
                        CardFragment?.CheckAndGetLocation();
                        break;
                    case 105:
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long)?.Show();
                        break;
                    case 108 when grantResults.Length > 0 && grantResults[0] == Permission.Granted:
                        PixImagePickerUtils.OpenDialogGallery(this);
                        break;
                    case 108:
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long)?.Show();
                        break;
                    case 110 when grantResults.Length > 0 && grantResults[0] == Permission.Granted:
                        Window?.AddFlags(WindowManagerFlags.KeepScreenOn);
                        break;
                    case 110:
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long)?.Show();
                        break;
                    case 100 when grantResults.Length > 0 && grantResults[0] == Permission.Granted:
                        ShowMessagesBox(DialogController.DataUser);
                        break;
                    case 100:
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long)?.Show();
                        break;
                    case 111 when grantResults.Length > 0 && grantResults[0] == Permission.Granted:
                        new LiveUtil(this).GoLiveOnClick();
                        break;
                    case 111:
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long)?.Show();
                        break;
                    case 1300 when grantResults.Length > 0 && grantResults[0] == Permission.Granted:
                        LastChatFragment?.OpenMessagesBoxIntent();
                        break;
                    case 1300:
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long)?.Show();
                        break;
                    case 16248 when grantResults.Length > 0 && grantResults[0] == Permission.Granted:
                        if (string.IsNullOrEmpty(UserDetails.DeviceId))
                            OneSignalNotification.Instance.RegisterNotificationDevice(this);
                        break;
                    case 16248:
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

        #region Back Pressed 

        public void BackPressed()
        {
            try
            {
                int count = LastChatFragment.MAdapter?.GetSelectedItemCount() ?? 0;
                if (count > 0)
                {
                    LastChatFragment.ToolBar.Visibility = ViewStates.Visible;
                    LastChatFragment.ActionMode.Finish();
                }
                else
                {
                    if (FragmentBottomNavigator.GetCountFragment() > 0)
                    {
                        FragmentBottomNavigator.OnBackStackClickFragment();
                    }
                    else
                    {
                        if (RecentlyBackPressed)
                        {
                            ExitHandler.RemoveCallbacks(() => { RecentlyBackPressed = false; });
                            RecentlyBackPressed = false;
                            MoveTaskToBack(true);
                            //Finish();
                        }
                        else
                        {
                            RecentlyBackPressed = true;
                            Toast.MakeText(this, GetString(Resource.String.press_again_exit), ToastLength.Long)?.Show();
                            ExitHandler.PostDelayed(() => { RecentlyBackPressed = false; }, 2000L);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Timer

        public void GetNotifications(long countNotifications, long countMessages)
        {
            try
            {
                if (FragmentBottomNavigator != null)
                {
                    if (FragmentBottomNavigator.NotificationImage != null && countNotifications != 0 && countNotifications != CountNotificationsStatic)
                    {
                        RunOnUiThread(() =>
                        {
                            try
                            {
                                CountNotificationsStatic = countNotifications;
                                FragmentBottomNavigator.ShowNotificationBadge(true);
                            }
                            catch (Exception e)
                            {
                                Methods.DisplayReportResultTrack(e);
                            }
                        });
                    }
                    else
                    {
                        RunOnUiThread(() =>
                        {
                            try
                            {
                                CountNotificationsStatic = 0;
                                FragmentBottomNavigator.ShowNotificationBadge(false);
                            }
                            catch (Exception e)
                            {
                                Methods.DisplayReportResultTrack(e);
                            }
                        });
                    }

                    if (FragmentBottomNavigator.MessagesImage != null && countMessages != 0 && countMessages != CountMessagesStatic)
                    {
                        RunOnUiThread(() =>
                        {
                            try
                            {
                                CountMessagesStatic = countMessages;
                                FragmentBottomNavigator.ShowMessagesBadge(true, Convert.ToInt32(CountMessagesStatic));
                            }
                            catch (Exception e)
                            {
                                Methods.DisplayReportResultTrack(e);
                            }
                        });
                    }
                    //else
                    //{
                    //    RunOnUiThread(() =>
                    //    {
                    //        try
                    //        {
                    //            CountMessagesStatic = 0;
                    //            //FragmentBottomNavigator.ShowMessagesBadge(false, 0);
                    //        }
                    //        catch (Exception e)
                    //        {
                    //            Methods.DisplayReportResultTrack(e);
                    //        }
                    //    });
                    //}
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region General App Data

        private void GetMyInfoData()
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    return;
                }

                var dbDatabase = new SqLiteDatabase();
                dbDatabase.GetSettings();
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetSettings_Api(this) });

                RunOnUiThread(() =>
                {
                    try
                    {
                        var dataUser = dbDatabase.GetDataMyInfo();
                        if (dataUser != null)
                        {
                            Glide.With(this).Load(UserDetails.Avatar).Apply(new RequestOptions().SetDiskCacheStrategy(DiskCacheStrategy.All).CircleCrop()).Preload();
                            GlideImageLoader.LoadImage(this, UserDetails.Avatar, FragmentBottomNavigator.ProfileImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                        }
                    }
                    catch (Exception e)
                    {
                        Methods.DisplayReportResultTrack(e);
                    }
                });

                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetInfoData(this, UserDetails.UserId.ToString()) });

                var listStickers = dbDatabase.GetStickersList();
                if (ListUtils.StickersList.Count == 0 && listStickers?.Count > 0)
                    ListUtils.StickersList = listStickers;
                else
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetStickers(this) });

                var listGifts = dbDatabase.GetGiftsList();
                if (ListUtils.GiftsList.Count == 0 && listGifts?.Count > 0)
                    ListUtils.GiftsList = listGifts;
                else
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetGifts(this) });

                InAppUpdate();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void InAppUpdate()
        {
            try
            {
                if (AppSettings.ShowSettingsUpdateManagerApp)
                    UpdateManagerApp.CheckUpdateApp(this, 4711, Intent);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private static int CountRateApp;
        public void InAppReview()
        {
            try
            {
                bool inAppReview = MainSettings.InAppReview.GetBoolean(MainSettings.PrefKeyInAppReview, false);
                if (!inAppReview && AppSettings.ShowSettingsRateApp)
                {
                    if (CountRateApp == AppSettings.ShowRateAppCount)
                    {
                        var dialog = new MaterialAlertDialogBuilder(this);
                        dialog.SetTitle(GetText(Resource.String.Lbl_RateOurApp));
                        dialog.SetMessage(GetText(Resource.String.Lbl_RateOurAppContent));
                        dialog.SetPositiveButton(GetText(Resource.String.Lbl_Rate), (materialDialog, action) =>
                        {
                            try
                            {
                                StoreReviewApp store = new StoreReviewApp();
                                store.OpenStoreReviewPage(this, PackageName);
                            }
                            catch (Exception e)
                            {
                                Methods.DisplayReportResultTrack(e);
                            }
                        });
                        dialog.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                        dialog.Show();

                        MainSettings.InAppReview?.Edit()?.PutBoolean(MainSettings.PrefKeyInAppReview, true)?.Commit();
                    }
                    else
                    {
                        CountRateApp++;
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region WakeLock System

        private void AddFlagsWakeLock()
        {
            try
            {
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    Window?.AddFlags(WindowManagerFlags.KeepScreenOn);
                }
                else
                {
                    if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WakeLock) == Permission.Granted)
                    {
                        Window?.AddFlags(WindowManagerFlags.KeepScreenOn);
                    }
                    else
                    {
                        //request Code 110
                        new PermissionsController(this).RequestPermission(110);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetWakeLock()
        {
            try
            {
                if (Wl == null)
                {
                    PowerManager pm = (PowerManager)GetSystemService(PowerService);
                    Wl = pm.NewWakeLock(WakeLockFlags.ScreenBright, "My Tag");
                    Wl?.Acquire();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetOnWakeLock()
        {
            try
            {
                PowerManager pm = (PowerManager)GetSystemService(PowerService);
                Wl = pm.NewWakeLock(WakeLockFlags.ScreenBright, "My Tag");
                Wl?.Acquire();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetOffWakeLock()
        {
            try
            {
                PowerManager pm = (PowerManager)GetSystemService(PowerService);
                Wl = pm.NewWakeLock(WakeLockFlags.ProximityScreenOff, "My Tag");
                Wl?.Acquire();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OffWakeLock()
        {
            try
            {
                // ..screen will stay on during this section..
                Wl?.Release();
                Wl = null;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Service Chat

        public void SetService(bool run = true)
        {
            try
            {
                if (run)
                {
                    // reschedule the job
                    AppApiService.GetInstance().StartService(this);
                }
                else
                {
                    // Cancel all jobs
                    AppApiService.GetInstance().StopJob(this);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnReceiveResult(string resultData)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<GetConversationListObject>(resultData);
                if (result != null && LastChatFragment?.MAdapter != null)
                {
                    LastChatFragment?.LoadDataLastChatNewV(result);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

    }
}