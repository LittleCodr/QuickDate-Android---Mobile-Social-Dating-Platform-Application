using Android.App;
using Android.Content;
using Com.Onesignal;
using Com.Onesignal.Debug;
using Com.Onesignal.InAppMessages;
using Com.Onesignal.Notifications;
using Com.Onesignal.User.Subscriptions;
using Newtonsoft.Json;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.Library.OneSignalNotif.Models;
using System;
using Object = Java.Lang.Object;

namespace QuickDate.Library.OneSignalNotif
{
    public class OneSignalNotification : Java.Lang.Object, Java.Util.Functions.IConsumer, INotificationLifecycleListener, INotificationClickListener, IPushSubscriptionObserver, IInAppMessageLifecycleListener
    {
        //Force your app to Register Notification directly without loading it from server (For Best Result)

        private OsObject.OsNotificationObject DataNotification;

        private static volatile OneSignalNotification InstanceRenamed;
        public static OneSignalNotification Instance
        {
            get
            {
                OneSignalNotification localInstance = InstanceRenamed;
                if (localInstance == null)
                {
                    lock (typeof(OneSignalNotification))
                    {
                        localInstance = InstanceRenamed;
                        if (localInstance == null)
                        {
                            InstanceRenamed = localInstance = new OneSignalNotification();
                        }
                    }
                }
                return localInstance;

            }
        }

        public void RegisterNotificationDevice(Context context)
        {
            try
            {
                if (!string.IsNullOrEmpty(AppSettings.OneSignalAppId) || !string.IsNullOrWhiteSpace(AppSettings.OneSignalAppId))
                {
                    //The following options are available with increasingly more information:
                    //NONE, FATAL, ERROR, WARN, INFO, DEBUG, VERBOSE
                    OneSignal.Debug.LogLevel = LogLevel.Verbose;
                    OneSignal.Debug.AlertLevel = LogLevel.None;

                    // OneSignal Initialization  
                    OneSignal.InitWithContext(context, AppSettings.OneSignalAppId);

                    // OneSignal Methods
                    OneSignal.Notifications.RequestPermission(true, Continue.With(this));
                    OneSignal.Notifications.AddForegroundLifecycleListener(this);
                    OneSignal.Notifications.AddClickListener(this);
                    //OneSignal.Notifications.AddPermissionObserver(this);

                    OneSignal.InAppMessages.AddLifecycleListener(this);

                    OneSignal.Login(UserDetails.UserId.ToString());

                    OneSignal.ConsentRequired = true;
                    OneSignal.ConsentGiven = true;

                    OneSignal.InAppMessages.Paused = true;
                    OneSignal.Location.Shared = true;

                    OneSignal.User.PushSubscription.AddObserver(this);
                    IdsAvailable();
                }
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }

        public void UnRegisterNotificationDevice()
        {
            try
            {
                OneSignal.Notifications.ClearAllNotifications();

                OneSignal.Notifications.RemoveForegroundLifecycleListener(this);
                OneSignal.Notifications.RemoveClickListener(this);

                OneSignal.User.PushSubscription.RemoveObserver(this);

                OneSignal.Logout();

                //AppSettings.ShowNotification = false;
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }

        public void IdsAvailable()
        {
            try
            {
                var device = OneSignal.User.PushSubscription;

                if (device != null)
                {
                    //string email = device.EmailAddress;
                    //string emailId = device.EmailUserId;
                    string pushToken = device.Token;
                    string userId = device.Id;

                    //bool enabled = device.AreNotificationsEnabled();
                    bool subscribed = device.OptedIn;
                    //bool subscribedToOneSignal = device.IsEmailSubscribed;

                    if (subscribed && !string.IsNullOrEmpty(userId))
                        UserDetails.DeviceId = userId;
                }
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }

        /// <summary>
        /// NotificationWillShowInForeground
        /// Adds a listener to run before whenever a notification lifecycle event occurs.
        /// </summary>
        /// <param name="result"></param>
        public void OnWillDisplay(INotificationWillDisplayEvent result)
        {
            try
            {
                var notification = result;

                string title = notification.Notification.Title;
                string message = notification.Notification.Body;
                var additionalData = notification.Notification.AdditionalData?.ToString();
                DataNotification = JsonConvert.DeserializeObject<OsObject.OsNotificationObject>(additionalData);
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }

        /// <summary>
        /// NotificationOpened
        /// Adds a listener that will run whenever a notification is clicked on by the user.
        /// </summary>
        /// <param name="result"></param>
        public void OnClick(INotificationClickEvent result)
        {
            try
            {
                var notification = result;

                string title = notification.Notification.Title;
                string message = notification.Notification.Body;
                var additionalData = notification.Notification.AdditionalData?.ToString();
                DataNotification = JsonConvert.DeserializeObject<OsObject.OsNotificationObject>(additionalData);

                EventClickNotification();
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }

        public void EventClickNotification()
        {
            try
            {
                Intent intent = new Intent(Application.Context, typeof(HomeActivity));
                intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                intent.AddFlags(ActivityFlags.SingleTop);
                intent.SetAction(Intent.ActionView);
                intent.PutExtra("OsNotificationObject", JsonConvert.SerializeObject(DataNotification));
                Application.Context.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        /// <summary>
        /// respond to permission state change
        /// </summary>
        /// <param name="state"></param>
        public void OnPushSubscriptionChange(PushSubscriptionChangedState state)
        {
            try
            {
                //wael check  
                if (state.Current.OptedIn && !string.IsNullOrEmpty(state.Current.Token))
                    UserDetails.DeviceId = state.Current.Token;

                IdsAvailable();
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }

        public void OnDidDismiss(IInAppMessageDidDismissEvent e)
        {

        }

        public void OnDidDisplay(IInAppMessageDidDisplayEvent e)
        {

        }

        public void OnWillDismiss(IInAppMessageWillDismissEvent e)
        {

        }

        public void OnWillDisplay(IInAppMessageWillDisplayEvent e)
        {

        }

        public void Accept(Object t)
        {

        }
    }
}