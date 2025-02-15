using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Com.Twilio.Video;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Call;
using System;
using System.Collections.Generic;
using System.Linq;
using Tvi.Webrtc;
using AudioTrack = Com.Twilio.Video.AudioTrack;
using Object = Java.Lang.Object;
using VideoTrack = Com.Twilio.Video.VideoTrack;

namespace QuickDate.Activities.Call.Twilio
{
    public class TwilioVideoHelper : Object, Room.IListener, RemoteParticipant.IListener, AudioManager.IOnAudioFocusChangeListener
    {
        public static TwilioVideoHelper Instance { get; private set; }

        static volatile bool CallProgress;

        private static bool CallInProgress
        {
            get => CallProgress;
            set => CallProgress = value;
        }

        public LocalVideoTrack CurrentVideoTrack { get; private set; }
        protected VideoTrack RemoteVideoTrack { get; private set; }
        protected LocalAudioTrack CurrentAudioTrack { get; private set; }
        protected AudioTrack RemoteAudioTrack { get; private set; }
        protected CameraCapturer VideoCapturer { get; private set; }
        protected RemoteParticipant Participant { get; private set; }
        protected Room CurrentRoom { get; private set; }
        //protected Stopwatch Timer { get; private set; } = new Stopwatch();

        public bool ClientIsReady => AccessToken != null!;

        private string AccessToken;

        private IListener Listener;

        private AudioManager AudioManager;
        private Mode PreviousAudioMode;
        private bool PreviousSpeakerphoneOn;
        private static TypeCall TypeCall;

        protected TwilioVideoHelper(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        private TwilioVideoHelper()
        {
        }

        public enum StopReason
        {
            Error,
            VideoTrackRemoved,
            ParticipantDisconnected,
            RoomDisconnected
        }

        public interface IListener
        {
            void SetLocalVideoTrack(LocalVideoTrack track);
            void SetRemoteVideoTrack(VideoTrack track);
            void RemoveLocalVideoTrack(LocalVideoTrack track);
            void RemoveRemoteVideoTrack(VideoTrack track);
            void OnRoomConnected(string roomId);
            void OnRoomDisconnected(StopReason reason);
            void OnParticipantConnected(string participantId);
            void OnParticipantDisconnected(string participantId);
            //void SetCallTime(int seconds);
            void OnLastQualityLevel(RemoteParticipant remoteParticipant, NetworkQualityLevel networkQualityLevel);
            void OnMuteAudioTrack(RemoteAudioTrackPublication trackPublication, bool muted);
        }

        public static TwilioVideoHelper GetOrCreate(Context context, TypeCall typeCall)
        {
            try
            {
                TypeCall = typeCall;

                if (Instance == null)
                    Instance = new TwilioVideoHelper();

                if (Instance.CurrentVideoTrack == null || Instance.CurrentAudioTrack == null)
                    Instance.CreateLocalMedia(context, typeCall);

                return Instance;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null!;
            }
        }

        private string FrontCameraId = null!;
        private string BackCameraId = null!;
        private readonly Camera1Enumerator Camera1Enumerator = new Camera1Enumerator();
        private string GetFrontCameraId()
        {
            if (FrontCameraId == null)
            {
                foreach (var deviceName in Camera1Enumerator.GetDeviceNames())
                {
                    if (Camera1Enumerator.IsFrontFacing(deviceName))
                    {
                        FrontCameraId = deviceName;
                    }
                }
            }

            return FrontCameraId;
        }

        private string GetBackCameraId()
        {
            if (BackCameraId == null)
            {
                foreach (var deviceName in Camera1Enumerator.GetDeviceNames())
                {
                    if (Camera1Enumerator.IsBackFacing(deviceName))
                    {
                        BackCameraId = deviceName;
                    }
                }
            }

            return BackCameraId;
        }

        private void CreateLocalMedia(Context context, TypeCall typeCall)
        {
            try
            {
                AudioManager = (AudioManager)context.GetSystemService(Context.AudioService);
                AudioManager.SpeakerphoneOn = TypeCall != TypeCall.Audio;

                VideoCapturer = new CameraCapturer(context, GetFrontCameraId());

                VideoFormat videoConstraints = new VideoFormat(VideoDimensions.Hd720pVideoDimensions, 30);

                if (typeCall == TypeCall.Audio)
                {
                    CurrentAudioTrack = LocalAudioTrack.Create(context, true, "mic");
                }
                else
                {
                    CurrentVideoTrack = LocalVideoTrack.Create(context, true, VideoCapturer, videoConstraints, "camera");
                    CurrentAudioTrack = LocalAudioTrack.Create(context, true, "mic");
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void Bind(IListener listener)
        {
            try
            {
                Listener = listener;
                if (CurrentRoom != null)
                    Listener.OnRoomConnected(CurrentRoom.Sid);
                if (Participant != null)
                    Listener.OnParticipantConnected(Participant.Identity);
                if (CurrentVideoTrack != null)
                    Listener.SetLocalVideoTrack(CurrentVideoTrack);
                if (RemoteVideoTrack != null)
                    Listener.SetRemoteVideoTrack(RemoteVideoTrack);
                //if (Timer.IsRunning)
                //    Listener.SetCallTime((int)Timer.Elapsed.TotalSeconds);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void DropRenderings(VideoTrack track)
        {
            try
            {
                if (track?.Sinks?.Any() == true)
                    foreach (var r in track.Sinks.ToArray())
                        track.RemoveSink(r);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void Unbind(IListener listener)
        {
            try
            {
                RemoveTracksRenderings();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void UpdateToken(string capabilityToken)
        {
            try
            {
                if (AccessToken == capabilityToken)
                    return;
                AccessToken = capabilityToken;
                if (CurrentRoom == null)
                    return;
                CurrentRoom.Disconnect();
                CurrentRoom = null!;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void FlipCamera()
        {
            try
            {
                if (VideoCapturer != null)
                {
                    var cameraId = VideoCapturer.CameraId.Equals(GetFrontCameraId()) ? GetBackCameraId() : GetFrontCameraId();
                    VideoCapturer.SwitchCamera(cameraId);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void Mute(bool muted)
        {
            try
            {
                CurrentAudioTrack?.Enable(!muted);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void Speaker(bool speaker)
        {
            try
            {
                AudioManager ??= (AudioManager)Application.Context.GetSystemService(Context.AudioService);

                AudioManager.SpeakerphoneOn = speaker;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void JoinRoom(Context context, TypeCall typeCall, string roomName)
        {
            try
            {
                if (CurrentRoom != null)
                    return;

                // Enable network quality information for local and remote participants
                NetworkQualityConfiguration configuration = new NetworkQualityConfiguration(NetworkQualityVerbosity.NetworkQualityVerbosityMinimal, NetworkQualityVerbosity.NetworkQualityVerbosityMinimal);

                IList<LocalVideoTrack> videoTracks = new List<LocalVideoTrack> { CurrentVideoTrack };
                IList<LocalAudioTrack> audioTracks = new List<LocalAudioTrack> { CurrentAudioTrack };
                var options = new ConnectOptions.Builder(AccessToken)
                    .AudioTracks(audioTracks)
                    .RoomName(roomName)
                    .EnableDominantSpeaker(true)

                    //============= New 
                    .EnableNetworkQuality(true)
                    .NetworkQualityConfiguration(configuration)
                    .PreferVideoCodecs(new List<VideoCodec> { new Vp8Codec(true) })
                     //=============  
                     ;

                if (typeCall == TypeCall.Video)
                {
                    options.VideoTracks(videoTracks);
                }

                CurrentRoom = Video.Connect(context, options.Build(), this);
                CallInProgress = true;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void RemoveTracksRenderings()
        {
            try
            {
                DropRenderings(RemoteVideoTrack);
                DropRenderings(CurrentVideoTrack);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void ReleaseRoom()
        {
            try
            {
                CurrentRoom?.Disconnect();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
            finally
            {
                CurrentRoom = null!;
            }
        }

        private void ReleaseMedia()
        {
            try
            {
                if (VideoCapturer != null)
                {
                    VideoCapturer.StopCapture();
                    VideoCapturer = null!;
                }

                if (CurrentVideoTrack != null)
                {
                    var videoTrack = CurrentVideoTrack;
                    CurrentVideoTrack = null!;
                    CurrentRoom?.LocalParticipant.UnpublishTrack(videoTrack);
                    DropRenderings(videoTrack);
                    videoTrack.Release();
                }

                if (CurrentAudioTrack != null)
                {
                    var audioTrack = CurrentAudioTrack;
                    CurrentAudioTrack = null!;
                    CurrentRoom?.LocalParticipant.UnpublishTrack(audioTrack);
                    audioTrack.Enable(false);
                    audioTrack.Release();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void SetAudioFocus(bool focused)
        {
            try
            {
                AudioManager ??= (AudioManager)Application.Context.GetSystemService(Context.AudioService);

                if (focused)
                {
                    PreviousAudioMode = AudioManager.Mode;
                    PreviousSpeakerphoneOn = AudioManager.SpeakerphoneOn;

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                    {

                        var playbackAttributes = new AudioAttributes.Builder()
                             ?.SetUsage(AudioUsageKind.VoiceCommunication)
                             ?.SetContentType(AudioContentType.Speech)
                             ?.Build();

                        var focusRequest = new AudioFocusRequestClass.Builder(AudioFocus.Gain)
                             .SetAudioAttributes(playbackAttributes)
                             .SetAcceptsDelayedFocusGain(true)
                             .SetOnAudioFocusChangeListener(this)
                             .Build();

                        AudioManager.RequestAudioFocus(focusRequest);
                    }
                    else
                    {
#pragma warning disable 618
                        AudioManager.RequestAudioFocus(this, Stream.VoiceCall, AudioFocus.GainTransient);
#pragma warning restore 618
                    }

                    //Start by setting MODE_IN_COMMUNICATION as default audio mode. It is
                    //required to be in this mode when playout and/or recording starts for
                    //best possible VoIP performance. Some devices have difficulties with speaker mode
                    //if this is not set. 
                    AudioManager.SpeakerphoneOn = TypeCall != TypeCall.Audio;

                    //AudioManager.SpeakerphoneOn = false;
                    AudioManager.Mode = Mode.InCommunication;
                }
                else
                {
                    AudioManager.Mode = PreviousAudioMode;
                    AudioManager.SpeakerphoneOn = PreviousSpeakerphoneOn;
#pragma warning disable 618
                    AudioManager.AbandonAudioFocus(null);
#pragma warning restore 618
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void OnFinishConversation(StopReason reason)
        {
            try
            {
                if (!CallInProgress)
                    return;

                CallInProgress = false;
                RemoveTracksRenderings();
                ReleaseRoom();
                Listener?.OnRoomDisconnected(reason);
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
                Listener = null!;

                ReleaseRoom();

                Participant = null!;
                RemoteVideoTrack = null!;
                RemoteAudioTrack = null!;

                ReleaseMedia();

                if (AudioManager != null)
                {
                    SetAudioFocus(false);
                    AudioManager = null!;
                }

                //Timer.Stop();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                FinishCall();
                base.Dispose(disposing);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //=========, LocalParticipant.IListener  

        #region LocalParticipant.IListener

        public void OnAudioTrackDisabled(RemoteParticipant p0, RemoteAudioTrackPublication p1)
        {
            try
            {
                Listener?.OnMuteAudioTrack(p1, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnAudioTrackEnabled(RemoteParticipant p0, RemoteAudioTrackPublication p1)
        {
            try
            {
                Listener?.OnMuteAudioTrack(p1, false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnAudioTrackPublished(RemoteParticipant p0, RemoteAudioTrackPublication p1)
        {
            try
            {
                RemoteAudioTrack = p1.RemoteAudioTrack;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnAudioTrackSubscribed(RemoteParticipant p0, RemoteAudioTrackPublication p1, RemoteAudioTrack p2)
        {
            try
            {
                RemoteAudioTrack = p1.RemoteAudioTrack;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnAudioTrackSubscriptionFailed(RemoteParticipant p0, RemoteAudioTrackPublication p1, TwilioException p2)
        {

        }

        public void OnAudioTrackUnpublished(RemoteParticipant p0, RemoteAudioTrackPublication p1)
        {
            try
            {
                if (RemoteAudioTrack?.Name == p1.TrackName)
                    RemoteAudioTrack = null!;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnAudioTrackUnsubscribed(RemoteParticipant p0, RemoteAudioTrackPublication p1, RemoteAudioTrack p2)
        {
            try
            {
                if (RemoteAudioTrack?.Name == p1.TrackName)
                    RemoteAudioTrack = null!;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnDataTrackPublished(RemoteParticipant p0, RemoteDataTrackPublication p1)
        {

        }

        public void OnDataTrackSubscribed(RemoteParticipant p0, RemoteDataTrackPublication p1, RemoteDataTrack p2)
        {

        }

        public void OnDataTrackSubscriptionFailed(RemoteParticipant p0, RemoteDataTrackPublication p1, TwilioException p2)
        {

        }

        public void OnDataTrackUnpublished(RemoteParticipant p0, RemoteDataTrackPublication p1)
        {

        }

        public void OnDataTrackUnsubscribed(RemoteParticipant p0, RemoteDataTrackPublication p1, RemoteDataTrack p2)
        {

        }

        public void OnNetworkQualityLevelChanged(RemoteParticipant remoteParticipant, NetworkQualityLevel networkQualityLevel)
        {
            try
            {
                Listener?.OnLastQualityLevel(remoteParticipant, networkQualityLevel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnVideoTrackDisabled(RemoteParticipant p0, RemoteVideoTrackPublication p1)
        {

        }

        public void OnVideoTrackEnabled(RemoteParticipant p0, RemoteVideoTrackPublication p1)
        {

        }

        public void OnVideoTrackPublished(RemoteParticipant p0, RemoteVideoTrackPublication p1)
        {
            try
            {
                RemoteVideoTrack = p1.RemoteVideoTrack;
                Listener?.SetRemoteVideoTrack(RemoteVideoTrack);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnVideoTrackSubscribed(RemoteParticipant p0, RemoteVideoTrackPublication p1, RemoteVideoTrack p2)
        {
            try
            {
                RemoteVideoTrack = p1.RemoteVideoTrack;
                Listener?.SetRemoteVideoTrack(RemoteVideoTrack);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnVideoTrackSubscriptionFailed(RemoteParticipant p0, RemoteVideoTrackPublication p1, TwilioException p2)
        {

        }

        public void OnVideoTrackUnpublished(RemoteParticipant p0, RemoteVideoTrackPublication p1)
        {
            try
            {
                if (RemoteVideoTrack?.Name != p1.TrackName)
                    return;

                Listener?.RemoveRemoteVideoTrack(RemoteVideoTrack);
                RemoteVideoTrack = null!;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnVideoTrackUnsubscribed(RemoteParticipant p0, RemoteVideoTrackPublication p1, RemoteVideoTrack p2)
        {
            try
            {
                if (RemoteVideoTrack?.Name != p1.TrackName)
                    return;

                Listener?.RemoveRemoteVideoTrack(RemoteVideoTrack);
                RemoteVideoTrack = null!;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        //============== Room.IListener 

        #region Room.IListener

        public void OnConnectFailure(Room p0, TwilioException p1)
        {
            try
            {
                OnFinishConversation(StopReason.Error);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnConnected(Room room)
        {
            try
            {
                CurrentRoom = room;
                Listener?.OnRoomConnected(room.Name);
                var participant = room.RemoteParticipants.FirstOrDefault(p => p.Identity != room.LocalParticipant.Identity);

                if (participant != null)
                    OnParticipantConnected(room, participant);

                if (AudioManager != null)
                    SetAudioFocus(true);

                //if (!Timer.IsRunning)
                //{
                //    Timer.Start();
                //}
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnDisconnected(Room room, TwilioException p1)
        {
            try
            {
                room.Dispose();
                //Timer.Stop();
                OnFinishConversation(p1 != null ? StopReason.Error : StopReason.RoomDisconnected);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnDominantSpeakerChanged(Room room, RemoteParticipant remoteParticipant)
        {

        }

        public void OnParticipantConnected(Room p0, RemoteParticipant p1)
        {
            try
            {
                Participant = p1;
                Participant.SetListener(this);
                //Timer.Restart();
                Listener?.OnParticipantConnected(p1.Identity);
                var videoTrack = Participant.RemoteVideoTracks.FirstOrDefault();
                if (videoTrack != null)
                    OnVideoTrackPublished(Participant, videoTrack);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnParticipantDisconnected(Room p0, RemoteParticipant p1)
        {
            try
            {
                if (Participant?.Identity != p1.Identity)
                    return;

                Listener?.OnParticipantDisconnected(p1.Identity);
                OnFinishConversation(StopReason.ParticipantDisconnected);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnReconnected(Room p0)
        {

        }

        public void OnReconnecting(Room p0, TwilioException p1)
        {

        }

        public void OnRecordingStarted(Room p0)
        {

        }

        public void OnRecordingStopped(Room p0)
        {

        }

        #endregion

        //============== AudioManager.IOnAudioFocusChangeListener

        public void OnAudioFocusChange(AudioFocus focusChange)
        {

        }
    }
}