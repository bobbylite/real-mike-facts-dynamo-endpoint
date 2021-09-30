using Newtonsoft.Json;

namespace realmikefacts_dynamo_endpoint.Model.DB
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DynamoRealMikeFactsItem
    {
        [JsonProperty(PropertyName = "realmikefacts")]
        public string RealMikeFacts { get; set; }

        [JsonProperty(PropertyName = "isDeleted")]
        public string IsDeleted { get; set; }

        [JsonProperty(PropertyName = "tweetId")]
        public string TweetId { get; set; }

        [JsonProperty(PropertyName = "tweetText")]
        public string TweetText { get; set; }

    }
}
