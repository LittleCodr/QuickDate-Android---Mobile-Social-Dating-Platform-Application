using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Anjo.Android.AgoraIO.AgoraDynamicKey.Media;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Google.Android.Material.Dialog;
using IO.Agora.Rtc2;
using IO.Agora.Rtc2.Video;
using Newtonsoft.Json;
using QuickDate.Activities.Base;
using QuickDate.Activities.Call.Tools;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Controller;
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
using Exception = System.Exception;

namespace QuickDate.Activities.Call.Agora
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", AutoRemoveFromRecents = true, ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.ScreenLayout | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize, SupportsPictureInPicture = true)]
    public class AgoraVideoCallActivity : AppCompatActivity
    {
        #region Variables Basic

        public static AgoraVideoCallActivity Instance;
        private string CallType = "0", Token = "";
        private DataCallObject CallUserObject;
        public RtcEngine AgoraEngine;
        public AgoraRtcCallHandler AgoraHandler;

        private const int MaxLocalVideoDimension = 150;
        private bool IsVideoEnabled = true;
        private SurfaceView LocalVideoView, RemoteSurfaceView;

        //Controls
        private RelativeLayout MainVideoViewLayout;

        private FrameLayout RemoteVideoViewContainer, LocalVideoContainer, LocalVideoOverlay;
        private ImageView IconMuteVoiceLocalVideo, IconMuteLocalVideo;
        private ImageView IconMuteVoiceRemoteVideo, IconMuteRemoteVideo;

        private LinearLayout TopControlLayout;
        private FrameLayout BottomControlLayout;
        private ImageView IconBack; //wael add call in background

        private LinearLayout SwitchButtonLayout, EndCallButton, StopVideoButton, MuteAudioButton;
        private ImageView IconEndCall, IconSwitch, IconStopVideo, IconMute;
        private ImageView UserImageView;
        private TextView UserNameTextView, DurationTextView;
        private TextView IconSignal;

        private int CountSecondsOfOutGoingCall;
        private Timer TimerRequestWaiter, TimerSound;

        private HomeActivity GlobalContext;

        private PictureInPictureParams PictureInPictureParams;
        private RelativeLayout ThumbnailVideo;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this, true);

                Window?.AddFlags(WindowManagerFlags.KeepScreenOn);

                // Create your application here
                SetContentView(Resource.Layout.AgoraVideoCallActivityLayout);
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

        protected override void OnStop()
        {
            try
            {
                base.OnStop();
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
                BackPressed();
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
                MainVideoViewLayout = (RelativeLayout)FindViewById(Resource.Id.activity_video_chat_view);
                MainVideoViewLayout.Tag = "show";

                RemoteVideoViewContainer = (FrameLayout)FindViewById(Resource.Id.remote_video_view_container);

                TopControlLayout = FindViewById<LinearLayout>(Resource.Id.top_control);
                IconBack = FindViewById<ImageView>(Resource.Id.icon_back);
                IconBack.SetImageResource(AppSettings.FlowDirectionRightToLeft ? Resource.Drawable.icon_back_arrow_right : Resource.Drawable.icon_back_arrow_left);

                ThumbnailVideo = FindViewById<RelativeLayout>(Resource.Id.local_video_container);

                LocalVideoContainer = (FrameLayout)FindViewById(Resource.Id.local_video_view_container);

                LocalVideoOverlay = (FrameLayout)FindViewById(Resource.Id.local_video_overlay);
                IconMuteVoiceLocalVideo = FindViewById<ImageView>(Resource.Id.iconMuteVoice_local_video);
                IconMuteLocalVideo = FindViewById<ImageView>(Resource.Id.iconMute_local_video);

                IconMuteVoiceRemoteVideo = FindViewById<ImageView>(Resource.Id.iconMuteVoice_remote_video);
                IconMuteRemoteVideo = FindViewById<ImageView>(Resource.Id.iconMute_remote_video);

                BottomControlLayout = FindViewById<FrameLayout>(Resource.Id.bottom_control);
                SwitchButtonLayout = FindViewById<LinearLayout>(Resource.Id.SwitchButtonLayout);
                IconSwitch = FindViewById<ImageView>(Resource.Id.iconSwitch);

                EndCallButton = FindViewById<LinearLayout>(Resource.Id.EndCallButtonLayout);
                IconEndCall = FindViewById<ImageView>(Resource.Id.iconEndCall);

                StopVideoButton = FindViewById<LinearLayout>(Resource.Id.StopVideoButtonLayout);
                IconStopVideo = FindViewById<ImageView>(Resource.Id.iconStopVideo);

                MuteAudioButton = FindViewById<LinearLayout>(Resource.Id.MuteButtonLayout);
                IconMute = FindViewById<ImageView>(Resource.Id.iconMute);

                UserImageView = FindViewById<ImageView>(Resource.Id.userImage);
                UserNameTextView = FindViewById<TextView>(Resource.Id.name);

                IconSignal = FindViewById<TextView>(Resource.Id.icon_signal);
                DurationTextView = FindViewById<TextView>(Resource.Id.time);

                if (!PackageManager.HasSystemFeature(PackageManager.FeaturePictureInPicture))
                {
                    //PictureInToPictureButton.Visibility = ViewStates.Gone;
                }
                else
                {
                    var pictureInPicture = new PictureInPictureParams.Builder().SetAspectRatio(new Rational(9, 16));
                    //.SetSourceRectHint(sourceRectHint)
                    if ((int)Build.VERSION.SdkInt >= 31)
                        pictureInPicture.SetAutoEnterEnabled(true);

                    PictureInPictureParams = pictureInPicture?.Build();
                    SetPictureInPictureParams(PictureInPictureParams);
                }
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
                    OnBackInvokedDispatcher.RegisterOnBackInvokedCallback(0, new BackCallAppBase2(this, "AgoraVideoCallActivity"));
                }
                else
                {
                    OnBackPressedDispatcher.AddCallback(new BackCallAppBase1(this, "AgoraVideoCallActivity", true));
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
                    MainVideoViewLayout.Click += MainVideoViewLayoutOnClick;
                    SwitchButtonLayout.Click += SwitchCamButtonOnClick;
                    StopVideoButton.Click += StopVideoButtonOnClick;
                    EndCallButton.Click += EndCallButtonOnClick;
                    MuteAudioButton.Click += MuteAudioButtonOnClick;
                    IconBack.Click += PictureInToPictureButtonOnClick;
                }
                else
                {
                    MainVideoViewLayout.Click -= MainVideoViewLayoutOnClick;
                    SwitchButtonLayout.Click -= SwitchCamButtonOnClick;
                    StopVideoButton.Click -= StopVideoButtonOnClick;
                    EndCallButton.Click -= EndCallButtonOnClick;
                    MuteAudioButton.Click -= MuteAudioButtonOnClick;
                    IconBack.Click -= PictureInToPictureButtonOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        private void MainVideoViewLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                var stat = MainVideoViewLayout.Tag?.ToString();
                if (stat == "show")
                {
                    MainVideoViewLayout.Tag = "hide";

                    TopControlLayout.Visibility = ViewStates.Gone;
                    BottomControlLayout.Visibility = ViewStates.Gone;
                }
                else if (stat == "hide")
                {
                    MainVideoViewLayout.Tag = "show";

                    TopControlLayout.Visibility = ViewStates.Visible;
                    BottomControlLayout.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void PictureInToPictureButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                BackPressed();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void StopVideoButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (StopVideoButton.Selected)
                {
                    StopVideoButton.Selected = false;
                    IconStopVideo.SetImageResource(Resource.Drawable.icon_video_camera);
                }
                else
                {
                    StopVideoButton.Selected = true;
                    IconStopVideo.SetImageResource(Resource.Drawable.icon_video_camera_mute);
                }

                AgoraEngine?.MuteLocalVideoStream(StopVideoButton.Selected);

                IsVideoEnabled = !StopVideoButton.Selected;
                LocalVideoContainer.Visibility = IsVideoEnabled ? ViewStates.Visible : ViewStates.Gone;
                //LocalVideoView.Visibility = IsVideoEnabled ? ViewStates.Visible : ViewStates.Gone;

                LocalVideoOverlay.Visibility = IsVideoEnabled ? ViewStates.Gone : ViewStates.Visible;
                IconMuteLocalVideo.Visibility = IsVideoEnabled ? ViewStates.Gone : ViewStates.Visible;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void SwitchCamButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                AgoraEngine?.SwitchCamera();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

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

                var visibleMutedLayers = MuteAudioButton.Selected ? ViewStates.Visible : ViewStates.Gone;
                //LocalVideoOverlay.Visibility = visibleMutedLayers;
                IconMuteVoiceLocalVideo.Visibility = visibleMutedLayers;
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

        #endregion

        #region PictureInPicture

        public override void OnPictureInPictureModeChanged(bool isInPictureInPictureMode, Configuration newConfig)
        {
            try
            {
                if (isInPictureInPictureMode)
                {
                    TopControlLayout.Visibility = ViewStates.Gone;
                    EndCallButton.Visibility = ViewStates.Gone;
                    MuteAudioButton.Visibility = ViewStates.Gone;
                    StopVideoButton.Visibility = ViewStates.Gone;
                    SwitchButtonLayout.Visibility = ViewStates.Gone;
                    UserNameTextView.Visibility = ViewStates.Gone;
                    DurationTextView.Visibility = ViewStates.Gone;
                    //PictureInToPictureButton.Visibility = ViewStates.Gone;
                    ThumbnailVideo.Visibility = ViewStates.Gone;
                }
                else
                {
                    TopControlLayout.Visibility = ViewStates.Visible;
                    EndCallButton.Visibility = ViewStates.Visible;
                    MuteAudioButton.Visibility = ViewStates.Visible;
                    SwitchButtonLayout.Visibility = ViewStates.Visible;
                    UserNameTextView.Visibility = ViewStates.Visible;
                    //DurationTextView.Visibility = ViewStates.Visible;
                    StopVideoButton.Visibility = ViewStates.Visible;
                    //PictureInToPictureButton.Visibility = ViewStates.Visible;
                    ThumbnailVideo.Visibility = ViewStates.Visible;
                }

                base.OnPictureInPictureModeChanged(isInPictureInPictureMode, newConfig);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnUserLeaveHint()
        {
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    EnterPictureInPictureMode(PictureInPictureParams);
                }

                base.OnUserLeaveHint();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
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
                    if (CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted && CheckSelfPermission(Manifest.Permission.RecordAudio) == Permission.Granted && CheckSelfPermission(Manifest.Permission.ModifyAudioSettings) == Permission.Granted)
                    {
                        InitAgoraCall();
                    }
                    else
                    {
                        new PermissionsController(this).RequestPermission(103);
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

                if (requestCode == 103)
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
                CallType = Intent?.GetStringExtra("type") ?? ""; // Agora_video_call_recieve , Agora_video_calling_start

                if (!string.IsNullOrEmpty(Intent?.GetStringExtra("callUserObject")))
                    CallUserObject = JsonConvert.DeserializeObject<DataCallObject>(Intent?.GetStringExtra("callUserObject") ?? "");

                InitializeAgoraEngine();

                if (CallType == "Agora_video_call_recieve")
                {
                    if (!string.IsNullOrEmpty(CallUserObject?.ToId))
                        Load_userWhenCall();

                    Token = CallUserObject.AccessToken;

                    DurationTextView.Text = GetText(Resource.String.Lbl_Waiting_for_answer);

                    var (apiStatus, respond) = await RequestsAsync.Call.SendAnswerCallAsync(CallUserObject.Id, TypeCall.Video);
                    if (apiStatus == 200)
                    {
                        JoinChannel(Token, CallUserObject.RoomName);

                        //HomeActivity.AddCallToListAndSend("Answered", GetText(Resource.String.Lbl_Incoming), TypeCall.Video, CallUserObject);
                    }
                    //else Methods.DisplayReportResult(this, respond);
                }
                else if (CallType == "Agora_video_calling_start")
                {
                    DurationTextView.Text = GetText(Resource.String.Lbl_Calling);

                    //Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("outgoin_call.mp3", "Looping");

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
                AgoraEngine?.SetChannelProfile(Constants.ChannelProfileCommunication);
                AgoraEngine?.SetClientRole(Constants.ClientRoleBroadcaster);
                AgoraEngine?.EnableAudio();
                AgoraEngine?.EnableVideo();
                //AgoraEngine?.SetVideoEncoderConfiguration(new VideoEncoderConfiguration(VideoEncoderConfiguration.VD640x480, VideoEncoderConfiguration.FRAME_RATE.FrameRateFps15, VideoEncoderConfiguration.StandardBitrate, VideoEncoderConfiguration.ORIENTATION_MODE.OrientationModeFixedPortrait));

                SetupLocalVideo();
                //// Set the video profile
                //VideoEncoderConfiguration videoConfig = new VideoEncoderConfiguration();
                //// Set mirror mode
                //videoConfig.MirrorMode = VideoEncoderConfiguration.MIRROR_MODE_TYPE.MirrorModeAuto;
                //// Set framerate
                //videoConfig.MinFrameRate = VideoEncoderConfiguration.FRAME_RATE.FrameRateFps10.Value;
                //// Set bitrate
                //videoConfig.MinBitrate = VideoEncoderConfiguration.StandardBitrate;
                //// Set dimensions
                //videoConfig.Dimensions = VideoEncoderConfiguration.VD640x480;
                //// Set orientation mode
                //videoConfig.OrientationMode = VideoEncoderConfiguration.ORIENTATION_MODE.OrientationModeAdaptive;
                //// Set degradation preference
                //videoConfig.DegradationPrefer = VideoEncoderConfiguration.DEGRADATION_PREFERENCE.MaintainBalanced;
                //// Apply the configuration
                //AgoraEngine?.SetVideoEncoderConfiguration(videoConfig);

            }
            catch (Exception e)
            {
                //Colud not create RtcEngine
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void Load_userWhenCall()
        {
            try
            {
                UserNameTextView.Text = CallUserObject.Fullname;

                //profile_picture
                Glide.With(this).Load(CallUserObject.Avater).Apply(new RequestOptions().Placeholder(Resource.Drawable.ImagePlacholder)).Into(UserImageView);
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
            var (apiStatus, respond) = await RequestsAsync.Call.CreateNewCallAgoraAsync(UserDetails.UserId.ToString(), CallUserObject.ToId, TypeCall.Video);
            if (apiStatus == 200)
            {
                if (respond is CreateNewCallAgoraObject result)
                {
                    CallUserObject.Id = result.Id;
                    Token = CallUserObject.AccessToken = result.Token;
                    CallUserObject.RoomName = result.RoomName;

                    //HomeActivity.AddCallToListAndSend("Answered", GetText(Resource.String.Lbl_Outgoing), TypeCall.Video, CallUserObject);

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
                var (apiStatus, respond) = await RequestsAsync.Call.CheckForAnswerAsync(CallUserObject.Id, TypeCall.Video);
                if (apiStatus == 200)
                {
                    if (respond is AnswerCallObject agoraObject)
                    {
                        //RunOnUiThread(Methods.AudioRecorderAndPlayer.StopAudioFromAsset);
                        CallingService.GetService().RingtonePlayer?.StopRingtone();

                        JoinChannel(Token, CallUserObject.RoomName);

                        if (TimerRequestWaiter != null)
                        {
                            TimerRequestWaiter.Enabled = false;
                            TimerRequestWaiter.Stop();
                            TimerRequestWaiter.Close();
                        }

                        //HomeActivity.AddCallToListAndSend("Answered", GetText(Resource.String.Lbl_Outgoing), TypeCall.Video, CallUserObject);

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

                                //HomeActivity.AddCallToListAndSend("Cancel", GetText(Resource.String.Lbl_Outgoing), TypeCall.Video, CallUserObject);

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

                                //HomeActivity.AddCallToListAndSend("Cancel", GetText(Resource.String.Lbl_Missing), TypeCall.Video, CallUserObject);

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

                                //HomeActivity.AddCallToListAndSend("NoAnswer", GetText(Resource.String.Lbl_Missing), TypeCall.Video, CallUserObject);

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
                            //    //HomeActivity.AddCallToListAndSend("busy_another_call", GetText(Resource.String.Lbl_Missing), TypeCall.Video, CallUserObject);

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

                                //HomeActivity.AddCallToListAndSend("busy_another_call", GetText(Resource.String.Lbl_Missing), TypeCall.Video, CallUserObject);

                                FinishCallingService();
                            }
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

        public void OnUserOffline()
        {
            RunOnUiThread(async () =>
            {
                try
                {
                    //Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
                    //Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("Error.mp3");
                    DurationTextView.Text = GetText(Resource.String.Lbl_Lost_his_connection);
                    await Task.Delay(500);
                    FinishCallingService();
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                    FinishCallingService();
                }
            });
        }

        public void OnUserJoined(int uid, int elapsed)
        {
            RunOnUiThread(() =>
            {
                try
                {
                    DurationTextView.Text = "";
                    SetupRemoteVideo(uid);

                    TimerSound = new Timer();
                    TimerSound.Interval = 1000;
                    TimerSound.Elapsed += TimerSoundOnElapsed;
                    TimerSound.Start();

                    //Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
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

        private void SetupLocalVideo()
        {
            try
            {
                LocalVideoView = new SurfaceView(BaseContext);
                LocalVideoView.SetZOrderMediaOverlay(true);
                LocalVideoContainer.AddView(LocalVideoView);
                AgoraEngine?.SetupLocalVideo(new VideoCanvas(LocalVideoView, VideoCanvas.RenderModeHidden, 0));

                //if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList.AgoraCustomerCertificate))
                //{
                //    AgoraEngine.SetEncryptionMode(EncryptionType.xts128);
                //    AgoraEngine.SetEncryptionSecret(ListUtils.SettingsSiteList.AgoraCustomerCertificate);
                //}

                AgoraEngine?.StartPreview();
                LocalVideoView.Visibility = ViewStates.Visible;
                LocalVideoOverlay.Visibility = ViewStates.Gone;
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

        private void SetupRemoteVideo(int uid)
        {
            try
            {
                if (RemoteVideoViewContainer.ChildCount >= 1)
                    return;

                RemoteSurfaceView = new SurfaceView(BaseContext);
                RemoteVideoViewContainer.AddView(RemoteSurfaceView);
                AgoraEngine.SetupRemoteVideo(new VideoCanvas(RemoteSurfaceView, VideoCanvas.RenderModeFit, uid));
                // Display RemoteSurfaceView.
                RemoteSurfaceView.Visibility = ViewStates.Visible;

                RemoteSurfaceView.Tag = uid; // for mark purpose
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnFirstLocalVideoFrame(Constants.VideoSourceType source, int width, int height, int elapsed)
        {
            try
            {
                var ratio = height / width;
                var ratioHeight = ratio * MaxLocalVideoDimension;
                var ratioWidth = MaxLocalVideoDimension / ratio;
                var containerHeight = height > width ? MaxLocalVideoDimension : ratioHeight;
                var containerWidth = height > width ? ratioWidth : MaxLocalVideoDimension;
                RunOnUiThread(() =>
                {
                    try
                    {
                        var parameters = ThumbnailVideo.LayoutParameters;
                        parameters.Height = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, containerHeight, Resources.DisplayMetrics);
                        parameters.Width = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, containerWidth, Resources.DisplayMetrics);
                        ThumbnailVideo.LayoutParameters = parameters;
                        ThumbnailVideo.Visibility = IsVideoEnabled ? ViewStates.Visible : ViewStates.Gone;
                    }
                    catch (Exception e)
                    {
                        Methods.DisplayReportResultTrack(e);
                    }
                });
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnJoinChannelSuccess(string channel, int uid, int elapsed)
        {

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

        public void OnRemoteAudioStateChanged(int uid, int state, int reason, int elapsed)
        {
            RunOnUiThread(() =>
            {
                try
                {
                    switch (reason)
                    {
                        //The SDK reports this reason when the audio state changes
                        case Constants.RemoteAudioReasonInternal:
                            //MakeText(this, "The SDK reports this reason when the audio state changes", ToastLength.Short)?.Show();
                            break;
                        //Network congestion
                        case Constants.RemoteAudioReasonNetworkCongestion:
                            //Toast.MakeText(this, "Network congestion", ToastLength.Short)?.Show();
                            break;
                        //Network recovery.
                        case Constants.RemoteAudioReasonNetworkRecovery:
                            //Toast.MakeText(this, "Network recovery", ToastLength.Short)?.Show();
                            break;
                        //The local user stops receiving the remote audio stream or disables the audio module
                        case Constants.RemoteAudioReasonLocalMuted:
                            //Toast.MakeText(this, "The local user stops receiving the remote audio stream or disables the audio module", ToastLength.Short)?.Show();
                            break;
                        //The local user resumes receiving the remote audio stream or enables the audio module
                        case Constants.RemoteAudioReasonLocalUnmuted:
                            //Toast.MakeText(this, "The local user resumes receiving the remote audio stream or enables the audio module", ToastLength.Short)?.Show();
                            break;
                        //The remote user stops sending the audio stream or disables the audio module.
                        case Constants.RemoteAudioReasonRemoteMuted:
                            IconMuteVoiceRemoteVideo.Visibility = ViewStates.Visible;
                            IsMuted = true;
                            DurationTextView.Text = GetText(Resource.String.Lbl_Muted_his_video);
                            break;
                        //The remote user resumes sending the audio stream or enables the audio module.
                        case Constants.RemoteAudioReasonRemoteUnmuted:
                            IconMuteVoiceRemoteVideo.Visibility = ViewStates.Gone;
                            IsMuted = false;
                            DurationTextView.Text = TimeCall;
                            break;
                        //The remote user leaves the channel.
                        case Constants.RemoteAudioReasonRemoteOffline:
                            OnUserOffline();
                            break;
                    }
                }
                catch (Exception exception)
                {
                    Methods.DisplayReportResultTrack(exception);
                }
            });
        }

        public void OnRemoteVideoStateChanged(int uid, int state, int reason, int elapsed)
        {
            RunOnUiThread(() =>
            {
                try
                {
                    switch (reason)
                    {
                        //Internal reasons
                        case Constants.RemoteVideoStateReasonInternal:
                            //Toast.MakeText(this, "Internal reasons", ToastLength.Short)?.Show();
                            break;
                        //Network congestion
                        case Constants.RemoteVideoStateReasonNetworkCongestion:
                            //Toast.MakeText(this, "Network congestion", ToastLength.Short)?.Show();
                            break;
                        //Network recovery.
                        case Constants.RemoteVideoStateReasonNetworkRecovery:
                            //Toast.MakeText(this, "Network recovery", ToastLength.Short)?.Show();
                            break;
                        //The local user stops receiving the remote video stream or disables the video module
                        case Constants.RemoteVideoStateReasonLocalMuted:
                            // Toast.MakeText(this, "The local user stops receiving the remote video stream or disables the video module", ToastLength.Short)?.Show();
                            break;
                        //The local user resumes receiving the remote video stream or enables the video module
                        case Constants.RemoteVideoStateReasonLocalUnmuted:
                            //Toast.MakeText(this, "The local user resumes receiving the remote video stream or enables the video module", ToastLength.Short)?.Show();
                            break;
                        //The remote user stops sending the video stream or disables the video module.
                        case Constants.RemoteVideoStateReasonRemoteMuted:
                            IconMuteRemoteVideo.Visibility = ViewStates.Visible;
                            RemoteSurfaceView.Visibility = ViewStates.Gone;
                            break;
                        //The remote user resumes sending the video stream or enables the video module.
                        case Constants.RemoteVideoStateReasonRemoteUnmuted:
                            IconMuteRemoteVideo.Visibility = ViewStates.Gone;
                            RemoteSurfaceView.Visibility = ViewStates.Visible;
                            break;
                        //The remote user leaves the channel.
                        case Constants.RemoteVideoStateReasonRemoteOffline:
                            OnUserOffline();
                            break;
                        //The remote media stream falls back to the audio-only stream due to poor network conditions.
                        case Constants.RemoteVideoStateReasonAudioFallback:
                            //Toast.MakeText(this, "The remote media stream falls back to the audio-only stream due to poor network conditions", ToastLength.Short)?.Show();
                            break;
                        //The remote media stream switches back to the video stream after the network conditions improve.
                        case Constants.RemoteVideoStateReasonAudioFallbackRecovery:
                            //Toast.MakeText(this , "The remote media stream switches back to the video stream after the network conditions improve", ToastLength.Short)?.Show();
                            break;
                    }
                }
                catch (Exception exception)
                {
                    Methods.DisplayReportResultTrack(exception);
                }
            });
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

        #endregion

        #region Back Pressed

        private bool IsPipModeEnabled = true;
        public void BackPressed()
        {
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N && PackageManager.HasSystemFeature(PackageManager.FeaturePictureInPicture) && IsPipModeEnabled)
                {
                    EnterPipMode();
                }
                else
                {
                    FinishCallingService();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                FinishCallingService();
            }
        }

        private void EnterPipMode()
        {
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N && PackageManager.HasSystemFeature(PackageManager.FeaturePictureInPicture))
                {
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                    {
                        Rational rational = new Rational(9, 16);
                        PictureInPictureParams.Builder builder = new PictureInPictureParams.Builder();
                        builder.SetAspectRatio(rational);
                        EnterPictureInPictureMode(builder.Build());
                    }
                    else
                    {
                        var param = new PictureInPictureParams.Builder().Build();
                        EnterPictureInPictureMode(param);
                    }

                    new Handler(Looper.MainLooper).PostDelayed(CheckPipPermission, 30);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void CheckPipPermission()
        {
            try
            {
                IsPipModeEnabled = IsInPictureInPictureMode;
                if (!IsInPictureInPictureMode)
                {
                    BackPressed();
                }
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
                    AgoraEngine.RemoveHandler(AgoraHandler);
                    AgoraEngine.StopPreview();
                    AgoraEngine.SetupLocalVideo(null);
                    AgoraEngine.LeaveChannel();
                    AgoraEngine.Dispose();
                }

                // Destroy the engine in a sub-thread to avoid congestion
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