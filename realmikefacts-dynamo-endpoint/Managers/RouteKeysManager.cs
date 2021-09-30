using System;
using System.Collections.Generic;
using System.Text;

namespace realmikefacts_dynamo_endpoint.Model
{
	public static class RouteKeyManager
	{
		public const string Put = "PUT /tweets";
		public const string Post = "POST /tweets";
		public const string Get = "GET /tweets";
		public const string Options = "OPTIONS /tweets";


		/// <summary>
		/// Creates route key from HTTP method type string and the api path string
		/// </summary>
		/// <param name="method">HTTP method type</param>
		/// <param name="path">API Endpoint path</param>
		/// <returns>Route key string</returns>
		public static string CreateRouteKey(string method, string path)
		{
			return $"{method} {path}";
		}
	}
}
