using IO.Agora.Rtc2;
using System.Collections.Generic;

namespace QuickDate.Activities.Live.Rtc
{
    public class AgoraEventHandler : IRtcEngineEventHandler
    {
        private readonly List<IEventHandler> MHandler = new List<IEventHandler>();

        public void AddHandler(IEventHandler handler)
        {
            MHandler.Add(handler);
        }

        public void RemoveHandler(IEventHandler handler)
        {
            MHandler.Remove(handler);
        }

        public override void OnJoinChannelSuccess(string channel, int uid, int elapsed)
        {
            foreach (var handler in MHandler)
            {
                handler.OnJoinChannelSuccess(channel, uid, elapsed);
            }
        }

        public override void OnLeaveChannel(RtcStats stats)
        {
            foreach (var handler in MHandler)
            {
                handler.OnLeaveChannel(stats);
            }
        }

        public override void OnFirstRemoteVideoFrame(int uid, int width, int height, int elapsed)
        {
            foreach (var handler in MHandler)
            {
                handler.OnFirstRemoteVideoFrame(uid, width, height, elapsed);
            }
        }

        public override void OnUserJoined(int uid, int elapsed)
        {
            foreach (var handler in MHandler)
            {
                handler.OnUserJoined(uid, elapsed);
            }
        }

        public override void OnUserOffline(int uid, int reason)
        {
            foreach (var handler in MHandler)
            {
                handler.OnUserOffline(uid, reason);
            }
        }

        public override void OnLocalVideoStats(Constants.VideoSourceType source, LocalVideoStats stats)
        {
            foreach (var handler in MHandler)
            {
                handler.OnLocalVideoStats(source, stats);
            }
        }

        public override void OnRtcStats(RtcStats stats)
        {
            foreach (var handler in MHandler)
            {
                handler.OnRtcStats(stats);
            }
        }

        public override void OnNetworkQuality(int uid, int txQuality, int rxQuality)
        {
            foreach (var handler in MHandler)
            {
                handler.OnNetworkQuality(uid, txQuality, rxQuality);
            }
        }

        public override void OnRemoteVideoStats(RemoteVideoStats stats)
        {
            foreach (var handler in MHandler)
            {
                handler.OnRemoteVideoStats(stats);
            }
        }

        public override void OnRemoteAudioStats(RemoteAudioStats stats)
        {
            foreach (var handler in MHandler)
            {
                handler.OnRemoteAudioStats(stats);
            }
        }

        public override void OnLastmileQuality(int quality)
        {
            foreach (var handler in MHandler)
            {
                handler.OnLastmileQuality(quality);
            }
        }

        public override void OnLastmileProbeResult(LastmileProbeResult result)
        {
            foreach (var handler in MHandler)
            {
                handler.OnLastmileProbeResult(result);
            }
        }

        public override void OnFirstLocalVideoFrame(Constants.VideoSourceType source, int width, int height, int elapsed)
        {
            base.OnFirstLocalVideoFrame(source, width, height, elapsed);
            foreach (var handler in MHandler)
            {
                handler.OnFirstLocalVideoFrame(source, width, height, elapsed);
            }
        }
    }
}