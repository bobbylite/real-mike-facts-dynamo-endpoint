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
using System.Net;

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
                        var optionsResponse = new RealMikeFactsHttpResponse
                        {
                            StatusCode = ApiStatusCode.OK,
                            Headers = responseHeaders
                        };

                        return optionsResponse;

                    case RouteKeyManager.Get:
                        var cookiesFound = request.Headers.Keys;
                        if (true)
                        {
                            return new RealMikeFactsHttpResponse
                            {
                                StatusCode = ApiStatusCode.OK,
                                Body = JsonConvert.SerializeObject(request),
                                Headers = responseHeaders
                            };
                        }
                            
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

                        if (deserializedRequestItems == null)
                            throw new NullReferenceException("Post Request Server Error");

                        var putTweetText = deserializedRequestItems.TweetText;
                        var putTweetId = deserializedRequestItems.TweetId.ToString();
                        var putPk = deserializedRequestItems.RealMikeFacts;

                        return new RealMikeFactsHttpResponse
                        {
                            StatusCode = ApiStatusCode.OK,
                            Body = JsonConvert.SerializeObject(
                                new ApiBody
                                {
                                    Message = await MessageForPutRequest(putTweetText, putTweetId, putPk)
                                }),
                            Headers = responseHeaders
                        };

                    case RouteKeyManager.Post:
                        var deserializedPostRequestItems = JsonConvert.DeserializeObject<PostRequestDeserialized>(request?.Body);

                        if (deserializedPostRequestItems == null)
                            throw new NullReferenceException("Post Request Server Error");

                        if (true)
                        {
                            var cookie = new Cookie("JWT", deserializedPostRequestItems.JwtToken)
                            {
                                HttpOnly = true,
                                Expires = DateTime.Now.AddDays(1),
                                Expired = false
                            };
                            var serializedCookie = $"{cookie}; HttpOnly; Expires={cookie.Expires}; secure; SameSite=Lax;";
                            var response =  new RealMikeFactsHttpResponse
                            {
                                StatusCode = ApiStatusCode.OK,
                                Body = JsonConvert.SerializeObject(request),
                                Headers = responseHeaders
                            };

                            response.SetHeaderValues("Set-Cookie", serializedCookie, true);
                            response.SetHeaderValues("access-control-expose-headers", "Set-Cookie", true);
                            response.SetHeaderValues("Access-Control-Allow-Headers", "Set-Cookie", true);
                            response.SetHeaderValues("Access-Control-Allow-Credentials", "true", true);
                            response.SetHeaderValues("Access-Control-Allow-Origin", "https://api.realmikefacts.com", true);
                            response.SetHeaderValues("Access-Control-Allow-Headers", "Set-Cookie", true);

                            return response;
                        }

                        return new RealMikeFactsHttpResponse
                        {
                            StatusCode = ApiStatusCode.BAD,
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
                    StatusCode = ApiStatusCode.BAD,
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

        private async Task<DynamoResonseObject> MessageForPostRequest()
        {
            var dynamoResponseObject = new DynamoResonseObject();
            dynamoResponseObject.TableItems.Add(new DynamoRealMikeFactsItem()
            {
                TweetId = string.Empty,
                TweetText = string.Empty,
                RealMikeFacts = string.Empty,
                IsDeleted = "false"
            });
            return dynamoResponseObject;
        }
    }
}
