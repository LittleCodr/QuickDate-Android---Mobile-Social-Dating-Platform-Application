using IO.Agora.Rtc2;
using QuickDateClient.Classes.Call;

namespace QuickDate.Activities.Call.Agora
{
    public class AgoraRtcCallHandler : IRtcEngineEventHandler
    {
        private readonly AgoraAudioCallActivity ContextAgoraAudio;
        private readonly AgoraVideoCallActivity ContextAgoraVideo;
        private readonly TypeCall TypeCall;

        public AgoraRtcCallHandler(AgoraVideoCallActivity activity)
        {
            TypeCall = TypeCall.Video;
            ContextAgoraVideo = activity;
        }

        public AgoraRtcCallHandler(AgoraAudioCallActivity activity)
        {
            TypeCall = TypeCall.Audio;
            ContextAgoraAudio = activity;
        }

        public override void OnConnectionLost()
        {
            base.OnConnectionLost();
            switch (TypeCall)
            {
                case TypeCall.Video:
                    ContextAgoraVideo.OnConnectionLost();
                    break;
                case TypeCall.Audio:
                    ContextAgoraAudio.OnConnectionLost();
                    break;
            }
        }

        public override void OnUserOffline(int uid, int reason)
        {
            base.OnUserOffline(uid, reason);
            switch (TypeCall)
            {
                case TypeCall.Video:
                    ContextAgoraVideo.OnUserOffline();
                    break;
                case TypeCall.Audio:
                    ContextAgoraAudio.OnUserOffline(uid, reason);
                    break;
            }
        }

        public override void OnNetworkQuality(int uid, int txQuality, int rxQuality)
        {
            base.OnNetworkQuality(uid, txQuality, rxQuality);
            switch (TypeCall)
            {
                case TypeCall.Video:
                    //ContextAgoraVideo.OnNetworkQuality(uid, txQuality, rxQuality);
                    break;
                case TypeCall.Audio:
                    ContextAgoraAudio.OnNetworkQuality(uid, txQuality, rxQuality);
                    break;
            }
        }

        public override void OnUserJoined(int uid, int elapsed)
        {
            base.OnUserJoined(uid, elapsed);
            switch (TypeCall)
            {
                case TypeCall.Video:
                    ContextAgoraVideo.OnUserJoined(uid, elapsed);
                    break;
                case TypeCall.Audio:
                    ContextAgoraAudio.OnUserJoined(uid, elapsed);
                    break;
            }
        }

        public override void OnJoinChannelSuccess(string channel, int uid, int elapsed)
        {
            base.OnJoinChannelSuccess(channel, uid, elapsed);

            switch (TypeCall)
            {
                case TypeCall.Video:
                    ContextAgoraVideo.OnJoinChannelSuccess(channel, uid, elapsed);
                    break;
                case TypeCall.Audio:
                    ContextAgoraAudio.OnJoinChannelSuccess(channel, uid, elapsed);
                    break;
            }
        }

        public override void OnUserMuteAudio(int uid, bool muted)
        {
            base.OnUserMuteAudio(uid, muted);
            switch (TypeCall)
            {
                case TypeCall.Video:
                    //ContextAgoraVideo.OnUserMuteAudio(uid, muted);
                    break;
                case TypeCall.Audio:
                    ContextAgoraAudio.OnUserMuteAudio(uid, muted);
                    break;
            }
        }

        public override void OnLastmileQuality(int quality)
        {
            base.OnLastmileQuality(quality);
            switch (TypeCall)
            {
                case TypeCall.Video:
                    //ContextAgoraVideo.OnLastmileQuality(quality);
                    break;
                case TypeCall.Audio:
                    ContextAgoraAudio.OnLastmileQuality(quality);
                    break;
            }
        }

        public override void OnError(int err)
        {
            base.OnError(err);
            switch (TypeCall)
            {
                case TypeCall.Video:
                    ContextAgoraVideo.OnError(err);
                    break;
                case TypeCall.Audio:
                    ContextAgoraAudio.OnError(err);
                    break;
            }
        }

        public override void OnRemoteAudioStateChanged(int uid, int state, int reason, int elapsed)
        {
            base.OnRemoteAudioStateChanged(uid, state, reason, elapsed);
            switch (TypeCall)
            {
                case TypeCall.Video:
                    ContextAgoraVideo.OnRemoteAudioStateChanged(uid, state, reason, elapsed);
                    break;
                case TypeCall.Audio:
                    //ContextAgoraAudio.OnRemoteAudioStateChanged(uid, state, reason, elapsed);
                    break;
            }
        }

        public override void OnRemoteVideoStateChanged(int uid, int state, int reason, int elapsed)
        {
            base.OnRemoteVideoStateChanged(uid, state, reason, elapsed);
            switch (TypeCall)
            {
                case TypeCall.Video:
                    ContextAgoraVideo.OnRemoteVideoStateChanged(uid, state, reason, elapsed);
                    break;
                case TypeCall.Audio:
                    //ContextAgoraAudio.OnRemoteVideoStateChanged(uid, state, reason, elapsed);
                    break;
            }
        }

        public override void OnFirstLocalVideoFrame(Constants.VideoSourceType source, int width, int height, int elapsed)
        {
            base.OnFirstLocalVideoFrame(source, width, height, elapsed);
            switch (TypeCall)
            {
                case TypeCall.Video:
                    ContextAgoraVideo.OnFirstLocalVideoFrame(source, width, height, elapsed);
                    break;
                case TypeCall.Audio:
                    //ContextAgoraAudio.OnFirstLocalVideoFrame(source, width, height, elapsed);
                    break;
            }
        }
    }
}