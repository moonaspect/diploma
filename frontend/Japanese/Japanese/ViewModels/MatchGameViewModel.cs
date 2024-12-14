using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Windows;

namespace Japanese.ViewModels
{
    public class MatchGameViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<WordPair> WordPairs { get; private set; } = new();
        public ObservableCollection<WordPair> CurrentJapanesePairs { get; private set; } = new();
        public ObservableCollection<WordPair> CurrentUkrainianPairs { get; private set; } = new();

        private readonly Random _random = new();
        private int _matchedPairsCount = 0; // Count of successfully matched pairs
        private int _currentBatchMatches = 0; // Matches in the current batch
        private const int MaxMatches = 20; // Stop game after 20 matches
        private const int BatchSize = 4; // Number of pairs in a batch

        private string _gameMessage = "Match the words!"; // Default game message
        public string GameMessage
        {
            get => _gameMessage;
            set
            {
                _gameMessage = value;
                OnPropertyChanged(nameof(GameMessage));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public event EventHandler? GameOver; // Event to notify when the game is over

        public MatchGameViewModel()
        {
            LoadWords();
        }

        /// <summary>
        /// Loads word pairs from the backend into WordPairs.
        /// </summary>
        private void LoadWords()
        {
            try
            {
                var wordPairService = new WordPairService(
                    new PersistentCache<WordPair>("wordpairs.json")
                );
                var pairs = wordPairService.GetWordPairs();

                if (pairs == null)
                    return;

                WordPairs.Clear();
                foreach (var pair in pairs)
                {
                    WordPairs.Add(pair);
                }

                LoadNextBatch();
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
                var nextBatch = WordPairs.OrderBy(_ => _random.Next()).Take(BatchSize).ToList();

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
