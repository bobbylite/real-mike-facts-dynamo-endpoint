using Newtonsoft.Json;
namespace realmikefacts_dynamo_endpoint.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ApiBody
    {
        [JsonProperty(PropertyName = "Message")]
        public DynamoResonseObject Message { get; set; }
    }
}
