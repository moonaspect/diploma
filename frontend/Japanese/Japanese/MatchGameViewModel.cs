using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace Japanese
{
    public class MatchGameViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<WordPair> WordPairs { get; private set; } = new();
        public ObservableCollection<WordPair> CurrentWordPairs { get; private set; } = new();

        private static readonly HttpClient HttpClient = new();
        private const string ApiUrlBase =
            "https://lkrfzpjnh7.execute-api.eu-north-1.amazonaws.com/prod/words?pageSize=100&pageNumber=1";

        private readonly Random _random = new();
        private bool _isDataLoaded = false;
        private int _matchedPairsCount = 0; // Count of successfully matched pairs
        private const int MaxMatches = 40; // Stop game after 40 matches

        public event PropertyChangedEventHandler? PropertyChanged;

        public MatchGameViewModel()
        {
            _ = LoadWordsAsync();
        }

        /// <summary>
        /// Asynchronously loads word pairs from the backend into WordPairs.
        /// </summary>
        public async Task LoadWordsAsync()
        {
            if (_isDataLoaded)
            {
                // If data is already loaded, no need to fetch again
                return;
            }

            try
            {
                var response = await HttpClient.GetAsync(ApiUrlBase);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var wordPairsResponse = JsonSerializer.Deserialize<WordPairResponse>(responseBody);

                if (wordPairsResponse != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        WordPairs.Clear();
                        foreach (var pair in wordPairsResponse.Items)
                        {
                            WordPairs.Add(pair);
                        }
                    });

                    _isDataLoaded = true; // Mark data as loaded
                    LoadNextBatch(); // Load the first batch of 4 word pairs
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(
                    $"Failed to load words: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Loads the next batch of 4 random word pairs from WordPairs into CurrentWordPairs.
        /// </summary>
        public void LoadNextBatch()
        {
            if (_matchedPairsCount >= MaxMatches)
            {
                MessageBox.Show(
                    "Congratulations! You have matched all 40 pairs!",
                    "Game Over",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                CurrentWordPairs.Clear();
                var nextBatch = new List<WordPair>();

                // Select 4 random pairs with possible repetition
                for (int i = 0; i < 4; i++)
                {
                    var randomIndex = _random.Next(WordPairs.Count);
                    var wordPair = WordPairs[randomIndex];
                    nextBatch.Add(wordPair);
                }

                foreach (var pair in nextBatch)
                {
                    CurrentWordPairs.Add(pair);
                }
            });

            OnPropertyChanged(nameof(CurrentWordPairs));
        }

        /// <summary>
        /// Tracks successful matches and stops the game after 40 matches.
        /// </summary>
        public void IncrementMatchCount()
        {
            _matchedPairsCount++;
            if (_matchedPairsCount >= MaxMatches)
            {
                MessageBox.Show(
                    "You have matched all the required pairs! Well done!",
                    "Game Over",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
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
        [System.Text.Json.Serialization.JsonIgnore]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? WordId { get; set; }
        public string? Japanese { get; set; }
        public string? Ukrainian { get; set; }
    }

    /// <summary>
    /// Represents the API response containing word pairs and metadata.
    /// </summary>
    public class WordPairResponse
    {
        public int TotalItems { get; set; }
        public List<WordPair> Items { get; set; } = new();
    }
}
