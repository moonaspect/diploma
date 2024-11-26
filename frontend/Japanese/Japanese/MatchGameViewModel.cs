using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace Japanese
{
    public class MatchGameViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<WordPair> WordPairs { get; set; } =
            new ObservableCollection<WordPair>();

        private static readonly HttpClient HttpClient = new HttpClient();
        private int currentPage = 1;
        private const string ApiUrlBase =
            "https://khbczj8mb0.execute-api.eu-north-1.amazonaws.com/prod/words?pageSize=4&pageNumber=";

        public event PropertyChangedEventHandler? PropertyChanged;

        public MatchGameViewModel()
        {
            // Automatically load the first set of word pairs
            _ = LoadWordsAsync();
        }

        /// <summary>
        /// Increment the page number to load the next set of word pairs.
        /// </summary>
        public void IncrementPage()
        {
            currentPage++;
        }

        /// <summary>
        /// Asynchronously loads the word pairs from the backend.
        /// </summary>
        public async Task LoadWordsAsync()
        {
            try
            {
                var apiUrlWithPage = $"{ApiUrlBase}{currentPage}";
                var response = await HttpClient.GetAsync(apiUrlWithPage);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var wordPairsResponse = JsonSerializer.Deserialize<WordPairResponse>(responseBody);

                if (wordPairsResponse != null)
                {
                    // Ensure UI updates happen on the UI thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        WordPairs.Clear();
                        foreach (var pair in wordPairsResponse.Items)
                        {
                            WordPairs.Add(pair);
                        }
                    });

                    OnPropertyChanged(nameof(WordPairs));
                }
            }
            catch (HttpRequestException ex)
            {
                // Show an error message if the API call fails
                MessageBox.Show(
                    $"Failed to load words: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Notify property change to update UI bindings.
        /// </summary>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Represents a pair of words (Japanese and Ukrainian).
    /// </summary>
    public class WordPair
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Japanese { get; set; }
        public string? Ukrainian { get; set; }
    }

    /// <summary>
    /// Represents the API response containing word pairs and metadata.
    /// </summary>
    public class WordPairResponse
    {
        public int TotalItems { get; set; }
        public WordPair[] Items { get; set; } = Array.Empty<WordPair>();
    }
}
