using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Web.Server.Utils.ExternalLoginProviders.Facebook
{

    public partial class FacebookTokenValidationResult
    {
        [JsonProperty("data")]
        public Data? Data { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("app_id")]
        public long AppId { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("application")]
        public string? Application { get; set; }

        [JsonProperty("expires_at")]
        public long ExpiresAt { get; set; }

        [JsonProperty("is_valid")]
        public bool IsValid { get; set; }

        [JsonProperty("issued_at")]
        public long IssuedAt { get; set; }

        [JsonProperty("metadata")]
        public Metadata? Metadata { get; set; }

        [JsonProperty("scopes")]
        public string[]? Scopes { get; set; }

        [JsonProperty("user_id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long UserId { get; set; }
    }

    public partial class Metadata
    {
        [JsonProperty("sso")]
        public string? Sso { get; set; }
    }

    public partial class FacebookTokenValidationResult
    {
        public static FacebookTokenValidationResult? FromJson(string json) => JsonConvert.DeserializeObject<FacebookTokenValidationResult>(json, FacebookTokenValidationResultConverter.Settings);
    }

    public static class FacebookTokenValidationResultSerialize
    {
        public static string ToJson(this FacebookTokenValidationResult self) => JsonConvert.SerializeObject(self, FacebookTokenValidationResultConverter.Settings);
    }

    internal static class FacebookTokenValidationResultConverter
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

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object? ReadJson(JsonReader? reader, Type t, object? existingValue, JsonSerializer serializer)
        {
            if (reader?.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object? untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
