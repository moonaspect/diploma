using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

[assembly: LambdaSerializer(
    typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer)
)]

namespace GameRecordsLambda
{
    public class Function
    {
        private static readonly string? _tableName = Environment.GetEnvironmentVariable(
            "RECORDS_TABLE_NAME"
        );

        public async Task<APIGatewayProxyResponse> FunctionHandler(
            APIGatewayProxyRequest request,
            ILambdaContext context
        )
        {
            try
            {
                context.Logger.LogInformation(
                    $"Received Event: {JsonSerializer.Serialize(request)}"
                );

                AmazonDynamoDBConfig config = new AmazonDynamoDBConfig { LogMetrics = true };
                var dynamoDbClient = new AmazonDynamoDBClient(config);

                if (request.HttpMethod == "POST")
                {
                    // Add record to DynamoDB
                    var requestBody = JsonSerializer.Deserialize<Dictionary<string, string>>(
                        request.Body
                    );

                    var playerId = requestBody["PlayerId"];
                    var score = int.Parse(requestBody["Score"]);

                    var putRequest = new PutItemRequest
                    {
                        TableName = _tableName,
                        Item = new Dictionary<string, AttributeValue>
                        {
                            {
                                "PlayerId",
                                new AttributeValue { S = playerId }
                            },
                            {
                                "Score",
                                new AttributeValue { N = score.ToString() }
                            }
                        }
                    };

                    await dynamoDbClient.PutItemAsync(putRequest);

                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 200,
                        Body = "Record added successfully.",
                        Headers = new Dictionary<string, string>
                        {
                            { "Content-Type", "text/plain" }
                        }
                    };
                }
                else if (request.HttpMethod == "GET")
                {
                    // Retrieve records with pagination
                    var pageNumber = request.QueryStringParameters.ContainsKey("pageNumber")
                        ? int.Parse(request.QueryStringParameters["pageNumber"])
                        : 1;

                    var pageSize = request.QueryStringParameters.ContainsKey("pageSize")
                        ? int.Parse(request.QueryStringParameters["pageSize"])
                        : 10;

                    var scanRequest = new ScanRequest { TableName = _tableName };
                    var scanResponse = await dynamoDbClient.ScanAsync(scanRequest);

                    var totalItems = scanResponse.Items.Count;
                    var paginatedItems = scanResponse
                        .Items.Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .Select(item => new
                        {
                            PlayerId = item["PlayerId"].S,
                            Score = int.Parse(item["Score"].N)
                        })
                        .ToList();

                    var responseBody = new
                    {
                        TotalItems = totalItems,
                        Items = paginatedItems,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };

                    var options = new JsonSerializerOptions
                    {
                        Encoder = System
                            .Text
                            .Encodings
                            .Web
                            .JavaScriptEncoder
                            .UnsafeRelaxedJsonEscaping,
                        WriteIndented = false
                    };

                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 200,
                        Body = JsonSerializer.Serialize(responseBody, options),
                        Headers = new Dictionary<string, string>
                        {
                            { "Content-Type", "application/json" }
                        }
                    };
                }
                else
                {
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 405,
                        Body = "Method not allowed.",
                        Headers = new Dictionary<string, string>
                        {
                            { "Content-Type", "text/plain" }
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error: {ex.Message}");
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = $"Internal server error: {ex.Message}",
                    Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
                };
            }
        }
    }
}
