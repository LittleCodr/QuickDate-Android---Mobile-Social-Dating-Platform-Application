using QuickDate.Activities.Live.Page;

namespace QuickDate.Activities.Live.Rtc
{
    public class EngineConfig
    {
        // private static final int DEFAULT_UID = 0;
        // private int mUid = DEFAULT_UID;

        private string MChannelName;
        private bool MShowVideoStats;
        private int MDimenIndex = LiveConstants.DefaultProfileIdx;
        private int MMirrorLocalIndex;
        private int MMirrorRemoteIndex;
        private int MMirrorEncodeIndex;


        public int GetVideoDimenIndex()
        {
            return MDimenIndex;
        }

        public void SetVideoDimenIndex(int index)
        {
            MDimenIndex = index;
        }

        public string GetChannelName()
        {
            return MChannelName;
        }

        public void SetChannelName(string mChannel)
        {
            MChannelName = mChannel;
        }

        public bool IfShowVideoStats()
        {
            return MShowVideoStats;
        }

        public void SetIfShowVideoStats(bool show)
        {
            MShowVideoStats = show;
        }

        public int GetMirrorLocalIndex()
        {
            return MMirrorLocalIndex;
        }

        public void SetMirrorLocalIndex(int index)
        {
            MMirrorLocalIndex = index;
        }

        public int GetMirrorRemoteIndex()
        {
            return MMirrorRemoteIndex;
        }

        public void SetMirrorRemoteIndex(int index)
        {
            MMirrorRemoteIndex = index;
        }

        public int GetMirrorEncodeIndex()
        {
            return MMirrorEncodeIndex;
        }

        public void SetMirrorEncodeIndex(int index)
        {
            MMirrorEncodeIndex = index;
        }
    }
}