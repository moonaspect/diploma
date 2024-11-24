using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Japanese
{
    public class RecordsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<GameRecord> GameRecords { get; set; } =
            new ObservableCollection<GameRecord>();

        private static readonly HttpClient HttpClient = new HttpClient();
        private const string ApiUrl =
            "https://lkrfzpjnh7.execute-api.eu-north-1.amazonaws.com/prod/records";

        public event PropertyChangedEventHandler? PropertyChanged;

        // Method to load records from the API
        public async Task LoadRecordsAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var requestUrl = $"{ApiUrl}?pageNumber={pageNumber}&pageSize={pageSize}";
                var response = await HttpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                var recordsResponse = JsonSerializer.Deserialize<GameRecordsResponse>(responseBody);
                if (recordsResponse != null)
                {
                    GameRecords.Clear();
                    foreach (var record in recordsResponse.Items)
                    {
                        GameRecords.Add(record);
                    }

                    OnPropertyChanged(nameof(GameRecords));
                }
            }
            catch (HttpRequestException)
            {
                // Handle error (e.g., log it or display a message to the user)
            }
        }

        // Method to save a record to the API
        public async Task SaveRecordAsync(GameRecord record)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(record);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await HttpClient.PostAsync(ApiUrl, httpContent);
                response.EnsureSuccessStatusCode();
                // Optionally handle the response if needed
            }
            catch (HttpRequestException)
            {
                // Handle error (e.g., log it or display a message to the user)
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Record class for game records
    public class GameRecord
    {
        public string PlayerId { get; set; } = Guid.NewGuid().ToString();
        public int Score { get; set; }
    }

    // Response class for the game records API
    public class GameRecordsResponse
    {
        public int TotalItems { get; set; }
        public required GameRecord[] Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
