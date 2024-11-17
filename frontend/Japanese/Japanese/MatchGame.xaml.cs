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
                if (
                    selectedJapaneseButton.Uid.ToString()
                    == selectedTranslationButton.Uid.ToString()
                )
                {
                    MessageBox.Show("Правильно!");
                    selectedJapaneseButton.IsEnabled = false;
                    selectedTranslationButton.IsEnabled = false;
                }
                else
                {
                    MessageBox.Show("Неправильно, спробуйте знову.");
                }

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
