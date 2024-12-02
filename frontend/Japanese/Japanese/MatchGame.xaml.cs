﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using SharpVectors.Converters;

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

            Grid mainGrid = new Grid();

            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });


            SvgViewbox bgViewbox = new SvgViewbox
            {
                Source = new Uri("pack://application:,,,/bg3.svg"),
                Stretch = System.Windows.Media.Stretch.UniformToFill,
            };

            Grid.SetRow(bgViewbox, 0);
            Grid.SetRowSpan(bgViewbox, 2);
            mainGrid.Children.Add(bgViewbox);

            var messageTextBlock = new TextBlock
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 10, 0, 0),
                Foreground = System.Windows.Media.Brushes.DarkBlue,
                TextAlignment = TextAlignment.Center
            };
            var messageBinding = new Binding("GameMessage");
            messageTextBlock.SetBinding(TextBlock.TextProperty, messageBinding);
            mainGrid.Children.Add(messageTextBlock);

            SvgViewbox faceViewbox = new SvgViewbox
            {
                Source = new Uri("pack://application:,,,/face.svg"),
            };
            faceViewbox.Width = 92;
            faceViewbox.Height = 66;
            faceViewbox.VerticalAlignment = VerticalAlignment.Bottom;
            faceViewbox.HorizontalAlignment = HorizontalAlignment.Right;
            faceViewbox.Margin = new Thickness(0, 0, 20, 20);
            mainGrid.Children.Add(faceViewbox);

            SvgViewbox closeViewbox = new SvgViewbox
            {
                Source = new Uri("pack://application:,,,/exit.svg"),
            };
            Button closeButton = CreateButton(closeViewbox);
            closeButton.HorizontalAlignment = HorizontalAlignment.Right;
            closeButton.VerticalAlignment = VerticalAlignment.Top;
            closeButton.Width = 43;
            closeButton.Height = 43;
            closeButton.Click += (s, e) => this.Close();
            closeButton.Margin = new Thickness(0, 10, 10, 0);
            mainGrid.Children.Add(closeButton);

            SvgViewbox backViewbox = new SvgViewbox
            {
                Source = new Uri("pack://application:,,,/back.svg"),
            };
            Button backButton = CreateButton(backViewbox);
            backButton.HorizontalAlignment = HorizontalAlignment.Left;
            backButton.VerticalAlignment = VerticalAlignment.Top;
            backButton.Width = 43;
            backButton.Height = 43;
            backButton.Click += ButtonBack;
            backButton.Margin = new Thickness(10, 10, 0, 0);
            mainGrid.Children.Add(backButton);

            var japaneseStackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(50, 0, 0, 0)
            };
            var japaneseItemsControl = new ItemsControl();
            var japaneseTemplate = new DataTemplate
            {
                VisualTree = new FrameworkElementFactory(typeof(Button))
            };
            japaneseTemplate.VisualTree.SetBinding(Button.ContentProperty, new Binding("Japanese"));
            japaneseTemplate.VisualTree.SetBinding(Button.UidProperty, new Binding("WordId"));
            japaneseTemplate.VisualTree.SetValue(Button.WidthProperty, 150.0);
            japaneseTemplate.VisualTree.SetValue(Button.HeightProperty, 55.0);
            japaneseTemplate.VisualTree.SetValue(Button.MarginProperty, new Thickness(5));
            japaneseTemplate.VisualTree.AddHandler(Button.ClickEvent, new RoutedEventHandler(JapaneseWordButton_Click));
            japaneseItemsControl.ItemTemplate = japaneseTemplate;
            japaneseItemsControl.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("CurrentJapanesePairs"));

            japaneseStackPanel.Children.Add(japaneseItemsControl);
            mainGrid.Children.Add(japaneseStackPanel);

            // Create StackPanel for Ukrainian Words
            var ukrainianStackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 50, 0)
            };

            var ukrainianItemsControl = new ItemsControl();
            var ukrainianTemplate = new DataTemplate
            {
                VisualTree = new FrameworkElementFactory(typeof(Button))
            };
            ukrainianTemplate.VisualTree.SetBinding(Button.ContentProperty, new Binding("Ukrainian"));
            ukrainianTemplate.VisualTree.SetBinding(Button.UidProperty, new Binding("WordId"));
            ukrainianTemplate.VisualTree.SetValue(Button.WidthProperty, 150.0);
            ukrainianTemplate.VisualTree.SetValue(Button.HeightProperty, 55.0);
            ukrainianTemplate.VisualTree.SetValue(Button.MarginProperty, new Thickness(5));
            ukrainianTemplate.VisualTree.AddHandler(Button.ClickEvent, new RoutedEventHandler(TranslationButton_Click));
            ukrainianItemsControl.ItemTemplate = ukrainianTemplate;
            ukrainianItemsControl.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("CurrentUkrainianPairs"));

            ukrainianStackPanel.Children.Add(ukrainianItemsControl);
            mainGrid.Children.Add(ukrainianStackPanel);

            this.Content = mainGrid;

        }

        private void ButtonBack(object sender, RoutedEventArgs e)
        {
            ChooseGame chooseGame = new ChooseGame();
            chooseGame.Show();
            Close();
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
