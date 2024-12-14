using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Japanese.ViewModels
{
    public class KatakanaViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<WordPair> KatakanaPairs { get; private set; } = new();

        private const string ApiUrlBase =
            "https://lkrfzpjnh7.execute-api.eu-north-1.amazonaws.com/prod/katakana";

        public event PropertyChangedEventHandler? PropertyChanged;

        public KatakanaViewModel()
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
                    ApiUrlBase,
                    new PersistentCache<WordPair>("katakana.json")
                );
                var pairs = wordPairService.GetWordPairs();

                if (pairs == null)
                    return;

                KatakanaPairs.Clear();
                foreach (var pair in pairs)
                {
                    KatakanaPairs.Add(pair);
                }

                OnPropertyChanged(nameof(KatakanaPairs));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load words: {ex.Message}");
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
}
