namespace QuickDate.Activities.Live.Stats
{
    public class StatsData
    {
        private long Uid;
        private int Width;
        private int Height;
        private int Framerate;
        private string RecvQuality;
        private string SendQuality;

        public long GetUid()
        {
            return Uid;
        }

        public void SetUid(long uid)
        {
            Uid = uid;
        }

        public int GetWidth()
        {
            return Width;
        }

        public void SetWidth(int width)
        {
            Width = width;
        }

        public int GetHeight()
        {
            return Height;
        }

        public void SetHeight(int height)
        {
            Height = height;
        }

        public int GetFramerate()
        {
            return Framerate;
        }

        public void SetFramerate(int framerate)
        {
            Framerate = framerate;
        }

        public string GetRecvQuality()
        {
            return RecvQuality;
        }

        public void SetRecvQuality(string recvQuality)
        {
            RecvQuality = recvQuality;
        }

        public string GetSendQuality()
        {
            return SendQuality;
        }

        public void SetSendQuality(string sendQuality)
        {
            SendQuality = sendQuality;
        }
    }

}