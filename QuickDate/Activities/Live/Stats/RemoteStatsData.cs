using Java.Util;
using System;

namespace QuickDate.Activities.Live.Stats
{
    public class RemoteStatsData : StatsData
    {
        private static readonly string Format = "Remote(%d)\n\n" +
                                                "%dx%d %dfps\n" +
                                                "Quality tx/rx: %s/%s\n" +
                                                "Video delay: %d ms\n" +
                                                "Audio net delay/jitter: %dms/%dms\n" +
                                                "Audio loss/quality: %d%%/%s";

        private int VideoDelay;
        private int AudioNetDelay;
        private int AudioNetJitter;
        private int AudioLoss;
        private string AudioQuality;

        public override string ToString()
        {
            return string.Format(Locale.Default.ToString(), Format,
                GetUid(),
                GetWidth(), GetHeight(), GetFramerate(),
                GetSendQuality(), GetRecvQuality(),
                GetVideoDelay(),
                GetAudioNetDelay(), GetAudioNetJitter(),
                GetAudioLoss(), GetAudioQuality());
        }

        public static String GetFormat()
        {
            return Format;
        }

        public int GetVideoDelay()
        {
            return VideoDelay;
        }

        public void SetVideoDelay(int videoDelay)
        {
            VideoDelay = videoDelay;
        }

        public int GetAudioNetDelay()
        {
            return AudioNetDelay;
        }

        public void SetAudioNetDelay(int audioNetDelay)
        {
            AudioNetDelay = audioNetDelay;
        }

        public int GetAudioNetJitter()
        {
            return AudioNetJitter;
        }

        public void SetAudioNetJitter(int audioNetJitter)
        {
            AudioNetJitter = audioNetJitter;
        }

        public int GetAudioLoss()
        {
            return AudioLoss;
        }

        public void SetAudioLoss(int audioLoss)
        {
            AudioLoss = audioLoss;
        }

        public string GetAudioQuality()
        {
            return AudioQuality;
        }

        public void SetAudioQuality(String audioQuality)
        {
            AudioQuality = audioQuality;
        }
    }

}