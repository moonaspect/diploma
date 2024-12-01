using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
                Background = Brushes.Transparent
            };

            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Background
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto}); // Bottom Buttons


            SvgViewbox bgViewbox = new SvgViewbox
            {
                Source = new Uri("pack://application:,,,/bgmain.svg"),
                Stretch = System.Windows.Media.Stretch.UniformToFill,
            };

            Grid.SetRow(bgViewbox, 0);
            Grid.SetRowSpan(bgViewbox, 2);
            mainGrid.Children.Add(bgViewbox);


            SvgViewbox playViewbox = new SvgViewbox
            {
                Source = new Uri("pack://application:,,,/play.svg"),
            };
            SvgViewbox dictViewbox = new SvgViewbox
            {
                Source = new Uri("pack://application:,,,/dictionary.svg"),
            };
            SvgViewbox recViewbox = new SvgViewbox
            {
                Source = new Uri("pack://application:,,,/stats.svg"),
            };
            SvgViewbox closeViewbox = new SvgViewbox
            {
                Source = new Uri("pack://application:,,,/exit.svg"),
            };


            Button buttonMatch = CreateButton(playViewbox);
            buttonMatch.Click += ButtonMatch;
            buttonMatch.Width = 165;
            buttonMatch.Height = 78;

            Button buttonDictionary = CreateButton(dictViewbox);
            buttonDictionary.Width = 180;
            buttonDictionary.Height = 43;

            Button buttonRecords = CreateButton(recViewbox);
            buttonRecords.Click += ButtonRecords;
            buttonRecords.Width = 180;
            buttonRecords.Height = 43;

            Button closeButton = CreateButton(closeViewbox);
            closeButton.HorizontalAlignment = HorizontalAlignment.Right;
            closeButton.VerticalAlignment = VerticalAlignment.Top;
            closeButton.Width = 43;
            closeButton.Height = 43;
            closeButton.Click += (s, e) => this.Close();
            closeButton.Margin = new Thickness(0, 10, 10, 0);

            Grid buttonGrid = new Grid
            {
                Background = Brushes.Transparent,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 10, 0, 10)
            };

            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());

            Grid.SetColumn(buttonMatch, 0);
            Grid.SetColumn(buttonDictionary, 1);
            Grid.SetColumn(buttonRecords, 2);

            buttonGrid.Children.Add(buttonMatch);
            buttonGrid.Children.Add(buttonDictionary);
            buttonGrid.Children.Add(buttonRecords);

            Grid.SetRow(buttonGrid, 1);
            mainGrid.Children.Add(buttonGrid);
            mainGrid.Children.Add(closeButton);


            this.Content = mainGrid;
        }

        private Button CreateButton(SvgViewbox content)
        {
            // Create the ControlTemplate
            var template = new ControlTemplate(typeof(Button))
            {
                VisualTree = new FrameworkElementFactory(typeof(ContentPresenter)) // Display only the SVG content
            };

            // Add Trigger for Hover Effect (Change Cursor to Hand)
            template.Triggers.Add(new Trigger
            {
                Property = Button.IsMouseOverProperty, // Check if mouse is over the button
                Value = true,
                Setters =
        {
            new Setter(FrameworkElement.CursorProperty, Cursors.Hand) // Set cursor to hand
        }
            });

            // Create and return the button
            return new Button
            {
                Content = content,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                Margin = new Thickness(10, 0, 10, 0),
                Template = template
            };
        }

        private void ButtonMatch(object sender, RoutedEventArgs e)
        {
            ChooseGame chooseGame = new ChooseGame();
            chooseGame.Show();
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
