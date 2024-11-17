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
                    selectedJapaneseButton.Tag.ToString()
                    == selectedTranslationButton.Tag.ToString()
                )
                {
                    MessageBox.Show("Правильно!");
                    selectedJapaneseButton.IsEnabled = false;
                    selectedTranslationButton.IsEnabled = false;
                }
                else
                {
                    MessageBox.Show("Неправильно, попробуйте снова.");
                }

                selectedJapaneseButton = null;
                selectedTranslationButton = null;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}
