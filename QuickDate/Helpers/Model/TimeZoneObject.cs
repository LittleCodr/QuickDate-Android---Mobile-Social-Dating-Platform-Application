using Newtonsoft.Json;

namespace QuickDate.Helpers.Model
{
    public class TimeZoneObject
    {
        [JsonProperty("as", NullValueHandling = NullValueHandling.Ignore)]
        public string As { get; set; }

        [JsonProperty("city", NullValueHandling = NullValueHandling.Ignore)]
        public string City { get; set; }

        [JsonProperty("country", NullValueHandling = NullValueHandling.Ignore)]
        public string Country { get; set; }

        [JsonProperty("countryCode", NullValueHandling = NullValueHandling.Ignore)]
        public string CountryCode { get; set; }

        [JsonProperty("isp", NullValueHandling = NullValueHandling.Ignore)]
        public string Isp { get; set; }

        [JsonProperty("lat", NullValueHandling = NullValueHandling.Ignore)]
        public double Lat { get; set; }

        [JsonProperty("lon", NullValueHandling = NullValueHandling.Ignore)]
        public double Lon { get; set; }

        [JsonProperty("org", NullValueHandling = NullValueHandling.Ignore)]
        public string Org { get; set; }

        [JsonProperty("query", NullValueHandling = NullValueHandling.Ignore)]
        public string Query { get; set; }

        [JsonProperty("region", NullValueHandling = NullValueHandling.Ignore)]
        public string Region { get; set; }

        [JsonProperty("regionName", NullValueHandling = NullValueHandling.Ignore)]
        public string RegionName { get; set; }

        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }

        [JsonProperty("timezone", NullValueHandling = NullValueHandling.Ignore)]
        public string Timezone { get; set; }

        [JsonProperty("zip", NullValueHandling = NullValueHandling.Ignore)]
        public string Zip { get; set; }
    }
}