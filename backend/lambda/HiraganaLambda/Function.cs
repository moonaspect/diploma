using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(
    typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer)
)]

namespace HiraganaLambda;

public class Function
{
    private static readonly string? _tableName = Environment.GetEnvironmentVariable(
        "HIRAGANA_TABLE_NAME"
    );

    public async Task<APIGatewayProxyResponse> FunctionHandler(
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

            var scanRequest = new ScanRequest { TableName = _tableName };
            var scanResponse = await dynamoDbClient.ScanAsync(scanRequest);

            var totalItems = scanResponse.Items.Count;
            var letters = scanResponse
                .Items.Select(item => new
                {
                    WordId = item["WordId"].S,
                    Japanese = item["Japanese"].S,
                    Ukrainian = item["Ukrainian"].S
                })
                .ToList();

            var responseBody = new { TotalItems = totalItems, Items = letters, };

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
