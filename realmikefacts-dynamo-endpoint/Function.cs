using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using realmikefacts_dynamo_endpoint.Model;
using realmikefacts_dynamo_endpoint.Model.Deserializers;
using realmikefacts_dynamo_endpoint.Model.DB;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.DynamoDBv2;
using Newtonsoft.Json;
using Amazon.DynamoDBv2.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace realmikefacts_dynamo_endpoint
{
    public class Function
    {

        /// <summary>
        /// Real mike facts api gateway dynamo endpoint handler 
        /// </summary>
        /// <param name="request">The request coming from API Gateway</param>
        /// <param name="context"></param>
        /// <returns>Return test string</returns>
        public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            var responseHeaders = new Dictionary<string, string>();
            responseHeaders.Add("Content-Type", "application/json");

            try
            {
                switch (request?.RouteKey)
                {
                    case RouteKeyManager.Options:
                        return new RealMikeFactsHttpResponse
                        {
                            StatusCode = ApiStatusCode.OK,
                            Headers = responseHeaders
                        };
                    
                    case RouteKeyManager.Get:
                        return new RealMikeFactsHttpResponse
                        {
                            StatusCode = ApiStatusCode.OK,
                            Body = JsonConvert.SerializeObject(
                                new ApiBody
                                {
                                    Message = await MessageForGetRequest()
                                }),
                            Headers = responseHeaders
                        };

                    case RouteKeyManager.Put:
                        var deserializedRequestItems = JsonConvert.DeserializeObject<RequestBodyDeserialized>(request?.Body);
                        return new RealMikeFactsHttpResponse
                        {
                            StatusCode = ApiStatusCode.OK,
                            Body = JsonConvert.SerializeObject(
                                new ApiBody
                                {
                                    Message = await MessageForPutRequest(deserializedRequestItems.TweetText, deserializedRequestItems.TweetId.ToString(), deserializedRequestItems.RealMikeFacts)
                                }),
                            Headers = responseHeaders
                        };

                    default:
                        return new RealMikeFactsHttpResponse
                        {
                            StatusCode = ApiStatusCode.BAD,
                            Headers = responseHeaders
                        };
                }

            }
            catch (Exception e)
            {
                LambdaLogger.Log($"EXCEPTION: {e.Message}");
                return new RealMikeFactsHttpResponse
                {
                    StatusCode = ApiStatusCode.OK,
                    Body = JsonConvert.SerializeObject(e),
                    Headers = responseHeaders
                };
            }

        }

        private static async Task<ScanResponse> ScanDynamoTable(string tableName)
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();

            var scanRequest = new ScanRequest
            {
                TableName = "realmikefacts",
            };

            var response = await client.ScanAsync(scanRequest);

            return response;
        }

        private async Task<DynamoResonseObject> MessageForGetRequest()
        {
            var dynamoResponseObject = new DynamoResonseObject();
            
            var scanDynamoRealMikeFacts = await ScanDynamoTable("realmikefacts");
            foreach (var scanResponseItem in scanDynamoRealMikeFacts.Items.Where(item => !item["isDeleted"].BOOL))
            {
                dynamoResponseObject.TableItems.Add(new DynamoRealMikeFactsItem()
                {
                    TweetId = scanResponseItem["tweetId"].N,
                    TweetText = scanResponseItem["tweetText"].S,
                    RealMikeFacts = scanResponseItem["realmikefacts"].S, 
                    IsDeleted = "false"
                });
            }

            return dynamoResponseObject;
        }

        private async Task<DynamoResonseObject> MessageForPutRequest(string tweetText, string tweetId, string realmikefactsPk)
        {
            var client = new AmazonDynamoDBClient();
            var dynamoResponseObject = new DynamoResonseObject();

            var request = new PutItemRequest
            {
                TableName = "realmikefacts",
                Item = new Dictionary<string, AttributeValue>()
                {
                    { "tweetId", new AttributeValue { N = tweetId }},
                    { "tweetText", new AttributeValue { S = tweetText }},
                    { "realmikefacts", new AttributeValue { S = realmikefactsPk }},
                    { "isDeleted", new AttributeValue { BOOL = false }},
                }
            };

            await client.PutItemAsync(request);

            dynamoResponseObject.TableItems.Add(new DynamoRealMikeFactsItem()
            {
                TweetId = tweetId,
                TweetText = tweetText,
                RealMikeFacts = realmikefactsPk,
                IsDeleted = "false"
            });

            return dynamoResponseObject;
        }
    }
}
