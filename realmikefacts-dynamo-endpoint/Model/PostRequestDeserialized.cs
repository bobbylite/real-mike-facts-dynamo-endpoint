using Newtonsoft.Json;
using realmikefacts_dynamo_endpoint.Model.Cookie;

namespace realmikefacts_dynamo_endpoint.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PostRequestDeserialized
    {
        [JsonProperty(PropertyName = "SetCookie")]
        public bool SetCookie { get; set; }

        [JsonProperty(PropertyName = "JwtToken")]
        public string JwtToken { get; set; }
    }
}
