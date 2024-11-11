using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Japanese
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MatchGame : Window
    {
        private Button selectedJapaneseButton = null;
        private Button selectedTranslationButton = null;
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
                if (selectedJapaneseButton.Tag.ToString() == selectedTranslationButton.Tag.ToString())
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
