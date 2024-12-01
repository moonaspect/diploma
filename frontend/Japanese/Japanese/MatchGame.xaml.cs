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
                    ViewModel.HandleMatch(); // Delegate match handling to the ViewModel
                    selectedJapaneseButton.IsEnabled = false;
                    selectedTranslationButton.IsEnabled = false;
                }
                else
                {
                    ViewModel.HandleMismatch(); // Notify ViewModel of a mismatch
                }

                selectedJapaneseButton = null;
                selectedTranslationButton = null;
            }
        }

        private void OnGameOver(object? sender, EventArgs e)
        {
            // Optionally handle additional logic if needed during game over.
            Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ChooseGame choose = new ChooseGame();
            choose.Show();
            Close();
        }
    }
}
