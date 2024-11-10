using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(
    typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer)
)]

namespace MatchingPairsLambda
{
    public class Function
    {
        private static readonly string? _tableName = Environment.GetEnvironmentVariable(
            "TABLE_NAME"
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

                // Get query parameters
                var pageNumber = request.QueryStringParameters.ContainsKey("pageNumber")
                    ? int.Parse(request.QueryStringParameters["pageNumber"])
                    : 1;

                var pageSize = request.QueryStringParameters.ContainsKey("pageSize")
                    ? int.Parse(request.QueryStringParameters["pageSize"])
                    : 10;

                context.Logger.LogInformation(
                    $"PageNumber: {pageNumber}, PageSize: {pageSize}, TableName: {_tableName ?? "MatchingPairsTable"}"
                );

                AmazonDynamoDBConfig config = new AmazonDynamoDBConfig { LogMetrics = true };
                var dynamoDbClient = new AmazonDynamoDBClient(config);

                ScanResponse? scanResponse = null;

                try
                {
                    context.Logger.LogInformation(
                        $"AWS Region: {dynamoDbClient.Config.RegionEndpoint}"
                    );

                    // Scan DynamoDB Table
                    var scanRequest = new ScanRequest { TableName = _tableName, };

                    scanResponse = await dynamoDbClient.ScanAsync(scanRequest);

                    context.Logger.LogInformation($"Scan response: {scanResponse.ScannedCount}");
                }
                catch (Exception ex)
                {
                    context.Logger.LogError($"Error scanning DynamoDB table: {ex.Message}");
                    context.Logger.LogError($"Stack Trace: {ex.StackTrace}");
                    throw; // Rethrow to ensure the exception is captured in the Lambda response
                }

                // Apply pagination manually
                var totalItems = scanResponse.Items.Count;
                var paginatedItems = scanResponse
                    .Items.Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(item => new
                    {
                        WordId = item["WordId"].S,
                        Japanese = item["Japanese"].S,
                        Ukrainian = item["Ukrainian"].S
                    })
                    .ToList();

                context.Logger.LogInformation($"Total Items Retrieved: {paginatedItems.Count}");

                // Return the response
                var responseBody = new
                {
                    TotalItems = totalItems,
                    Items = paginatedItems,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var options = new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = false
                };
                var responseBodyJson = JsonSerializer.Serialize(responseBody, options);

                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = responseBodyJson,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json" }
                    }
                };
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
