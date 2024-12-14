using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Japanese.ViewModels
{
    public class HiraganaViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<WordPair> HiraganaPairs { get; private set; } = new();

        private const string ApiUrlBase =
            "https://lkrfzpjnh7.execute-api.eu-north-1.amazonaws.com/prod/hiragana";

        public event PropertyChangedEventHandler? PropertyChanged;

        public HiraganaViewModel()
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
                    new PersistentCache<WordPair>("hiragana.json")
                );
                var pairs = wordPairService.GetWordPairs();

                if (pairs == null)
                    return;

                HiraganaPairs.Clear();
                foreach (var pair in pairs)
                {
                    HiraganaPairs.Add(pair);
                }

                OnPropertyChanged(nameof(HiraganaPairs));
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
