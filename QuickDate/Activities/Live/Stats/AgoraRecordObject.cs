using Newtonsoft.Json;

namespace QuickDate.Activities.Live.Stats
{
    public class AgoraRecordObject
    {
        [JsonProperty("resourceId", NullValueHandling = NullValueHandling.Ignore)]
        public string ResourceId { get; set; }

        [JsonProperty("sid", NullValueHandling = NullValueHandling.Ignore)]
        public string Sid { get; set; }

        [JsonProperty("serverResponse", NullValueHandling = NullValueHandling.Ignore)]
        public ServerResponse ServerResponse { get; set; }
    }

    public class ServerResponse
    {
        [JsonProperty("fileListMode", NullValueHandling = NullValueHandling.Ignore)]
        public string FileListMode { get; set; }

        [JsonProperty("fileList", NullValueHandling = NullValueHandling.Ignore)]
        public string FileList { get; set; }

        [JsonProperty("uploadingStatus", NullValueHandling = NullValueHandling.Ignore)]
        public string UploadingStatus { get; set; }
    }
}