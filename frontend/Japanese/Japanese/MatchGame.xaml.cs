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

        //private readonly int currentBatchMatches = 0; // Number of matched pairs in the current batch
        //private readonly int totalMatches = 0; // Total number of matches across all batches
        //private const int MaxMatches = 40; // Stop the game after 40 matches

        public MatchGame()
        {
            InitializeComponent();
            Loaded += async (s, e) => await ViewModel.LoadWordsAsync();
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
                // Check if the UIDs of the selected buttons match
                if (selectedJapaneseButton.Uid == selectedTranslationButton.Uid)
                {
                    MessageBox.Show("Правильно!");
                    selectedJapaneseButton.IsEnabled = false;
                    selectedTranslationButton.IsEnabled = false;

                    //currentBatchMatches++;
                    //totalMatches++;

                    //// Check if the current batch is completed
                    //if (currentBatchMatches == 4)
                    //{
                    //    currentBatchMatches = 0;
                    //    ViewModel.LoadNextBatch(); // Load the next set of 4 pairs
                    //}

                    // Check if the game is completed
                    if (ViewModel.IsGameOver())
                    {
                        MessageBox.Show(
                            "Congratulations! You have matched all 40 pairs!",
                            "Game Over",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                        Close(); // Close the game window or restart logic
                    }
                }
                else
                {
                    MessageBox.Show("Неправильно, спробуйте знову.");
                }

                // Reset the selected buttons
                selectedJapaneseButton = null;
                selectedTranslationButton = null;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }
    }
}
