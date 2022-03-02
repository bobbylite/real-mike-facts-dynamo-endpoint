using System.Collections.Generic;
using Xunit;
using Amazon.Lambda.TestUtilities;
using realmikefacts_dynamo_endpoint.Model.DB;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Amazon.DynamoDBv2.Model;

namespace realmikefacts_dynamo_endpoint.Tests
{
    public class FunctionTest
    {
        [Fact]
        public async void TestGetRequest()
        {
            var function = new Function();
            var context = new TestLambdaContext();
            var responseTestHeaders = new Dictionary<string, string>();
            responseTestHeaders.Add("Content-Type", "application/json");

            var request = new APIGatewayHttpApiV2ProxyRequest

            {
                Body = JsonConvert.SerializeObject(new Dictionary<string, AttributeValue>
                {
                    { "tweetId", new AttributeValue { N = $"9999" }},
                    { "tweetText", new AttributeValue { S = $" test " }},
                    { "realmikefacts", new AttributeValue { S = $"http://realmikefacts.com" }},
                    { "isDeleted", new AttributeValue { BOOL = false }},
                }),
                Headers = responseTestHeaders,
                RouteKey = "GET /tweets"
            };

            var testResponse = await function.FunctionHandler(request, context);
            var expectedResponse = new APIGatewayHttpApiV2ProxyResponse();
            expectedResponse.Body = JsonConvert.SerializeObject(new DynamoRealMikeFactsItem
            {
                TweetId = $"9999",
                TweetText = $"9999",
                RealMikeFacts = $"9999",
                IsDeleted = "false"
            });

            var actualResponse = JsonConvert.DeserializeObject<DynamoRealMikeFactsItem>(testResponse.Body);

            Assert.Equal
            (
                expectedResponse.ToString().Contains("Body"),
                testResponse.ToString().Contains("Body")
            );
        }

        [Fact]
        public void UnknownGetPathRequest()
        {

            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            var responseTestHeaders = new Dictionary<string, string>();
            responseTestHeaders.Add("Content-Type", "application/json");

            var request = new APIGatewayHttpApiV2ProxyRequest

            {
                Body = string.Empty,
                Headers = responseTestHeaders,
                RouteKey = "GET /incorrect"
            };

            var testResponse = function.FunctionHandler(request, context);

            Assert.NotEqual
            (
                200,
                testResponse.Result.StatusCode
            );

            Assert.Equal
            (
                400,
                testResponse.Result.StatusCode
            );
        }
    }
}