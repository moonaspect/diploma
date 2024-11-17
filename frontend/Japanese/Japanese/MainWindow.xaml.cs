using System.Windows;

namespace Japanese
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonMatch(object sender, RoutedEventArgs e)
        {
            MatchGame matchGame = new MatchGame();
            matchGame.Show();
            this.Close();
        }
    }
}
