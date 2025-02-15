using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Com.Twilio.Video;
using Newtonsoft.Json;
using QuickDate.Activities.Base;
using QuickDate.Activities.Call.Tools;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Call;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace QuickDate.Activities.Call.Twilio
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", AutoRemoveFromRecents = true, ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class TwilioAudioCallActivity : AppCompatActivity, TwilioVideoHelper.IListener
    {
        #region Variables Basic

        public static TwilioAudioCallActivity Instance;
        private ImageView IconBack;
        private string CallType = "0";
        private DataCallObject CallUserObject;
        public TwilioVideoHelper TwilioVideo;

        private LinearLayout EndCallButton, SpeakerAudioButton, MuteAudioButton;
        private ImageView IconEndCall, IconSpeaker, IconMute;
        private ImageView UserImageView;
        private TextView UserNameTextView, DurationTextView;
        private TextView IconSignal;

        private Timer TimerRequestWaiter, TimerSound;
        private LocalVideoTrack LocalVideoTrack;
        private VideoTrack UserVideoTrack;
        private bool DataUpdated;
        private int CountSecondsOfOutGoingCall;
        private string LocalVideoTrackId, RemoteVideoTrackId;
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
                UpdateState();
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
                UpdateState();
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
                DataUpdated = false;
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
                TwilioVideo = TwilioVideoHelper.GetOrCreate(this, TypeCall.Audio);
                UpdateState();
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
                if (LocalVideoTrack != null)
                {
                    LocalVideoTrack.Release();
                    LocalVideoTrack = null;
                }

                Instance = null;

                CallConstant.CallActive = false;
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
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
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
                MuteAudioButton.Selected = false;

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
                    OnBackInvokedDispatcher.RegisterOnBackInvokedCallback(0, new BackCallAppBase2(this, "TwilioAudioCallActivity"));
                }
                else
                {
                    OnBackPressedDispatcher.AddCallback(new BackCallAppBase1(this, "TwilioAudioCallActivity", true));
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

                TwilioVideo.Mute(MuteAudioButton.Selected);
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
                TwilioVideo.Speaker(SpeakerAudioButton.Selected);
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
                        InitTwilioCall();
                    }
                    else
                    {
                        new PermissionsController(this).RequestPermission(102);
                    }
                }
                else
                {
                    InitTwilioCall();
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
                    InitTwilioCall();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Twilio  

        private async void InitTwilioCall()
        {
            try
            {
                CallType = Intent?.GetStringExtra("type") ?? ""; // Twilio_video_call , Twilio_audio_call,Agora_video_call_recieve,Agora_audio_call_recieve

                if (!string.IsNullOrEmpty(Intent?.GetStringExtra("callUserObject")))
                    CallUserObject = JsonConvert.DeserializeObject<DataCallObject>(Intent?.GetStringExtra("callUserObject") ?? "");

                switch (CallType)
                {
                    case "Twilio_audio_call":
                        {
                            if (!string.IsNullOrEmpty(CallUserObject?.ToId))
                                Load_userWhenCall();

                            TwilioVideo = TwilioVideoHelper.GetOrCreate(this, TypeCall.Audio);
                            UpdateState();
                            DurationTextView.Text = GetText(Resource.String.Lbl_Waiting_for_answer);

                            var (apiStatus, respond) = await RequestsAsync.Call.SendAnswerCallAsync(CallUserObject.Id, TypeCall.Audio);
                            if (apiStatus == 200)
                            {
                                ConnectToRoom();

                                //HomeActivity.AddCallToListAndSend("Answered", GetText(Resource.String.Lbl_Incoming), TypeCall.Audio, CallUserObject);
                            }
                            //else Methods.DisplayReportResult(this, respond);

                            break;
                        }
                    case "Twilio_audio_calling_start":
                        DurationTextView.Text = GetText(Resource.String.Lbl_Calling);
                        TwilioVideo = TwilioVideoHelper.GetOrCreate(this, TypeCall.Audio);

                        //Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("outgoin_call.mp3", "left");

                        StartApiService();

                        UpdateState();
                        break;
                }
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
                GlideImageLoader.LoadImage(this, CallUserObject.Avater, UserImageView, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
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
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { LoadProfileFromUserId });
        }

        private async Task LoadProfileFromUserId()
        {
            Load_userWhenCall();
            var (apiStatus, respond) = await RequestsAsync.Call.CreateNewCallAsync(CallUserObject.ToId, TypeCall.Audio);
            if (apiStatus == 200)
            {
                if (respond is CreateNewCallObject result)
                {
                    CallUserObject.Id = result.Data.Id.ToString();
                    CallUserObject.AccessToken = result.Data.AccessToken;
                    CallUserObject.AccessToken2 = result.Data.AccessToken2;
                    CallUserObject.RoomName = result.Data.RoomName;

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
                    //Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
                    CallingService.GetService().RingtonePlayer?.StopRingtone();

                    if (TwilioVideo != null)
                    {
                        TwilioVideo.UpdateToken(CallUserObject.AccessToken2);
                        TwilioVideo.JoinRoom(this, TypeCall.Audio, CallUserObject.RoomName);
                    }

                    if (TimerRequestWaiter != null)
                    {
                        TimerRequestWaiter.Enabled = false;
                        TimerRequestWaiter.Stop();
                        TimerRequestWaiter.Close();
                    }

                    //HomeActivity.AddCallToListAndSend("Answered", GetText(Resource.String.Lbl_Outgoing), TypeCall.Audio, CallUserObject);

                }
                else if (apiStatus == 300)
                {
                    if (respond is InfoObject callObject)
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

                            //HomeActivity.AddCallToListAndSend("Cancel", GetText(Resource.String.Lbl_Outgoing), TypeCall.Audio, CallUserObject);
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

        #region TwilioVideoHelper.IListener

        public void SetLocalVideoTrack(LocalVideoTrack track)
        {
            try
            {
                if (LocalVideoTrack == null)
                {
                    LocalVideoTrack = track;
                    var trackId = track?.Name;
                    if (LocalVideoTrackId == trackId)
                    {
                        LocalVideoTrack.Enable(false);
                    }
                    else
                    {
                        LocalVideoTrackId = trackId;
                        LocalVideoTrack.Enable(false);
                    }
                }
                else
                {
                    if (LocalVideoTrack.IsEnabled)
                        LocalVideoTrack.Enable(false);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetRemoteVideoTrack(VideoTrack track)
        {
            try
            {
                var trackId = track?.Name;

                if (RemoteVideoTrackId == trackId)
                    return;

                RemoteVideoTrackId = trackId;
                if (UserVideoTrack == null)
                {
                    UserVideoTrack = track;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void RemoveLocalVideoTrack(LocalVideoTrack track)
        {
            try
            {
                SetLocalVideoTrack(null);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void RemoveRemoteVideoTrack(VideoTrack track)
        {

        }

        public void OnRoomConnected(string roomId)
        {

        }

        public void OnRoomDisconnected(TwilioVideoHelper.StopReason reason)
        {
            try
            {
                Toast.MakeText(this, GetString(Resource.String.Lbl_Room_Disconnected), ToastLength.Short)?.Show();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnParticipantConnected(string participantId)
        {
            try
            {
                //Lbl_Connected
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
            }
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

        public void OnParticipantDisconnected(string participantId)
        {
            RunOnUiThread(async () =>
            {
                try
                {
                    DurationTextView.Text = GetText(Resource.String.Lbl_User_Lost_Connection);
                    await Task.Delay(2000);
                    FinishCallingService();
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            });
        }

        public void OnLastQualityLevel(RemoteParticipant remoteParticipant, NetworkQualityLevel networkQualityLevel)
        {
            try
            {
                IconSignal.Visibility = ViewStates.Visible;
                string icon = string.Empty;
                if (networkQualityLevel == NetworkQualityLevel.NetworkQualityLevelFive)
                {
                    icon = "Excellent";
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, IconSignal, FontAwesomeIcon.Signal);
                }
                else if (networkQualityLevel == NetworkQualityLevel.NetworkQualityLevelFour)
                {
                    icon = "Good";
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, IconSignal, FontAwesomeIcon.Signal4);
                }
                else if (networkQualityLevel == NetworkQualityLevel.NetworkQualityLevelThree)
                {
                    icon = "Poor";
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, IconSignal, FontAwesomeIcon.SignalAlt3);
                }
                else if (networkQualityLevel == NetworkQualityLevel.NetworkQualityLevelTwo)
                {
                    icon = "Bad";
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, IconSignal, FontAwesomeIcon.SignalAlt2);
                }
                else if (networkQualityLevel == NetworkQualityLevel.NetworkQualityLevelOne)
                {
                    icon = "Very Bad";
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, IconSignal, FontAwesomeIcon.SignalAlt1);
                }
                else if (networkQualityLevel == NetworkQualityLevel.NetworkQualityLevelZero)
                {
                    icon = "Down";
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, IconSignal, FontAwesomeIcon.SignalAltSlash);
                }
                else
                {
                    icon = "Unknown";
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, IconSignal, FontAwesomeIcon.SignalAltSlash);
                }

                Console.WriteLine("Quality : " + icon);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnMuteAudioTrack(RemoteAudioTrackPublication trackPublication, bool muted)
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

        #endregion

        private void ConnectToRoom()
        {
            TwilioVideo?.UpdateToken(CallUserObject.AccessToken);
            TwilioVideo?.JoinRoom(this, TypeCall.Audio, CallUserObject.RoomName);
        }

        public override bool OnSupportNavigateUp()
        {
            FinishCallingService();
            return true;
        }

        void UpdateState()
        {
            try
            {
                if (DataUpdated)
                    return;

                DataUpdated = true;

                TwilioVideo?.Bind(this);

                UpdatingState();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void UpdatingState()
        {
        }

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

                if (TwilioVideo.ClientIsReady)
                {
                    TwilioVideo.Unbind(this);
                    TwilioVideo.FinishCall();
                }

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

                //Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
                Finish();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

    }
}