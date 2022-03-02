using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace realmikefacts_dynamo_endpoint.Model.Cookie
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Cookie
    {
        [JsonProperty(PropertyName = "JwtString")]
        public string JwtString;
    }

}
