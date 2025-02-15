using Java.Util;

namespace QuickDate.Activities.Live.Stats
{
    public class LocalStatsData : StatsData
    {
        private static readonly string Format = "Local(%d)\n\n" +
                                                "%dx%d %dfps\n" +
                                                "LastMile delay: %d ms\n" +
                                                "Video tx/rx (kbps): %d/%d\n" +
                                                "Audio tx/rx (kbps): %d/%d\n" +
                                                "CPU: app/total %.1f%%/%.1f%%\n" +
                                                "Quality tx/rx: %s/%s\n" +
                                                "Loss tx/rx: %d%%/%d%%";

        private int LastMileDelay;
        private int VideoSend;
        private int VideoRecv;
        private int AudioSend;
        private int AudioRecv;
        private double CpuApp;
        private double CpuTotal;
        private int SendLoss;
        private int RecvLoss;

        public override string ToString()
        {
            return string.Format(Locale.Default.ToString(), Format,
                GetUid(),
                GetWidth(), GetHeight(), GetFramerate(),
                GetLastMileDelay(),
                GetVideoSendBitrate(), GetVideoRecvBitrate(),
                GetAudioSendBitrate(), GetAudioRecvBitrate(),
                GetCpuApp(), GetCpuTotal(),
                GetSendQuality(), GetRecvQuality(),
                GetSendLoss(), GetRecvLoss());
        }

        public int GetLastMileDelay()
        {
            return LastMileDelay;
        }

        public void SetLastMileDelay(int lastMileDelay)
        {
            LastMileDelay = lastMileDelay;
        }

        public int GetVideoSendBitrate()
        {
            return VideoSend;
        }

        public void SetVideoSendBitrate(int videoSend)
        {
            VideoSend = videoSend;
        }

        public int GetVideoRecvBitrate()
        {
            return VideoRecv;
        }

        public void SetVideoRecvBitrate(int videoRecv)
        {
            VideoRecv = videoRecv;
        }

        public int GetAudioSendBitrate()
        {
            return AudioSend;
        }

        public void SetAudioSendBitrate(int audioSend)
        {
            AudioSend = audioSend;
        }

        public int GetAudioRecvBitrate()
        {
            return AudioRecv;
        }

        public void SetAudioRecvBitrate(int audioRecv)
        {
            AudioRecv = audioRecv;
        }

        public double GetCpuApp()
        {
            return CpuApp;
        }

        public void SetCpuApp(double cpuApp)
        {
            CpuApp = cpuApp;
        }

        public double GetCpuTotal()
        {
            return CpuTotal;
        }

        public void SetCpuTotal(double cpuTotal)
        {
            CpuTotal = cpuTotal;
        }

        public int GetSendLoss()
        {
            return SendLoss;
        }

        public void SetSendLoss(int sendLoss)
        {
            SendLoss = sendLoss;
        }

        public int GetRecvLoss()
        {
            return RecvLoss;
        }

        public void SetRecvLoss(int recvLoss)
        {
            RecvLoss = recvLoss;
        }

    }

}