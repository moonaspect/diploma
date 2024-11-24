using System.Windows;
using System.Windows.Controls;
using SharpVectors.Converters;

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

            // Create the main Grid
            Grid mainGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(200) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };

            // Create the StackPanel for the first column
            StackPanel stackPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };

            // Add buttons to the StackPanel
            Button buttonMatch = new Button { Content = "Гра на зіставлення", };
            buttonMatch.Click += ButtonMatch;

            Button buttonDictionary = new Button { Content = "Словник" };

            Button buttonRecords = new Button { Content = "Рекорди" };
            buttonRecords.Click += ButtonRecords;

            stackPanel.Children.Add(buttonMatch);
            stackPanel.Children.Add(buttonDictionary);
            stackPanel.Children.Add(buttonRecords);

            // Add the StackPanel to the first column of the Grid
            Grid.SetColumn(stackPanel, 0);
            mainGrid.Children.Add(stackPanel);

            // Create the Button with SvgViewbox for the second column
            Button svgButton = new Button { Width = 100, Height = 50 };

            SvgViewbox svgViewbox = new SvgViewbox
            {
                Source = new Uri("pack://application:,,,/Variant2.svg"),
                Stretch = System.Windows.Media.Stretch.Uniform
            };

            // Add SvgViewbox to the Button's content
            svgButton.Content = svgViewbox;

            // Add the Button to the second column of the Grid
            Grid.SetColumn(svgButton, 1);
            mainGrid.Children.Add(svgButton);

            // Set the Grid as the content of the Window
            this.Content = mainGrid;
        }

        private void ButtonMatch(object sender, RoutedEventArgs e)
        {
            MatchGame matchGame = new MatchGame();
            matchGame.Show();
            Close();
        }

        private void ButtonRecords(object sender, RoutedEventArgs e)
        {
            RecordsTable recordsTable = new RecordsTable();
            recordsTable.Show();
            Close();
        }
    }
}
