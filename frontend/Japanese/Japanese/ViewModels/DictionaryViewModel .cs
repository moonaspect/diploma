using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Japanese.ViewModels
{
    public class DictionaryViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<WordPair> WordPairs { get; private set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public DictionaryViewModel()
        {
            LoadWords();
        }

        /// <summary>
        /// Loads word pairs from the backend into WordPairs.
        /// </summary>
        private void LoadWords()
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
