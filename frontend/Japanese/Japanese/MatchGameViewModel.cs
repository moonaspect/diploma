using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;

namespace Japanese
{
    public class MatchGameViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<WordPair> WordPairs { get; set; } =
            new ObservableCollection<WordPair>();

        private static readonly HttpClient HttpClient = new HttpClient();
        private int currentPage = 1;
        private const string ApiUrlBase = "https://khbczj8mb0.execute-api.eu-north-1.amazonaws.com/prod/words?pageSize=4&pageNumber=";
        private const string ApiUrl = "https://khbczj8mb0.execute-api.eu-north-1.amazonaws.com/prod/words?pageNumber=1&pageSize=4";

        public event PropertyChangedEventHandler? PropertyChanged;
        public void IncrementPage()
        {
            currentPage++;
        }
        public async Task LoadWordsAsync()
        {
            try
            {
                var apiUrlWithPage = $"{ApiUrlBase}{currentPage}";
                var response = await HttpClient.GetAsync(apiUrlWithPage);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                var wordPairs = JsonSerializer.Deserialize<WordPairResponse>(responseBody);
                if (wordPairs != null)
                {
                    WordPairs.Clear();
                    foreach (var pair in wordPairs.Items)
                    {
                        WordPairs.Add(pair);
                    }

                    OnPropertyChanged(nameof(WordPairs));
                }
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

    public class WordPair
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Japanese { get; set; }
        public string? Ukrainian { get; set; }
    }

    public class WordPairResponse
    {
        public int TotalItems { get; set; }
        public required WordPair[] Items { get; set; }
    }
}
