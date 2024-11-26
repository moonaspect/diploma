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
        public ObservableCollection<WordPair> CurrentJapanesePairs { get; private set; } = new();
        public ObservableCollection<WordPair> CurrentUkrainianPairs { get; private set; } = new();

        private static readonly HttpClient HttpClient = new();
        private const string ApiUrlBase =
            "https://lkrfzpjnh7.execute-api.eu-north-1.amazonaws.com/prod/words?pageSize=100&pageNumber=1";

        private readonly Random _random = new();
        private bool _isDataLoaded = false;
        private int _matchedPairsCount = 0; // Count of successfully matched pairs
        private const int MaxMatches = 8; // Stop game after 20 matches

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
                CurrentJapanesePairs.Clear();
                CurrentUkrainianPairs.Clear();

                // Select 4 random pairs
                var nextBatch = WordPairs.OrderBy(_ => _random.Next()).Take(4).ToList();

                // Shuffle Japanese and Ukrainian pairs independently
                foreach (var pair in nextBatch.OrderBy(_ => _random.Next()))
                {
                    CurrentJapanesePairs.Add(pair);
                }

                foreach (var pair in nextBatch.OrderBy(_ => _random.Next()))
                {
                    CurrentUkrainianPairs.Add(pair);
                }
            });

            OnPropertyChanged(nameof(CurrentJapanesePairs));
            OnPropertyChanged(nameof(CurrentUkrainianPairs));
        }

        /// <summary>
        /// Tracks successful matches
        /// </summary>
        public bool IsGameOver()
        {
            _matchedPairsCount++;
            return _matchedPairsCount >= MaxMatches;
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
