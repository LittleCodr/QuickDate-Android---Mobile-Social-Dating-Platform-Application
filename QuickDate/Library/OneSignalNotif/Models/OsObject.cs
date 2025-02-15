using Newtonsoft.Json;
using QuickDateClient.Classes.Global;

namespace QuickDate.Library.OneSignalNotif.Models
{
    public class OsObject
    {
        public class OsNotificationObject
        {
            [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
            public string Type { get; set; }

            [JsonProperty("userdata", NullValueHandling = NullValueHandling.Ignore)]
            public UserInfoObject UserData { get; set; }

            [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
            public string Url { get; set; }

        }
    }
}