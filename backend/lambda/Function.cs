using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(
    typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer)
)]

namespace MatchingPairsLambda
{
    public class Function
    {
        private static readonly AmazonDynamoDBClient DynamoDbClient = new AmazonDynamoDBClient();
        private static readonly string TableName = Environment.GetEnvironmentVariable("TABLE_NAME");

        public async Task<APIGatewayProxyResponse> FunctionHandler(
            APIGatewayProxyRequest request,
            ILambdaContext context
        )
        {
            var action = request.QueryStringParameters?["action"] ?? "default";
            var pageSize = int.Parse(request.QueryStringParameters?["pageSize"] ?? "10");

            if (action == "random")
            {
                return await GetRandomWords(pageSize);
            }
            else
            {
                var pageNumber = int.Parse(request.QueryStringParameters?["pageNumber"] ?? "1");
                return await GetWordsByPage(pageNumber, pageSize);
            }
        }

        private async Task<APIGatewayProxyResponse> GetWordsByPage(int pageNumber, int pageSize)
        {
            // Calculate the number of items to skip
            var skipCount = (pageNumber - 1) * pageSize;

            var request = new ScanRequest { TableName = TableName, Limit = skipCount + pageSize };

            var result = await DynamoDbClient.ScanAsync(request);
            var items = result
                .Items.Skip(skipCount)
                .Take(pageSize)
                .Select(item => new
                {
                    UkrainianWord = item["UkrainianWord"].S,
                    JapaneseWord = item["JapaneseWord"].S
                })
                .ToList();

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(new { words = items }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        private async Task<APIGatewayProxyResponse> GetRandomWords(int pageSize)
        {
            var request = new ScanRequest { TableName = TableName };

            var result = await DynamoDbClient.ScanAsync(request);
            var randomItems = result
                .Items.OrderBy(x => Guid.NewGuid())
                .Take(pageSize)
                .Select(item => new
                {
                    UkrainianWord = item["UkrainianWord"].S,
                    JapaneseWord = item["JapaneseWord"].S
                })
                .ToList();

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(new { words = randomItems }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}
