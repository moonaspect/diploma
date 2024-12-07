using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Japanese.Views;
using SharpVectors.Converters;

namespace Japanese
{
    /// <summary>
    /// Interaction logic for ChooseGame.xaml
    /// </summary>
    public partial class ChooseGame : Window
    {
        public ChooseGame()
        {
            InitializeComponent();
            Grid mainGrid = new Grid();
            SvgViewbox bgViewbox = new SvgViewbox
            {
                Source = new Uri("pack://application:,,,./Resources/bg2.svg"),
                Stretch = System.Windows.Media.Stretch.UniformToFill,
            };

            Grid.SetRow(bgViewbox, 0);
            mainGrid.Children.Add(bgViewbox);

            mainGrid.RowDefinitions.Add(
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
            ); // Background

            SvgViewbox wordsViewbox = new SvgViewbox
            {
                Source = new Uri("pack://application:,,,./Resources/words.svg"),
            };

            SvgViewbox hiraganaViewbox = new SvgViewbox
            {
                Source = new Uri("pack://application:,,,./Resources/hiragana.svg"),
            };

            SvgViewbox katakanaViewbox = new SvgViewbox
            {
                Source = new Uri("pack://application:,,,./Resources/katakana.svg"),
            };

            SvgViewbox closeViewbox = new SvgViewbox
            {
                Source = new Uri("pack://application:,,,./Resources/exit.svg"),
            };

            SvgViewbox backViewbox = new SvgViewbox
            {
                Source = new Uri("pack://application:,,,./Resources/back.svg"),
            };

            Button closeButton = CreateButton(closeViewbox);
            closeButton.HorizontalAlignment = HorizontalAlignment.Right;
            closeButton.VerticalAlignment = VerticalAlignment.Top;
            closeButton.Width = 43;
            closeButton.Height = 43;
            closeButton.Click += (s, e) => this.Close();
            closeButton.Margin = new Thickness(0, 10, 10, 0);

            Button backButton = CreateButton(backViewbox);
            backButton.HorizontalAlignment = HorizontalAlignment.Left;
            backButton.VerticalAlignment = VerticalAlignment.Top;
            backButton.Width = 43;
            backButton.Height = 43;
            backButton.Click += ButtonBack;
            backButton.Margin = new Thickness(10, 10, 0, 0);

            Button buttonHiragana = CreateButton(hiraganaViewbox);
            buttonHiragana.Width = 200;
            buttonHiragana.Height = 45;
            buttonHiragana.Click += ButtonHiragana;

            Button buttonKatakana = CreateButton(katakanaViewbox);
            buttonKatakana.Width = 200;
            buttonKatakana.Height = 45;

            Button buttonMatch = CreateButton(wordsViewbox);
            buttonMatch.Click += ButtonMatch;
            buttonMatch.Width = 200;
            buttonMatch.Height = 45;

            Grid buttonGrid = new Grid
            {
                Background = Brushes.Transparent,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 10)
            };

            buttonGrid.RowDefinitions.Add(
                new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
            );
            buttonGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10) });
            buttonGrid.RowDefinitions.Add(
                new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
            );
            buttonGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10) });
            buttonGrid.RowDefinitions.Add(
                new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
            );

            buttonGrid.Children.Add(buttonHiragana);
            buttonGrid.Children.Add(buttonKatakana);
            buttonGrid.Children.Add(buttonMatch);

            Grid.SetRow(buttonHiragana, 0);
            Grid.SetRow(buttonKatakana, 2);
            Grid.SetRow(buttonMatch, 4);

            mainGrid.Children.Add(buttonGrid);
            mainGrid.Children.Add(closeButton);
            mainGrid.Children.Add(backButton);

            this.Content = mainGrid;
        }

        private void ButtonMatch(object sender, RoutedEventArgs e)
        {
            MatchGame matchGame = new MatchGame();
            matchGame.Show();
            Close();
        }

        private void ButtonHiragana(object sender, RoutedEventArgs e)
        {
            Hiragana hiraganaGame = new Hiragana();
            hiraganaGame.Show();
            Close();
        }

        private void ButtonBack(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private Button CreateButton(SvgViewbox content)
        {
            var template = new ControlTemplate(typeof(Button))
            {
                VisualTree = new FrameworkElementFactory(typeof(ContentPresenter))
            };

            template.Triggers.Add(
                new Trigger
                {
                    Property = Button.IsMouseOverProperty,
                    Value = true,
                    Setters = { new Setter(FrameworkElement.CursorProperty, Cursors.Hand) }
                }
            );

            return new Button
            {
                Content = content,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                Template = template,
                Margin = new Thickness(10, 0, 10, 0) // Add horizontal spacing
            };
        }
    }
}
