using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.Widget;
using QuickDate.Activities.Base;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.Library.OneSignalNotif;
using QuickDateClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickDate.Activities.Default
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class FirstActivity : BaseActivity
    {
        #region Variables Basic

        private AppCompatButton LoginButton, RegisterButton;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                InitializeQuickDate.Initialize(AppSettings.TripleDesAppServiceProvider, PackageName, AppSettings.TurnSecurityProtocolType3072On, new MyReportModeApp());

                Methods.App.FullScreenApp(this);
                // Create your application here 
                SetContentView(Resource.Layout.FirstLayout);
                //Get Value  
                InitComponent();
                InitBackground();

                if (Methods.CheckConnectivity())
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetSettings_Api(this) });
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
                AddOrRemoveEvent(false);
                base.OnPause();
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

        private void InitComponent()
        {
            try
            {
                LoginButton = FindViewById<AppCompatButton>(Resource.Id.LoginButton);
                RegisterButton = FindViewById<AppCompatButton>(Resource.Id.RegisterButton);

                if (!string.IsNullOrEmpty(UserDetails.DeviceId))
                    OneSignalNotification.Instance.RegisterNotificationDevice(this);

                if (!AppSettings.RegisterEnabled)
                    RegisterButton.Visibility = ViewStates.Gone;
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
                    RegisterButton.Click += RegisterButton_Click;
                    LoginButton.Click += LoginButton_Click;
                }
                else
                {
                    RegisterButton.Click -= RegisterButton_Click;
                    LoginButton.Click -= LoginButton_Click;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        //Event Click open Register Activity
        private void RegisterButton_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(this, typeof(RegisterActivity)));
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }

        //Event Click open Login Activity
        private void LoginButton_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(this, typeof(LoginActivity)));
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }

        #endregion

    }
}