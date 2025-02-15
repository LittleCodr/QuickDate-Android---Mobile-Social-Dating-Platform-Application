using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Anjo.Android.AgoraIO.AgoraDynamicKey.Media;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Google.Android.Material.Dialog;
using IO.Agora.Rtc2;
using Newtonsoft.Json;
using QuickDate.Activities.Base;
using QuickDate.Activities.Call.Tools;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient;
using QuickDateClient.Classes.Call;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace QuickDate.Activities.Call.Agora
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", AutoRemoveFromRecents = true, ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize, ScreenOrientation = ScreenOrientation.Portrait)]
    public class AgoraAudioCallActivity : AppCompatActivity
    {
        #region Variables Basic

        public static AgoraAudioCallActivity Instance;
        private string CallType = "0", Token = "";
        private DataCallObject CallUserObject;
        public RtcEngine AgoraEngine;
        public AgoraRtcCallHandler AgoraHandler;

        private ImageView IconBack;
        private LinearLayout EndCallButton, SpeakerAudioButton, MuteAudioButton;
        private ImageView IconEndCall, IconSpeaker, IconMute;
        private ImageView UserImageView;
        private TextView UserNameTextView, DurationTextView;
        private TextView IconSignal;

        private int CountSecondsOfOutGoingCall;
        private Timer TimerRequestWaiter, TimerSound;

        private HomeActivity GlobalContext;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);

                Window?.AddFlags(WindowManagerFlags.KeepScreenOn);

                // Create your application here
                SetContentView(Resource.Layout.CallAudioLayout);
                Instance = this;
                GlobalContext = HomeActivity.GetInstance();

                //Get Value And Set Toolbar
                InitComponent();
                InitBackPressed();
                CheckCallPermission();
                CallConstant.CallActive = true;
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

        protected override void OnStart()
        {
            try
            {
                base.OnStart();
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

        protected override void OnRestart()
        {
            try
            {
                base.OnRestart();
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
                CallConstant.CallActive = false;
                StopRtc();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                CallConstant.CallActive = false;
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);
            CallingService.GetService().OnWindowFocusChanged(hasFocus);
        }

        #endregion

        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                FinishCallingService();
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
                IconBack = FindViewById<ImageView>(Resource.Id.icon_back);
                EndCallButton = FindViewById<LinearLayout>(Resource.Id.EndCallButtonLayout);
                IconEndCall = FindViewById<ImageView>(Resource.Id.iconEndCall);

                SpeakerAudioButton = FindViewById<LinearLayout>(Resource.Id.SpeakerButtonLayout);
                IconSpeaker = FindViewById<ImageView>(Resource.Id.iconSpeaker);

                MuteAudioButton = FindViewById<LinearLayout>(Resource.Id.MuteButtonLayout);
                IconMute = FindViewById<ImageView>(Resource.Id.iconMute);

                UserImageView = FindViewById<ImageView>(Resource.Id.userImage);
                UserNameTextView = FindViewById<TextView>(Resource.Id.name);

                IconSignal = FindViewById<TextView>(Resource.Id.icon_signal);
                DurationTextView = FindViewById<TextView>(Resource.Id.time);

                IconBack.SetImageResource(AppSettings.FlowDirectionRightToLeft ? Resource.Drawable.icon_back_arrow_right : Resource.Drawable.icon_back_arrow_left);

                SpeakerAudioButton.Selected = false;

                CallConstant.IsSpeakerEnabled = false;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void InitBackPressed()
        {
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                {
                    OnBackInvokedDispatcher.RegisterOnBackInvokedCallback(0, new BackCallAppBase2(this, "AgoraAudioCallActivity"));
                }
                else
                {
                    OnBackPressedDispatcher.AddCallback(new BackCallAppBase1(this, "AgoraAudioCallActivity", true));
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
                    SpeakerAudioButton.Click += SpeakerAudioButtonOnClick;
                    EndCallButton.Click += EndCallButtonOnClick;
                    MuteAudioButton.Click += MuteAudioButtonOnClick;
                }
                else
                {
                    SpeakerAudioButton.Click -= SpeakerAudioButtonOnClick;
                    EndCallButton.Click -= EndCallButtonOnClick;
                    MuteAudioButton.Click -= MuteAudioButtonOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        private void MuteAudioButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (MuteAudioButton.Selected)
                {
                    MuteAudioButton.Selected = false;
                    IconMute.SetImageResource(Resource.Drawable.icon_mic_vector);
                }
                else
                {
                    MuteAudioButton.Selected = true;
                    IconMute.SetImageResource(Resource.Drawable.icon_microphone_mute);
                }
                AgoraEngine?.MuteLocalAudioStream(MuteAudioButton.Selected);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void EndCallButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                FinishCallingService();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void SpeakerAudioButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                //Speaker
                if (SpeakerAudioButton.Selected)
                {
                    SpeakerAudioButton.Selected = false;
                    IconSpeaker.SetImageResource(Resource.Drawable.icon_volume_off_vector);
                }
                else
                {
                    SpeakerAudioButton.Selected = true;
                    IconSpeaker.SetImageResource(Resource.Drawable.icon_volume_up_vector);
                }

                CallConstant.IsSpeakerEnabled = SpeakerAudioButton.Selected;
                AgoraEngine?.SetEnableSpeakerphone(SpeakerAudioButton.Selected);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Check Call Permission

        private void CheckCallPermission()
        {
            try
            {
                if ((int)Build.VERSION.SdkInt >= 23)
                {
                    if (CheckSelfPermission(Manifest.Permission.RecordAudio) == Permission.Granted && CheckSelfPermission(Manifest.Permission.ModifyAudioSettings) == Permission.Granted)
                    {
                        InitAgoraCall();
                    }
                    else
                    {
                        new PermissionsController(this).RequestPermission(102);
                    }
                }
                else
                {
                    InitAgoraCall();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Permissions

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (grantResults.Length <= 0 || grantResults[0] != Permission.Granted)
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denied), ToastLength.Short)?.Show();
                    return;
                }

                if (requestCode == 102)
                {
                    InitAgoraCall();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Agora  

        private async void InitAgoraCall()
        {
            try
            {
                CallType = Intent?.GetStringExtra("type") ?? ""; // Agora_audio_call_recieve , Agora_audio_calling_start

                if (!string.IsNullOrEmpty(Intent?.GetStringExtra("callUserObject")))
                    CallUserObject = JsonConvert.DeserializeObject<DataCallObject>(Intent?.GetStringExtra("callUserObject") ?? "");

                InitializeAgoraEngine();

                if (CallType == "Agora_audio_call_recieve")
                {
                    if (!string.IsNullOrEmpty(CallUserObject.ToId))
                        Load_userWhenCall();

                    Token = CallUserObject.AccessToken;

                    DurationTextView.Text = GetText(Resource.String.Lbl_Waiting_for_answer);

                    var (apiStatus, respond) = await RequestsAsync.Call.SendAnswerCallAsync(CallUserObject.Id, TypeCall.Audio);
                    if (apiStatus == 200)
                    {
                        Methods.AudioRecorderAndPlayer.StopAudioFromAsset();

                        JoinChannel(Token, CallUserObject.RoomName);

                        //HomeActivity.AddCallToListAndSend("Answered", GetText(Resource.String.Lbl_Incoming), TypeCall.Audio, CallUserObject);
                    }
                    //else Methods.DisplayReportResult(this, respond);
                }
                else if (CallType == "Agora_audio_calling_start")
                {
                    DurationTextView.Text = GetText(Resource.String.Lbl_Calling);

                    // Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("outgoin_call.mp3", "left");

                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.AgoraChatAppCertificate))
                    {
                        string channelName = "room_" + Methods.Time.CurrentTimeMillis();
                        uint uid = 0;
                        uint expirationTimeInSeconds = 3600;

                        uint timestamp = expirationTimeInSeconds + (uint)Methods.Time.CurrentTimeMillis();

                        Token = RtcTokenBuilder.BuildTokenWithUid(ListUtils.SettingsSiteList?.AgoraChatAppId, ListUtils.SettingsSiteList?.AgoraChatAppCertificate, channelName, uid, RtcTokenBuilder.Role.RolePublisher, timestamp);
                    }

                    StartApiService();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void InitializeAgoraEngine()
        {
            try
            {
                AgoraHandler = new AgoraRtcCallHandler(this);
                AgoraEngine = RtcEngine.Create(this, ListUtils.SettingsSiteList?.AgoraChatAppId, AgoraHandler);
                AgoraEngine?.EnableWebSdkInteroperability(true);
                AgoraEngine?.SetChannelProfile(Constants.ChannelProfileCommunication);
                AgoraEngine?.EnableAudio();
                AgoraEngine?.DisableVideo();

                // Enable the dual stream mode
                AgoraEngine?.SetDualStreamMode(Constants.SimulcastStreamMode.AutoSimulcastStream);
                // Set audio profile and audio scenario.
                AgoraEngine?.SetAudioProfile(Constants.AudioProfileDefault, Constants.AudioScenarioGameStreaming);
            }
            catch (Exception e)
            {
                //Colud not create RtcEngine
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void StopRtc()
        {
            try
            {
                AgoraEngine?.RemoveHandler(AgoraHandler);
                AgoraEngine?.StopPreview();
                AgoraEngine?.LeaveChannel();
                AgoraHandler = null;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void JoinChannel(string accessToken, string channelName)
        {
            try
            {
                AgoraEngine?.JoinChannel(accessToken, channelName, string.Empty, 0);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void Load_userWhenCall()
        {
            try
            {
                UserNameTextView.Text = CallUserObject.Fullname;

                //profile_picture
                Glide.With(this).Load(CallUserObject.Avater).Apply(new RequestOptions().CircleCrop().Placeholder(Resource.Drawable.ImagePlacholder)).Into(UserImageView);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void StartApiService()
        {
            if (!Methods.CheckConnectivity())
                Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { CreateNewCall });
        }

        private async Task CreateNewCall()
        {

            if (!Methods.CheckConnectivity())
                return;

            Load_userWhenCall();
            var (apiStatus, respond) = await RequestsAsync.Call.CreateNewCallAgoraAsync(UserDetails.UserId.ToString(), CallUserObject.ToId, TypeCall.Audio);
            if (apiStatus == 200)
            {
                if (respond is CreateNewCallAgoraObject result)
                {
                    CallUserObject.Id = result.Id;
                    Token = CallUserObject.AccessToken = result.Token;
                    CallUserObject.RoomName = result.RoomName;

                    //HomeActivity.AddCallToListAndSend("Answered", GetText(Resource.String.Lbl_Outgoing), TypeCall.Audio, CallUserObject);

                    TimerRequestWaiter = new Timer { Interval = 3000 };
                    TimerRequestWaiter.Elapsed += TimerCallRequestAnswer_Waiter_Elapsed;
                    TimerRequestWaiter.Start();
                }
            }
            else
            {
                FinishCallingService();
                //Methods.DisplayReportResult(this, respond);
            }
        }

        private void TimerCallRequestAnswer_Waiter_Elapsed(object sender, ElapsedEventArgs e)
        {
            RunOnUiThread(CheckForAnswer);
        }

        private async void CheckForAnswer()
        {
            try
            {
                var (apiStatus, respond) = await RequestsAsync.Call.CheckForAnswerAsync(CallUserObject.Id, TypeCall.Audio);
                if (apiStatus == 200)
                {
                    if (respond is AnswerCallObject agoraObject)
                    {
                        //Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
                        CallingService.GetService().RingtonePlayer?.StopRingtone();

                        JoinChannel(Token, CallUserObject.RoomName);

                        if (TimerRequestWaiter != null)
                        {
                            TimerRequestWaiter.Enabled = false;
                            TimerRequestWaiter.Stop();
                            TimerRequestWaiter.Close();
                        }

                        //HomeActivity.AddCallToListAndSend("Answered", GetText(Resource.String.Lbl_Outgoing), TypeCall.Audio, CallUserObject);
                    }
                    else if (respond is InfoObject callObject)
                    {
                        if (callObject.Message == "calling" && CountSecondsOfOutGoingCall < 1000)
                            CountSecondsOfOutGoingCall += 5;
                        else if (callObject.Message == "calling")
                        {
                            //Call Is inactive 
                            if (TimerRequestWaiter != null)
                            {
                                TimerRequestWaiter.Enabled = false;
                                TimerRequestWaiter.Stop();
                                TimerRequestWaiter.Close();
                            }

                            //HomeActivity.AddCallToListAndSend("Cancel", GetText(Resource.String.Lbl_Missing), TypeCall.Audio, CallUserObject);

                            FinishCallingService();
                        }
                        else if (callObject.Message == "declined")
                        {
                            //Call Is inactive 
                            if (TimerRequestWaiter != null)
                            {
                                TimerRequestWaiter.Enabled = false;
                                TimerRequestWaiter.Stop();
                                TimerRequestWaiter.Close();
                            }

                            //HomeActivity.AddCallToListAndSend("Cancel", GetText(Resource.String.Lbl_Missing), TypeCall.Audio, CallUserObject);

                            FinishCallingService();
                        }
                        else if (callObject.Message == "no_answer")
                        {
                            //Call Is inactive 
                            if (TimerRequestWaiter != null)
                            {
                                TimerRequestWaiter.Enabled = false;
                                TimerRequestWaiter.Stop();
                                TimerRequestWaiter.Close();
                            }

                            //HomeActivity.AddCallToListAndSend("NoAnswer", GetText(Resource.String.Lbl_Missing), TypeCall.Audio, CallUserObject);

                            FinishCallingService();
                        }
                        //else if (callObject.Message == "busy_another_call")
                        //{
                        //    //Call Is inactive 
                        //    if (TimerRequestWaiter != null)
                        //    {
                        //        TimerRequestWaiter.Enabled = false;
                        //        TimerRequestWaiter.Stop();
                        //        TimerRequestWaiter.Close();
                        //    }

                        //    ToastUtils.ShowToast(this, GetString(Resource.String.Lbl_BusyAnotherCall), ToastLength.Long);
                        //    //HomeActivity.AddCallToListAndSend("busy_another_call", GetText(Resource.String.Lbl_Missing), TypeCall.Audio, CallUserObject);

                        //    FinishCallingService();
                        //}
                        else
                        {
                            //Call Is inactive 
                            if (TimerRequestWaiter != null)
                            {
                                TimerRequestWaiter.Enabled = false;
                                TimerRequestWaiter.Stop();
                                TimerRequestWaiter.Close();
                            }

                            //HomeActivity.AddCallToListAndSend("NoAnswer", GetText(Resource.String.Lbl_Missing), TypeCall.Audio, CallUserObject);

                            FinishCallingService();
                            //Methods.DisplayReportResult(this, respond);
                        }
                    }
                }

            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }

        }

        #endregion

        #region Agora Rtc Handler

        public void OnConnectionLost()
        {
            RunOnUiThread(() =>
            {
                try
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_Lost_Connection), ToastLength.Short)?.Show();
                    FinishCallingService();
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                    FinishCallingService();
                }
            });
        }

        public void OnUserOffline(int uid, int reason)
        {
            RunOnUiThread(async () =>
            {
                try
                {
                    //Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
                    //Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("Error.mp3");
                    DurationTextView.Text = GetText(Resource.String.Lbl_Lost_his_connection);
                    await Task.Delay(2000);
                    FinishCallingService();
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                    FinishCallingService();
                }
            });
        }

        public void OnNetworkQuality(int uid, int txQuality, int rxQuality)
        {
            RunOnUiThread(() => UpdateNetworkStatus(rxQuality));
        }

        public void OnUserJoined(int uid, int elapsed)
        {
            RunOnUiThread(() =>
            {
                try
                {
                    DurationTextView.Text = GetText(Resource.String.Lbl_Please_wait);
                    //Methods.AudioRecorderAndPlayer.StopAudioFromAsset();

                    TimerSound = new Timer();
                    TimerSound.Interval = 1000;
                    TimerSound.Elapsed += TimerSoundOnElapsed;
                    TimerSound.Start();
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                    FinishCallingService();
                }
            });
        }

        private string TimeCall;
        private bool IsMuted;
        private void TimerSoundOnElapsed(object sender, ElapsedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                try
                {
                    if (!IsMuted)
                    {
                        //Write your own duration function here 
                        TimeCall = TimeSpan.FromSeconds(e.SignalTime.Second).ToString(@"hh\:mm\:ss");
                        DurationTextView.Text = TimeCall;
                    }
                }
                catch (Exception exception)
                {
                    Methods.DisplayReportResultTrack(exception);
                }
            });
        }

        public void OnJoinChannelSuccess(string channel, int uid, int elapsed)
        {

        }

        public void OnUserMuteAudio(int uid, bool muted)
        {
            RunOnUiThread(() =>
            {
                try
                {
                    IsMuted = muted;
                    if (muted)
                    {
                        DurationTextView.Text = GetText(Resource.String.Lbl_Muted_his_video);
                    }
                    else
                    {
                        DurationTextView.Text = TimeCall;
                    }
                }
                catch (Exception exception)
                {
                    Methods.DisplayReportResultTrack(exception);
                }
            });
        }

        public void OnError(int err)
        {
            RunOnUiThread(() =>
            {
                try
                {
                    Console.WriteLine("Error code " + err);

                    var dialog = new MaterialAlertDialogBuilder(this);
                    dialog.SetTitle(GetText(Resource.String.Lbl_ErrorCall_Code) + " " + err);
                    dialog.SetMessage(GetText(Resource.String.Lbl_ErrorCall_Message));
                    dialog.SetPositiveButton(GetText(Resource.String.Lbl_Ok), (materialDialog, action) =>
                    {
                        try
                        {
                            FinishCallingService();
                        }
                        catch (Exception e)
                        {
                            Methods.DisplayReportResultTrack(e);
                        }
                    });
                    dialog.SetNeutralButton(GetText(Resource.String.Lbl_ContactUs), (materialDialog, action) =>
                    {
                        try
                        {
                            new IntentController(this).OpenBrowserFromApp(InitializeQuickDate.WebsiteUrl + "/contact-us");
                            FinishCallingService();
                        }
                        catch (Exception e)
                        {
                            Methods.DisplayReportResultTrack(e);
                        }
                    });
                    dialog.Show();
                }
                catch (Exception e)
                {
                    FinishCallingService();
                    Methods.DisplayReportResultTrack(e);
                }
            });
        }

        public void OnLastmileQuality(int quality)
        {
            RunOnUiThread(() => UpdateNetworkStatus(quality));
        }

        public void UpdateNetworkStatus(int quality)
        {
            try
            {
                IconSignal.Visibility = ViewStates.Visible;
                string icon = string.Empty;
                switch (quality)
                {
                    case Constants.QualityExcellent:
                        icon = "Excellent";
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, IconSignal, FontAwesomeIcon.Signal);
                        break;
                    case Constants.QualityGood:
                        icon = "Good";
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, IconSignal, FontAwesomeIcon.Signal4);
                        break;
                    case Constants.QualityPoor:
                        icon = "Poor";
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, IconSignal, FontAwesomeIcon.SignalAlt3);
                        break;
                    case Constants.QualityBad:
                        icon = "Bad";
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, IconSignal, FontAwesomeIcon.SignalAlt2);
                        break;
                    case Constants.QualityVbad:
                        icon = "Very Bad";
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, IconSignal, FontAwesomeIcon.SignalAlt1);
                        break;
                    case Constants.QualityDown:
                        icon = "Down";
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, IconSignal, FontAwesomeIcon.SignalAltSlash);
                        break;
                    default:
                        icon = "Unknown";
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, IconSignal, FontAwesomeIcon.SignalAltSlash);
                        break;
                }

                Console.WriteLine("Quality : " + icon);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }

        }

        #endregion

        public void FinishCallingService()
        {
            try
            {
                Intent hangupIntent = new Intent(this, typeof(CallingService));
                hangupIntent.PutExtra("type", CallType);
                hangupIntent.SetAction(CallingService.ActionCloseCall);
                StartService(hangupIntent);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void FinishCall()
        {
            try
            {
                //Close Api Starts here >> 

                if (!Methods.CheckConnectivity())
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                else
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Call.DeleteCallAsync(CallUserObject.Id, TypeCall.Audio) });

                if (AgoraEngine != null)
                {
                    AgoraEngine.LeaveChannel();
                    AgoraEngine.RemoveHandler(AgoraHandler);
                    AgoraEngine.Dispose();
                }

                AgoraEngine = null!;

                if (TimerSound != null)
                {
                    TimerSound.Enabled = false;
                    TimerSound.Stop();
                    TimerSound.Close();
                }

                TimerSound = null;

                if (TimerRequestWaiter != null)
                {
                    TimerRequestWaiter.Enabled = false;
                    TimerRequestWaiter.Stop();
                    TimerRequestWaiter.Close();
                }
                TimerRequestWaiter = null;

                CallConstant.CallActive = false;
                //Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
                Finish();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                CallConstant.CallActive = false;
                Finish();
            }
        }

    }
}