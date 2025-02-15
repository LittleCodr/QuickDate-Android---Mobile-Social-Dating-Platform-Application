using IO.Agora.Rtc2;

namespace QuickDate.Activities.Live.Rtc
{
    public interface IEventHandler
    {
        void OnFirstLocalVideoFrame(Constants.VideoSourceType source, int width, int height, int elapsed);
        void OnFirstRemoteVideoFrame(int uid, int width, int height, int elapsed);

        void OnLeaveChannel(IRtcEngineEventHandler.RtcStats stats);

        void OnJoinChannelSuccess(string channel, int uid, int elapsed);

        void OnUserOffline(int uid, int reason);

        void OnUserJoined(int uid, int elapsed);

        void OnLastmileQuality(int quality);

        void OnLastmileProbeResult(IRtcEngineEventHandler.LastmileProbeResult result);

        void OnLocalVideoStats(Constants.VideoSourceType source, IRtcEngineEventHandler.LocalVideoStats stats);

        void OnRtcStats(IRtcEngineEventHandler.RtcStats stats);

        void OnNetworkQuality(int uid, int txQuality, int rxQuality);

        void OnRemoteVideoStats(IRtcEngineEventHandler.RemoteVideoStats stats);

        void OnRemoteAudioStats(IRtcEngineEventHandler.RemoteAudioStats stats);

    }
}