using System.Windows;
using System.Windows.Controls;

namespace Japanese
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MatchGame : Window
    {
        private Button? selectedJapaneseButton;
        private Button? selectedTranslationButton;

        public MatchGame()
        {
            InitializeComponent();
            Loaded += async (s, e) => await ViewModel.LoadWordsAsync();
            ViewModel.GameOver += OnGameOver; // Subscribe to GameOver event
            ViewModel.BatchComplete += OnBatchComplete;
        }

        private void JapaneseWordButton_Click(object sender, RoutedEventArgs e)
        {
            selectedJapaneseButton = sender as Button;
            CheckForMatch();
        }

        private void TranslationButton_Click(object sender, RoutedEventArgs e)
        {
            selectedTranslationButton = sender as Button;
            CheckForMatch();
        }

        private void CheckForMatch()
        {
            if (selectedJapaneseButton != null && selectedTranslationButton != null)
            {
                if (selectedJapaneseButton.Uid == selectedTranslationButton.Uid)
                {
                    MessageBox.Show("Correct!");
                    selectedJapaneseButton.IsEnabled = false;
                    selectedTranslationButton.IsEnabled = false;

                    ViewModel.HandleMatch(); // Delegate match handling to the ViewModel
                }
                else
                {
                    MessageBox.Show("Incorrect, try again.");
                }

                selectedJapaneseButton = null;
                selectedTranslationButton = null;
            }
        }

        private void OnGameOver(object? sender, EventArgs e)
        {
            MessageBox.Show(
                "Congratulations! You have matched all 20 pairs!",
                "Game Over",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
            Close();
        }

        private void OnBatchComplete(object? sender, EventArgs e)
        {
            MessageBox.Show(
                "Batch complete! Loading next set of words...",
                "Info",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }
    }
}
