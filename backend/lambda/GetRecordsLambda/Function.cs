using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(
    typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer)
)]

namespace GetRecordsLambda;

public class Function
{
    private static readonly string? _tableName = Environment.GetEnvironmentVariable(
        "RECORDS_TABLE_NAME"
    );

    public async Task<APIGatewayProxyResponse> GetRecordsHandler(
        APIGatewayProxyRequest request,
        ILambdaContext context
    )
    {
        // Log the incoming request for debugging
        context.Logger.LogInformation($"Received Event: {JsonSerializer.Serialize(request)}");
        context.Logger.LogInformation($"Table name: {_tableName}");

        try
        {
            AmazonDynamoDBConfig config = new AmazonDynamoDBConfig { LogMetrics = true };
            var dynamoDbClient = new AmazonDynamoDBClient(config);

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
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            };

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(responseBody, options),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error: {ex.Message}");
            return new APIGatewayProxyResponse
            {
                StatusCode = 500,
                Body = "Internal Server Error",
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };
        }
    }
}
