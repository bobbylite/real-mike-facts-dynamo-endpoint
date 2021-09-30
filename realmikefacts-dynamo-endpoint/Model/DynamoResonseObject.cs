using Newtonsoft.Json;
using realmikefacts_dynamo_endpoint.Model.DB;
using System.Collections.Generic;

namespace realmikefacts_dynamo_endpoint.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DynamoResonseObject
    {
        [JsonProperty(PropertyName = "Items")]
        public List<DynamoRealMikeFactsItem> TableItems { get; set; } = new List<DynamoRealMikeFactsItem>();
    }
}
