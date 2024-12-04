using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace Japanese
{
    public class RecordsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<GameRecord> GameRecords { get; set; } =
            new ObservableCollection<GameRecord>();

        private static readonly HttpClient HttpClient = new HttpClient();
        private const string GetRecordsUrl =
            "https://lkrfzpjnh7.execute-api.eu-north-1.amazonaws.com/prod/records";
        private const string SaveRecordsUrl =
            "https://lkrfzpjnh7.execute-api.eu-north-1.amazonaws.com/prod/birecords";

        public event PropertyChangedEventHandler? PropertyChanged;

        // Commands
        public ICommand LoadRecordsCommand { get; }

        public ICommand SaveRecordCommand { get; }

        public RecordsViewModel()
        {
            // Initialize Commands
            LoadRecordsCommand = new RelayCommand(async _ => await LoadRecordsAsync());
            SaveRecordCommand = new RelayCommand(async _ =>
                await SaveRecordAsync(new GameRecord { PlayerId = "NewPlayer", Score = 100 })
            );
        }

        public async Task LoadRecordsAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var requestUrl = $"{GetRecordsUrl}?pageNumber={pageNumber}&pageSize={pageSize}";
                var response = await HttpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                var recordsResponse = JsonSerializer.Deserialize<GameRecordsResponse>(responseBody);
                if (recordsResponse != null)
                {
                    GameRecords.Clear();
                    foreach (var record in recordsResponse.Items)
                    {
                        GameRecords.Add(record);
                    }

                    OnPropertyChanged(nameof(GameRecords));
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle error
                Console.WriteLine($"Error loading records: {ex.Message}");
            }
        }

        public async Task SaveRecordAsync(GameRecord record)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(record);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await HttpClient.PostAsync(GetRecordsUrl, httpContent);
                response.EnsureSuccessStatusCode();
                await LoadRecordsAsync(); // Reload the table after saving
            }
            catch (HttpRequestException ex)
            {
                // Handle error
                Console.WriteLine($"Error saving record: {ex.Message}");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class GameRecord
    {
        public string PlayerId { get; set; } = Guid.NewGuid().ToString();
        public int Score { get; set; }
    }

    public class GameRecordsResponse
    {
        public int TotalItems { get; set; }
        public required GameRecord[] Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class RelayCommand : ICommand
    {
        private readonly Func<object?, Task> execute;
        private readonly Func<object?, bool>? canExecute;

        public RelayCommand(Func<object?, Task> execute, Func<object?, bool>? canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => canExecute == null || canExecute(parameter);

        public async void Execute(object? parameter) => await execute(parameter);

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
