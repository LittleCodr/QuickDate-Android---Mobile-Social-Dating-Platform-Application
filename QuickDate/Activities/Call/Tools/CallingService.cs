using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Media;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Newtonsoft.Json;
using QuickDate.Activities.Call.Agora;
using QuickDate.Activities.Call.Twilio;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Call;
using QuickDateClient.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickDate.Activities.Tabbes;
using Console = System.Console;
using Exception = System.Exception;

namespace QuickDate.Activities.Call.Tools
{
    [Service(Exported = true)]
    public class CallingService : Android.App.Service, ProximitySensor.IDelegate, AudioManager.IOnAudioFocusChangeListener
    {
        #region Variables Basic

        private static CallingService Service;

        private AudioManager AudioManager;
        private ProximitySensor ProximitySensor;
        public RingtonePlayer RingtonePlayer;
        private PowerManager.WakeLock PowerManagerWakeLock;

        public static readonly string ActionStartIncoming = Application.Context.PackageName + ".action.ACTION_START_INCOMING";
        public static readonly string ActionStartNewCall = Application.Context.PackageName + ".action.ACTION_START_NEW_CALL";
        public static readonly string ActionAnswer = Application.Context.PackageName + ".action.ACTION_ANSWER";
        public static readonly string ActionDecline = Application.Context.PackageName + ".action.ACTION_DECLINE";
        public static readonly string ActionCloseCall = Application.Context.PackageName + ".action.ACTION_CLOSE_CALL";

        #endregion

        #region General

        public static CallingService GetService()
        {
            return Service;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            try
            {
                base.OnCreate();
                Service = this;

                MNotificationManager = (NotificationManager)GetSystemService(NotificationService);
                AudioManager = (AudioManager)GetSystemService(AudioService);
                ProximitySensor = new ProximitySensor(this, this);
                RingtonePlayer = new RingtonePlayer(this);
                InitializePowerManagerWakeLock();
                BuildNotification();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            try
            {
                base.OnStartCommand(intent, flags, startId);
                if (intent != null)
                {
                    if (intent.HasExtra("type"))
                    {
                        CallConstant.CallType = intent?.GetStringExtra("type") ?? "";
                    }

                    var callObj = intent?.GetStringExtra("callUserObject") ?? "";
                    if (!string.IsNullOrEmpty(callObj))
                        CallConstant.CallUserObject = JsonConvert.DeserializeObject<DataCallObject>(callObj);

                    string action = intent.Action;
                    if (action == ActionStartIncoming)
                    {
                        CallConstant.CallActive = true;

                        var check = CallConstant.CallUserList.FirstOrDefault(a => a.Id == CallConstant.CallUserObject.Id);
                        if (check == null)
                        {
                            CallConstant.CallUserList.Add(CallConstant.CallUserObject);

                            var title = intent?.GetStringExtra("title") ?? "";
                            string longDescription = CallConstant.CallUserObject.Fullname + " " + GetText(Resource.String.Lbl_is_calling_you);
                            CreateNotificationIncoming(this, CallConstant.CallUserObject.Avater, title, longDescription, CallConstant.CallUserObject.Id);
                            OnPowerManagerWakeLock();

                            if (RingtonePlayer != null && AudioManager.RingerMode == RingerMode.Normal)
                            {
                                RequestAudioFocus(Stream.VoiceCall);

                                //if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
                                RingtonePlayer.PlayIncomingRingtone();
                            }
                        }
                    }
                    else if (action == ActionStartNewCall)
                    {
                        CallConstant.CallActive = true;

                        CreateActiveCallNotification(CallConstant.CallUserObject.Id, true);
                        StartNewCall();

                        if (RingtonePlayer != null && AudioManager.RingerMode == RingerMode.Normal)
                        {
                            RequestAudioFocus(Stream.VoiceCall);

                            //if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
                            RingtonePlayer.PlayProgressTone();
                        }
                    }
                    else if (action == ActionAnswer)
                    {
                        AnswerCall();

                        if (RingtonePlayer != null && AudioManager.RingerMode == RingerMode.Normal)
                        {
                            RequestAudioFocus(Stream.VoiceCall);

                            RingtonePlayer?.StopRingtone();
                        }

                        //divert sound to Bluetooth headset if available
                        if (AudioHelper.IsBluetoothHeadsetOn(AudioManager))
                            SetBluetoothHeadset(true);
                        CreateActiveCallNotification(CallConstant.CallUserObject.Id, false);
                    }
                    else if (action == ActionDecline)
                    {
                        RejectCall(intent);
                    }
                    else if (action == ActionCloseCall)
                    {
                        CloseCall(intent);
                    }
                }

                return StartCommandResult.NotSticky;
            }
            catch (Exception a)
            {
                Console.WriteLine(a);
                return base.OnStartCommand(intent, flags, startId);
            }
        }

        public override void OnTaskRemoved(Intent rootIntent)
        {
            try
            {
                base.OnTaskRemoved(rootIntent);
                //Stop(rootIntent);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion
         
        #region Function

        public void AnswerCall()
        {
            try
            {
                switch (CallConstant.CallType)
                {
                    case "Agora_video_call_recieve":
                    case "Twilio_video_call":
                        {
                            if ((int)Build.VERSION.SdkInt >= 23)
                            {
                                if (CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted && CheckSelfPermission(Manifest.Permission.RecordAudio) == Permission.Granted && CheckSelfPermission(Manifest.Permission.ModifyAudioSettings) == Permission.Granted)
                                {
                                    StartCallIncoming(TypeCall.Video);
                                }
                                else
                                {
                                    Intent intent = new Intent(this, typeof(VideoAudioComingCallActivity));
                                    intent.PutExtra("callUserObject", JsonConvert.SerializeObject(CallConstant.CallUserObject));
                                    intent.PutExtra("type", CallConstant.CallType);
                                    intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.MultipleTask);
                                    //intent.AddFlags(ActivityFlags.SingleTop);
                                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                                    {
                                        ActivityOptions options = ActivityOptions.MakeBasic();
                                        options.PendingIntentBackgroundActivityLaunchAllowed = true;
                                        StartActivity(intent, options?.ToBundle());
                                    }
                                    else
                                    {
                                        StartActivity(intent);
                                    }
                                }
                            }
                            else
                            {
                                StartCallIncoming(TypeCall.Video);
                            }
                            break;
                        }
                    case "Agora_audio_call_recieve":
                    case "Twilio_audio_call":
                        {
                            if ((int)Build.VERSION.SdkInt >= 23)
                            {
                                if (CheckSelfPermission(Manifest.Permission.RecordAudio) == Permission.Granted && CheckSelfPermission(Manifest.Permission.ModifyAudioSettings) == Permission.Granted)
                                {
                                    StartCallIncoming(TypeCall.Audio);
                                }
                                else
                                {
                                    Intent intent = new Intent(this, typeof(VideoAudioComingCallActivity));
                                    intent.PutExtra("callUserObject", JsonConvert.SerializeObject(CallConstant.CallUserObject));
                                    intent.PutExtra("type", CallConstant.CallType);
                                    intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.MultipleTask);
                                    //intent.AddFlags(ActivityFlags.SingleTop);
                                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                                    {
                                        ActivityOptions options = ActivityOptions.MakeBasic();
                                        options.PendingIntentBackgroundActivityLaunchAllowed = true;
                                        StartActivity(intent, options?.ToBundle());
                                    }
                                    else
                                    {
                                        StartActivity(intent);
                                    }
                                }
                            }
                            else
                            {
                                StartCallIncoming(TypeCall.Audio);
                            }
                            break;
                        }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void StartCallIncoming(TypeCall type)
        {
            try
            {
                var dataSettings = ListUtils.SettingsSiteList;
                CallConstant.TypeCall = type;
                Intent intentCall = null;
                if (type == TypeCall.Audio)
                {
                    if (dataSettings?.AgoraChatCall == "1")
                    {
                        intentCall = new Intent(this, typeof(AgoraAudioCallActivity));
                        intentCall.PutExtra("type", "Agora_audio_call_recieve");
                    }
                    else if (dataSettings?.TwilioChatCall == "1")
                    {
                        intentCall = new Intent(this, typeof(TwilioAudioCallActivity));
                        intentCall.PutExtra("type", "Twilio_audio_call");
                    }
                }
                else if (type == TypeCall.Video)
                {
                    if (dataSettings?.AgoraChatCall == "1")
                    {
                        intentCall = new Intent(this, typeof(AgoraVideoCallActivity));
                        intentCall.PutExtra("type", "Agora_video_call_recieve");
                    }
                    else if (dataSettings?.TwilioChatCall == "1")
                    {
                        intentCall = new Intent(this, typeof(TwilioVideoCallActivity));
                        intentCall.PutExtra("type", "Twilio_video_call");
                    }
                }

                if (CallConstant.CallUserObject != null)
                {
                    intentCall?.PutExtra("callUserObject", JsonConvert.SerializeObject(CallConstant.CallUserObject));
                }

                if (intentCall != null)
                {
                    intentCall.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.MultipleTask);
                    //intentCall.AddFlags(ActivityFlags.SingleTop);
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                    {
                        ActivityOptions options = ActivityOptions.MakeBasic();
                        options.PendingIntentBackgroundActivityLaunchAllowed = true;
                        StartActivity(intentCall, options?.ToBundle());
                    }
                    else
                    {
                        StartActivity(intentCall);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void StartNewCall()
        {
            try
            {
                Intent intentCall = null;
                switch (CallConstant.CallType)
                {
                    case "Agora_audio_calling_start":
                        intentCall = new Intent(this, typeof(AgoraAudioCallActivity));
                        intentCall.PutExtra("type", "Agora_audio_calling_start");
                        CallConstant.TypeCall = TypeCall.Audio;
                        break;
                    case "Twilio_audio_calling_start":
                        intentCall = new Intent(this, typeof(TwilioAudioCallActivity));
                        intentCall.PutExtra("type", "Twilio_audio_calling_start");
                        CallConstant.TypeCall = TypeCall.Audio;
                        break;
                    case "Agora_video_calling_start":
                        intentCall = new Intent(this, typeof(AgoraVideoCallActivity));
                        intentCall.PutExtra("type", "Agora_video_calling_start");
                        CallConstant.TypeCall = TypeCall.Video;
                        break;
                    case "Twilio_video_calling_start":
                        intentCall = new Intent(this, typeof(TwilioVideoCallActivity));
                        intentCall.PutExtra("type", "Twilio_video_calling_start");
                        CallConstant.TypeCall = TypeCall.Video;
                        break;
                }

                if (CallConstant.CallUserObject != null)
                    intentCall?.PutExtra("callUserObject", JsonConvert.SerializeObject(CallConstant.CallUserObject));

                if (intentCall != null)
                {
                    intentCall.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.MultipleTask);
                    //intentCall.AddFlags(ActivityFlags.SingleTop);
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                    {
                        ActivityOptions options = ActivityOptions.MakeBasic();
                        options.PendingIntentBackgroundActivityLaunchAllowed = true;
                        StartActivity(intentCall, options?.ToBundle());
                    }
                    else
                    {
                        StartActivity(intentCall);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void RejectCall(Intent intent)
        {
            try
            {
                switch (CallConstant.CallType)
                {
                    case "Twilio_video_call":
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { async () => await RequestsAsync.Call.DeclineCallAsync(CallConstant.CallUserObject.Id, TypeCall.Video) });
                        //HomeActivity.AddCallToListAndSend("Cancel", GetText(Resource.String.Lbl_Missing), TypeCall.Video, CallConstant.CallUserObject);
                        break;
                    case "Twilio_audio_call":
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { async () => await RequestsAsync.Call.DeclineCallAsync(CallConstant.CallUserObject.Id, TypeCall.Audio) });
                        //HomeActivity.AddCallToListAndSend("Cancel", GetText(Resource.String.Lbl_Missing), TypeCall.Audio, CallConstant.CallUserObject);
                        break;
                    case "Agora_video_call_recieve":
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { async () => await RequestsAsync.Call.DeclineCallAsync(CallConstant.CallUserObject.Id, TypeCall.Video) });
                        //HomeActivity.AddCallToListAndSend("Cancel", GetText(Resource.String.Lbl_Missing), TypeCall.Video, CallConstant.CallUserObject);
                        break;
                    case "Agora_audio_call_recieve":
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { async () => await RequestsAsync.Call.DeclineCallAsync(CallConstant.CallUserObject.Id, TypeCall.Audio) });
                        //HomeActivity.AddCallToListAndSend("Cancel", GetText(Resource.String.Lbl_Missing), TypeCall.Audio, CallConstant.CallUserObject);
                        break;
                }

                Stop(intent);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void CloseCall(Intent intent)
        {
            try
            {
                switch (CallConstant.CallType)
                {
                    case "Agora_video_calling_start":
                    case "Agora_video_call_recieve":
                        AgoraVideoCallActivity.Instance?.FinishCall();
                        break;
                    case "Agora_audio_calling_start":
                    case "Agora_audio_call_recieve":
                        AgoraAudioCallActivity.Instance?.FinishCall();
                        break;
                    case "Twilio_video_calling_start":
                    case "Twilio_video_call":
                        TwilioVideoCallActivity.Instance?.FinishCall();
                        break;
                    case "Twilio_audio_calling_start":
                    case "Twilio_audio_call":
                        TwilioAudioCallActivity.Instance?.FinishCall();
                        break;
                }

                Stop(intent);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void Stop(Intent intent)
        {
            try
            {
                CallConstant.CallActive = false;
                CallConstant.IsCallActivityVisible = false;
                CallConstant.IsSpeakerEnabled = false;

                CallConstant.CallType = null;
                CallConstant.CallUserObject = null;

                RingtonePlayer?.StopRingtone();
                RemoveNoti();

                VideoAudioComingCallActivity.CallActivity?.FinishVideoAudio();

                SetBluetoothHeadset(false);
                AudioManager?.AbandonAudioFocus(this);

                StopListenForSensor();

                //LocalBroadcastManager.GetInstance(this).SendBroadcast(new Intent("ACTION_FINISH_CALLING_ACTIVITY"));

                if (intent != null) StopService(intent);
                StopForeground(StopForegroundFlags.Remove);

                StopForeground(true);
                StopSelf();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Sensor

        public void OnWindowFocusChanged(bool hasFocus)
        {
            try
            {
                CallConstant.IsCallActivityVisible = hasFocus;
                if (hasFocus)
                {
                    StartListenForSensor();
                }
                else
                {
                    StopListenForSensor();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void StartListenForSensor()
        {
            try
            {
                if (CallConstant.TypeCall == TypeCall.Video) return;

                if (!AudioHelper.IsHeadsetOn(AudioManager) && !CallConstant.IsSpeakerEnabled)
                {
                    ProximitySensor?.ListenForSensor();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void StopListenForSensor()
        {
            try
            {
                ProximitySensor?.StopListenForSensor();
                ProximitySensor?.Release();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnProximitySensorNear()
        {
            try
            {
                if (CallConstant.IsCallActivityVisible) ProximitySensor?.Acquire();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnProximitySensorFar()
        {
            try
            {
                ProximitySensor?.Release();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region data Noti

        private readonly string NotificationChannelId = "call_ch_1";
        private static NotificationManager MNotificationManager;

        public void RemoveNoti()
        {
            try
            {
                MNotificationManager.Cancel(101);
                OffPowerManagerWakeLock();

                // MNotificationManager.CancelAll();
                // MNotificationManager = null;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public async void CreateNotificationIncoming(Context context, string imageProfile, string title, string longDescription, string notificationId)
        {
            try
            { 
                Intent notificationIntent = new Intent(this, typeof(VideoAudioComingCallActivity));
                if (CallConstant.CallUserObject != null)
                    notificationIntent.PutExtra("callUserObject", JsonConvert.SerializeObject(CallConstant.CallUserObject));
                notificationIntent.PutExtra("type", CallConstant.CallType);

                PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, Build.VERSION.SdkInt >= BuildVersionCodes.M ? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable : PendingIntentFlags.UpdateCurrent);

                Intent answerIntent = new Intent(this, typeof(CallingService));
                answerIntent.PutExtra("type", CallConstant.CallType);
                answerIntent.SetAction(ActionAnswer);
                PendingIntent pAnswerIntent = PendingIntent.GetService(this, 1, answerIntent, Build.VERSION.SdkInt >= BuildVersionCodes.M ? 0 | PendingIntentFlags.Immutable : 0);

                Intent declineIntent = new Intent(this, typeof(CallingService));
                declineIntent.PutExtra("type", CallConstant.CallType);
                declineIntent.SetAction(ActionDecline);
                PendingIntent pDeclineIntent = PendingIntent.GetService(this, 2, declineIntent, Build.VERSION.SdkInt >= BuildVersionCodes.M ? 0 | PendingIntentFlags.Immutable : 0);

                var vibrate = new long[]
                {
                    1000 , 1000 , 1000 , 1000 , 1000
                };
                //var sound = RingtoneManager.GetDefaultUri(RingtoneType.Ringtone);

                NotificationCompat.Builder builder = new NotificationCompat.Builder(context, notificationId);
                builder.SetSmallIcon(Resource.Mipmap.icon)
                    .SetContentTitle(title)
                    .SetContentText(longDescription)
                    .SetChannelId(NotificationChannelId)
                    .SetSound(null)
                    .SetVibrate(vibrate)
                    .SetDefaults((int)NotificationDefaults.All)
                    .SetOngoing(true)
                    .SetAutoCancel(true)
                    .SetOnlyAlertOnce(true)
                    .SetColorized(false)
                    .SetPriority((int)NotificationPriority.High)
                    .SetContentIntent(pendingIntent)
                    .SetLights(Color.Red, 3000, 3000)
                    .SetFullScreenIntent(pendingIntent, true)
                    .AddAction(new NotificationCompat.Action(Resource.Drawable.icon_calls_vector, context.GetText(Resource.String.Lbl_Answer), pAnswerIntent))
                    .AddAction(new NotificationCompat.Action(Resource.Drawable.icon_close_vector, context.GetText(Resource.String.Lbl_Dismiss), pDeclineIntent));

                var drawable = ContextCompat.GetDrawable(context, Resource.Drawable.no_profile_image);
                if (imageProfile.Contains("avatar"))
                {
                    Bitmap bitmap = BitmapUtil.ConvertToBitmap(drawable, 150, 150);
                    builder.SetLargeIcon(bitmap);
                }
                else
                {
                    var bit = await BitmapUtil.GetImageBitmapFromUrl(imageProfile);
                    if (bit != null)
                        builder.SetLargeIcon(bit);
                }

                if ((int)Build.VERSION.SdkInt >= 21)
                    builder.SetCategory(Notification.CategoryCall).SetVisibility(NotificationCompat.VisibilityPublic);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    NotificationImportance importance = NotificationImportance.High;
                    var channel = new NotificationChannel(NotificationChannelId, AppSettings.ApplicationName, importance);
                    channel.EnableLights(true);
                    channel.LightColor = Color.Red;

                    AudioAttributes audioAttributes = new AudioAttributes.Builder()
                        ?.SetContentType(AudioContentType.Music)
                        ?.SetUsage(AudioUsageKind.Notification)
                        ?.Build();
                    channel.SetSound(null, audioAttributes);

                    channel.Description = context.GetText(Resource.String.Lbl_Notification_of_Messages);
                    channel.EnableVibration(true);
                    channel.LockscreenVisibility = NotificationVisibility.Public;

                    MNotificationManager?.CreateNotificationChannel(channel);

                    //var style = new NotificationCompat.CallStyle()
                    //    .SetAnswerButtonColorHint(Color.Green)
                    //    .SetDeclineButtonColorHint(Color.Red);

                    //switch (CallType)
                    //{
                    //    case "Agora_video_call_recieve":
                    //    case "Twilio_video_call":
                    //        style.SetIsVideo(true);
                    //        break;
                    //    case "Agora_audio_call_recieve":
                    //    case "Twilio_audio_call":
                    //        style.SetIsVideo(false);
                    //        break;
                    //}
                     
                    builder.SetStyle(new NotificationCompat.BigTextStyle());
                    //builder.SetStyle(style);
                }

                builder.Notification.Flags |= NotificationFlags.AutoCancel;

                //if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
                //{
                //    builder.SetForegroundServiceBehavior((int)ForegroundService.TypeRemoteMessaging);
                //}

                Notification notificationBuild = builder.Build();
                notificationBuild.Defaults |= NotificationDefaults.Vibrate;
                notificationBuild.Defaults |= NotificationDefaults.Sound;
                //notificationBuild.Color = Color.ParseColor(AppSettings.MainColor);
                //notificationBuild.Flags |= NotificationFlags.Insistent;
                notificationBuild.Sound = null;
                notificationBuild.Vibrate = vibrate;

                MNotificationManager?.Notify(101, notificationBuild);

                try
                { 
                    //if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
                    //{
                    //    StartForeground(101, notificationBuild, ForegroundService.TypeRemoteMessaging);
                    //}
                    //else
                    //{
                    StartForeground(101, notificationBuild);
                    //}
                }
                catch 
                {
                     //
                }
               

                notificationIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.MultipleTask);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                {
                    ActivityOptions options = ActivityOptions.MakeBasic();
                    options.PendingIntentBackgroundActivityLaunchAllowed = true;
                    StartActivity(notificationIntent, options?.ToBundle());
                }
                else
                {
                    StartActivity(notificationIntent);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public async void CreateActiveCallNotification(string notificationId, bool isNewCall)
        {
            try
            {
                if (!isNewCall)
                    RemoveNoti();

                string title = "";
                switch (CallConstant.CallType)
                {
                    case "Agora_video_call_recieve":
                    case "Twilio_video_call":
                        title = GetText(Resource.String.Lbl_Video_call);
                        break;
                    case "Agora_audio_call_recieve":
                    case "Twilio_audio_call":
                        title = GetText(Resource.String.Lbl_Voice_call);
                        break;
                }

                Intent hangupIntent = new Intent(this, typeof(CallingService));
                hangupIntent.PutExtra("type", CallConstant.CallType);
                hangupIntent.SetAction(ActionCloseCall);
                PendingIntent hangupPIntent = PendingIntent.GetService(this, 0, hangupIntent, Build.VERSION.SdkInt >= BuildVersionCodes.M ? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Mutable : PendingIntentFlags.UpdateCurrent);

                NotificationCompat.Builder builder = new NotificationCompat.Builder(this, notificationId);
                builder.SetSmallIcon(Resource.Mipmap.icon)
                    .SetContentTitle(CallConstant.CallUserObject.Fullname)
                    .SetContentText(title)
                    .SetChannelId(NotificationChannelId)
                    .SetSound(null)
                    //.SetVibrate(vibrate)
                    .SetDefaults((int)NotificationDefaults.All)
                    .SetOngoing(true)
                    .SetAutoCancel(true)
                    .SetOnlyAlertOnce(true)
                    .SetColorized(false)
                    .SetPriority((int)NotificationPriority.Default)
                    .AddAction(new NotificationCompat.Action(Resource.Drawable.icon_call_end_vector, GetText(Resource.String.Lbl_HangUp), hangupPIntent));

                builder.SetStyle(new NotificationCompat.BigTextStyle());
                var drawable = ContextCompat.GetDrawable(this, Resource.Drawable.no_profile_image);
                if (CallConstant.CallUserObject.Avater.Contains("avatar"))
                {
                    Bitmap bitmap = BitmapUtil.ConvertToBitmap(drawable, 150, 150);
                    builder.SetLargeIcon(bitmap);
                }
                else
                {
                    var bit = await BitmapUtil.GetImageBitmapFromUrl(CallConstant.CallUserObject.Avater);
                    if (bit != null)
                        builder.SetLargeIcon(bit);
                }

                if ((int)Build.VERSION.SdkInt >= 21)
                    builder.SetCategory(Notification.CategoryCall).SetVisibility(NotificationCompat.VisibilityPublic);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    NotificationImportance importance = NotificationImportance.Default;
                    var channel = new NotificationChannel(NotificationChannelId, AppSettings.ApplicationName, importance);
                    channel.EnableLights(true);
                    channel.LightColor = Color.Red;

                    channel.LockscreenVisibility = NotificationVisibility.Public;

                    MNotificationManager?.CreateNotificationChannel(channel);
                }

                builder.Notification.Flags |= NotificationFlags.AutoCancel;

                //if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
                //{
                //    builder.SetForegroundServiceBehavior((int)ForegroundService.TypeRemoteMessaging);
                //}

                Notification notificationBuild = builder.Build();

                MNotificationManager?.Notify(101, notificationBuild);

                if (isNewCall)
                {
                    try
                    {
                        //if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
                        //{
                        //    StartForeground(101, notificationBuild, ForegroundService.TypeRemoteMessaging);
                        //}
                        //else
                        //{
                        StartForeground(101, notificationBuild);
                        //}
                    }
                    catch
                    {
                        //
                    }
                }
                else
                    MNotificationManager?.Notify(101, notificationBuild);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Audio Manager

        private void SetBluetoothHeadset(bool enable)
        {
            try
            {
                if (enable)
                {
                    AudioManager.Mode = Mode.Normal;
                    AudioManager.BluetoothScoOn = true;
                    AudioManager.StartBluetoothSco();
                    AudioManager.Mode = Mode.InCall;
                }
                else
                {
                    AudioManager.BluetoothScoOn = false;
                    AudioManager.StopBluetoothSco();
                    AudioManager.Mode = Mode.Normal;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void RequestAudioFocus(Stream streamType)
        {
            var audioFocusRequest = AudioManager?.RequestAudioFocus(this, streamType, AudioFocus.Gain);
        }

        public void OnAudioFocusChange(AudioFocus focusChange) { }

        #endregion

        #region PowerManager.WakeLock

        private void InitializePowerManagerWakeLock()
        {
            try
            {
                PowerManager powerMan = (PowerManager)GetSystemService(PowerService);
                PowerManagerWakeLock = powerMan.NewWakeLock(WakeLockFlags.ScreenDim | WakeLockFlags.AcquireCausesWakeup, "wakelockTag");
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void OnPowerManagerWakeLock()
        {
            try
            {
                if (PowerManagerWakeLock != null)
                    PowerManagerWakeLock.Acquire();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void OffPowerManagerWakeLock()
        {
            try
            {
                if (PowerManagerWakeLock != null)
                    PowerManagerWakeLock.Release();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        public void BuildNotification()
        {
            try
            {
                Intent notificationIntent = new Intent(this, typeof(HomeActivity));
                PendingIntent contentIntent = PendingIntent.GetService(this, 0, notificationIntent, PendingIntentFlags.Immutable);

               var Notification = new NotificationCompat.Builder(this, NotificationChannelId)
                    //.SetContentTitle("Foreground Service")
                    //.SetContentText("Running in the background")
                    .SetPriority((int)NotificationPriority.Low)
                    .SetCategory(NotificationPriorityCategory.Alarms.ToString())
                    .SetContentIntent(contentIntent)
                    .SetSmallIcon(Resource.Mipmap.icon)
                    .SetChannelId(NotificationChannelId);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    NotificationImportance importance = NotificationImportance.Low;
                    NotificationChannel mChannel = new NotificationChannel(NotificationChannelId, AppSettings.ApplicationName, importance);
                    MNotificationManager.CreateNotificationChannel(mChannel);
                } 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
        
    }
}
