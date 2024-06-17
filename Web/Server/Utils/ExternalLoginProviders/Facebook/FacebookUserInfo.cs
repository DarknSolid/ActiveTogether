using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Web.Server.Utils.ExternalLoginProviders.Facebook
{
        public partial class FacebookUserInfo
        {
            [JsonProperty("first_name")]
            public string? FirstName { get; set; }

            [JsonProperty("last_name")]
            public string? LastName { get; set; }

            [JsonProperty("picture")]
            public Picture? Picture { get; set; }

            [JsonProperty("email")]
            public string? Email { get; set; }

            [JsonProperty("id")]
            public string? Id { get; set; }
        }

        public partial class Picture
        {
            [JsonProperty("data")]
            public Data? Data { get; set; }
        }

        public partial class Data
        {
            [JsonProperty("height")]
            public long Height { get; set; }

            [JsonProperty("is_silhouette")]
            public bool IsSilhouette { get; set; }

            [JsonProperty("url")]
            public Uri? Url { get; set; }

            [JsonProperty("width")]
            public long Width { get; set; }
        }

        public partial class FacebookUserInfo
        {
        public static FacebookUserInfo? FromJson(string json) => JsonConvert.DeserializeObject<FacebookUserInfo>(json, FacebookUserInfoConverter.Settings);
        }

        public static class FacebookUserInfoSerialize
        {
            public static string ToJson(this FacebookUserInfo self) => JsonConvert.SerializeObject(self, FacebookUserInfoConverter.Settings);
        }

        public static class FacebookUserInfoConverter
    {
            public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
            };
        }
}
