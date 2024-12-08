using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace Japanese.ViewModels
{
    public class DictionaryViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<WordPair> WordPairs { get; private set; } = new();

        private static readonly HttpClient HttpClient = new();
        private const string ApiUrlBase =
            "https://lkrfzpjnh7.execute-api.eu-north-1.amazonaws.com/prod/words";

        private bool _isDataLoaded = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand LoadWordsCommand { get; }

        public DictionaryViewModel()
        {
            // Initialize Commands
            LoadWordsCommand = new RelayCommand(async _ => await LoadWordsAsync());
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
                }
            }
            catch (HttpRequestException ex)
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
