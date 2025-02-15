using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using Anjo.Android.AgoraIO.AgoraDynamicKey.Media;
using IO.Agora.Rtc2;
using IO.Agora.Rtc2.Video;
using Java.Lang;
using QuickDate.Activities.Live.Rtc;
using QuickDate.Activities.Live.Stats;
using QuickDate.Activities.Live.Utils;
using QuickDate.Helpers.Utils;
using Boolean = Java.Lang.Boolean;
using Exception = System.Exception;

namespace QuickDate.Activities.Live.Page
{
    [Activity]
    public class RtcBaseActivity : AppCompatActivity
    {
        public RtcEngine MRtcEngine;
        private readonly EngineConfig MGlobalConfig = new EngineConfig();
        private readonly AgoraEventHandler MHandler = new AgoraEventHandler();
        private readonly StatsManager MStatsManager = new StatsManager();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                InitRtcEngine();
                InitConfig();

                Window?.SetSoftInputMode(SoftInput.AdjustResize);

                Methods.App.FullScreenApp(this, true);

                //JoinChannel(); 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void InitConfig()
        {
            try
            {
                ISharedPreferences pref = PrefManager.GetPreferences(ApplicationContext);
                MGlobalConfig.SetVideoDimenIndex(pref.GetInt(LiveConstants.PrefResolutionIdx, LiveConstants.DefaultProfileIdx));

                bool showStats = pref.GetBoolean(LiveConstants.PrefEnableStats, false);
                MGlobalConfig.SetIfShowVideoStats(showStats);
                MStatsManager.EnableStats(showStats);

                MGlobalConfig.SetMirrorLocalIndex(pref.GetInt(LiveConstants.PrefMirrorLocal, 0));
                MGlobalConfig.SetMirrorRemoteIndex(pref.GetInt(LiveConstants.PrefMirrorRemote, 0));
                MGlobalConfig.SetMirrorEncodeIndex(pref.GetInt(LiveConstants.PrefMirrorEncode, 0));
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void InitRtcEngine()
        {
            try
            {
                // Create an RtcEngineConfig instance and configure it
                RtcEngineConfig config = new RtcEngineConfig();
                config.MContext = this;
                config.MAppId = AppSettings.AppIdAgoraLive;
                config.MChannelProfile = Constants.ChannelProfileLiveBroadcasting;
                config.MEventHandler = MHandler;
                config.MAudioScenario = Constants.AudioScenarioDefault;

                // Create and initialize an RtcEngine instance
                MRtcEngine = RtcEngine.Create(config);

                // In the live broadcast scenario, set the channel profile to BROADCASTING (live broadcast scenario)
                // Sets the channel profile of the Agora RtcEngine.
                // The Agora RtcEngine differentiates channel profiles and applies different optimization algorithms accordingly. For example, it prioritizes smoothness and low latency for a video call, and prioritizes video quality for a video broadcast.
                MRtcEngine.SetChannelProfile(Constants.ChannelProfileLiveBroadcasting);

                MRtcEngine.SetLogFile(FileUtil.InitializeLogFile(this));

                // Enable the video module
                MRtcEngine.EnableVideo();
                // Enable local preview
                MRtcEngine.StartPreview();

                MRtcEngine.SetParameters("{"
                                         + "\"rtc.report_app_scenario\":"
                                         + "{"
                                         + "\"appScenario\":" + 100 + ","
                                         + "\"serviceType\":" + 11 + ","
                                         + "\"appVersion\":\"" + RtcEngine.SdkVersion + "\""
                                         + "}"
                                         + "}");

                VideoEncoderConfiguration configuration = new VideoEncoderConfiguration(LiveConstants.VideoDimensions[Config().GetVideoDimenIndex()], VideoEncoderConfiguration.FRAME_RATE.FrameRateFps15, VideoEncoderConfiguration.StandardBitrate, VideoEncoderConfiguration.ORIENTATION_MODE.OrientationModeFixedPortrait)
                {
                    MirrorMode = LiveConstants.VideoMirrorModes[Config().GetMirrorEncodeIndex()]
                };
                MRtcEngine.SetVideoEncoderConfiguration(configuration);
                MRtcEngine.SetDualStreamMode(Constants.SimulcastStreamMode.EnableSimulcastStream);
                MRtcEngine.StartMediaRenderingTracing();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected void JoinChannel()
        {
            try
            {
                string token = null!;
                if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.AgoraAppCertificate))
                {
                    string channelName = Config().GetChannelName();
                    uint uid = 0;
                    uint expirationTimeInSeconds = 3600;
                    uint timestamp = (uint)(Methods.Time.CurrentTimeMillis() / 1000 + expirationTimeInSeconds);

                    token = RtcTokenBuilder.BuildTokenWithUid(AppSettings.AppIdAgoraLive, ListUtils.SettingsSiteList?.AgoraAppCertificate, channelName, uid, RtcTokenBuilder.Role.RolePublisher, timestamp);
                }

                ChannelMediaOptions option = new ChannelMediaOptions((Integer)Constants.ClientRoleAudience);
                //option.ChannelProfile = IO.Agora.Rtc2.Constants.ChannelProfileLiveBroadcasting;

                // Publish the audio captured by the microphone
                option.PublishMicrophoneTrack = Boolean.True;
                // Publish the video captured by the camera
                option.PublishCameraTrack = Boolean.True;
                // Automatically subscribe to all audio streams
                option.AutoSubscribeAudio = Boolean.True;
                // Automatically subscribe to all video streams
                option.AutoSubscribeVideo = Boolean.True;

                MRtcEngine.JoinChannel(token, Config().GetChannelName(), 0, option);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //protected TextureView PrepareRtcVideo(int uid, bool local)
        //{
        //    try
        //    {
        //        // Render local/remote video on a SurfaceView

        //        var surface = DT.Xamarin.Agora.RtcEngine.CreateTextureView(ApplicationContext);
        //        if (local)
        //        {
        //            MRtcEngine?.SetupLocalVideo(new VideoCanvas(surface, VideoCanvas.RenderModeHidden, 0, Constants.VideoMirrorModes[Config().GetMirrorLocalIndex()]));
        //        }
        //        else
        //        {
        //            MRtcEngine?.SetupRemoteVideo(new VideoCanvas(surface, VideoCanvas.RenderModeHidden, uid, Constants.VideoMirrorModes[Config().GetMirrorRemoteIndex()]));
        //        }
        //        return surface;
        //    }
        //    catch (Exception e)
        //    {
        //        Methods.DisplayReportResultTrack(e);
        //        return null!;
        //    }
        //}

        protected SurfaceView PrepareRtcVideo(int uid, bool local)
        {
            try
            {
                SurfaceView surface = new SurfaceView(ApplicationContext);
                surface.SetZOrderMediaOverlay(true);
                switch (local)
                {
                    case true:
                        MRtcEngine?.SetupLocalVideo(new VideoCanvas(surface, VideoCanvas.RenderModeFit, 0));
                        break;
                    default:
                        MRtcEngine?.SetupRemoteVideo(new VideoCanvas(surface, VideoCanvas.RenderModeFit, uid));
                        break;
                }
                MRtcEngine?.SetDefaultAudioRoutetoSpeakerphone(true);

                return surface;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null!;
            }
        }

        protected void RemoveRtcVideo(int uid, bool local)
        {
            try
            {
                switch (local)
                {
                    case true:
                        MRtcEngine?.SetupLocalVideo(null);
                        break;
                    default:
                        MRtcEngine?.SetupRemoteVideo(new VideoCanvas(null, VideoCanvas.RenderModeHidden, uid));
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void StopRtc()
        {
            try
            {
                MRtcEngine?.StopPreview();
                MRtcEngine?.LeaveChannel();
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
                StopRtc();
                base.OnDestroy();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void RegisterEventHandler(IEventHandler handler)
        {
            MHandler.AddHandler(handler);
        }

        public void RemoveEventHandler(IEventHandler handler)
        {
            MHandler.RemoveHandler(handler);
        }

        protected EngineConfig Config()
        {
            try
            {
                return MGlobalConfig;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null!;
            }
        }

        protected StatsManager StatsManager() { return MStatsManager; }


    }
}