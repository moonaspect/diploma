using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;

namespace Japanese.ViewModels
{
    public class HiraganaViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<WordPair> HiraganaPairs { get; private set; } = new();

        private static readonly HttpClient HttpClient = new();
        private const string ApiUrlBase =
            "https://lkrfzpjnh7.execute-api.eu-north-1.amazonaws.com/prod/hiragana";

        private bool _isDataLoaded = false;

        public event PropertyChangedEventHandler? PropertyChanged;

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
                        HiraganaPairs.Clear();
                        foreach (var pair in wordPairsResponse.Items)
                        {
                            HiraganaPairs.Add(pair);
                        }
                    });

                    _isDataLoaded = true; // Mark data as loaded
                    LoadNextBatch(); // Load the first batch of 4 word pairs
                }
            }
            catch (HttpRequestException ex)
            {
                GameMessage = $"Failed to load words: {ex.Message}";
            }
        }

        /// <summary>
        /// Handles a successful match by updating counters and checking if the game or batch is complete.
        /// </summary>
        public void HandleMatch()
        {
            _matchedPairsCount++;
            _currentBatchMatches++;

            if (_matchedPairsCount >= MaxMatches)
            {
                GameMessage = $"Congratulations! You have matched all {MaxMatches} pairs!";
                GameOver?.Invoke(this, EventArgs.Empty); // Notify the UI that the game is over
                return;
            }

            if (_currentBatchMatches == BatchSize)
            {
                _currentBatchMatches = 0; // Reset batch counter
                GameMessage = "Batch complete! Loading next set of words...";
                LoadNextBatch(); // Load the next batch
            }
            else
            {
                GameMessage = "Correct! Keep going!";
            }
        }

        /// <summary>
        /// Tracks mismatches and updates the game message accordingly.
        /// </summary>
        public void HandleMismatch()
        {
            GameMessage = "Incorrect, try again!";
        }

        /// <summary>
        /// Loads the next batch of random word pairs from WordPairs into CurrentWordPairs.
        /// </summary>
        public void LoadNextBatch()
        {
            if (_matchedPairsCount >= MaxMatches)
            {
                GameMessage = "Congratulations! You have matched all 20 pairs!";
                GameOver?.Invoke(this, EventArgs.Empty); // Notify the UI of game-over
                return;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                CurrentJapanesePairs.Clear();
                CurrentUkrainianPairs.Clear();

                // Select BatchSize random pairs
                var nextBatch = HiraganaPairs.OrderBy(_ => _random.Next()).Take(BatchSize).ToList();

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
        /// Tracks successful matches and checks if the game is over.
        /// </summary>
        public bool CheckAndIncrementMatchCount()
        {
            _matchedPairsCount++;
            if (_matchedPairsCount >= MaxMatches)
            {
                GameOver?.Invoke(this, EventArgs.Empty); // Notify the UI
                return true;
            }
            return false;
        }

        /// <summary>
        /// Notify property change to update UI bindings.
        /// </summary>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
