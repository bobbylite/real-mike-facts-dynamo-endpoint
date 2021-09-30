using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace realmikefacts_dynamo_endpoint.Model.Deserializers
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RequestBodyDeserialized
    {
        [JsonProperty(PropertyName = "realmikefacts")]
        public string RealMikeFacts { get; set; }

        [JsonProperty(PropertyName = "isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty(PropertyName = "tweetId")]
        public int TweetId { get; set; }

        [JsonProperty(PropertyName = "tweetText")]
        public string TweetText { get; set; }

    }
}
