using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace realmikefacts_dynamo_endpoint.Model
{
	[JsonObject(MemberSerialization.OptIn)]
	public class ApiHeaders
	{
		public ApiHeaders(string contentType = null)
		{
			ContentType = contentType;
		}

		[JsonProperty(PropertyName = "Content-Type")]
		public string ContentType { get; set; }
	}
}
