using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Japanese.ViewModels;
using SharpVectors.Converters;

namespace Japanese
{
    public partial class MatchGame : Window
    {
        private Button? selectedJapaneseButton;
        private Button? selectedTranslationButton;
        private int mistakes = 0;
        public RecordsViewModel RecordsViewModel { get; set; }

        public MatchGame()
        {
            InitializeComponent();
            RecordsViewModel = new RecordsViewModel();

            Loaded += async (s, e) => await ViewModel.LoadWordsAsync();
            ViewModel.GameOver += OnGameOver; // Subscribe to GameOver event

            Grid mainGrid = new Grid();

            mainGrid.RowDefinitions.Add(
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
            );
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            SvgViewbox bgViewbox = new SvgViewbox
            {
                Source = new Uri("pack://application:,,,./Resources/bg3.svg"),
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
                Source = new Uri("pack://application:,,,./Resources/face.svg"),
            };
            faceViewbox.Width = 92;
            faceViewbox.Height = 66;
            faceViewbox.VerticalAlignment = VerticalAlignment.Bottom;
            faceViewbox.HorizontalAlignment = HorizontalAlignment.Right;
            faceViewbox.Margin = new Thickness(0, 0, 20, 20);
            mainGrid.Children.Add(faceViewbox);

            SvgViewbox closeViewbox = new SvgViewbox
            {
                Source = new Uri("pack://application:,,,./Resources/exit.svg"),
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
                Source = new Uri("pack://application:,,,./Resources/back.svg"),
            };
            Button backButton = CreateButton(backViewbox);
            backButton.HorizontalAlignment = HorizontalAlignment.Left;
            backButton.VerticalAlignment = VerticalAlignment.Top;
            backButton.Width = 43;
            backButton.Height = 43;
            backButton.Click += ButtonBack;
            backButton.Margin = new Thickness(10, 10, 0, 0);
            mainGrid.Children.Add(backButton);

            // Define the control template for buttons
            var buttonTemplate = new ControlTemplate(typeof(Button));
            var gridFactory = new FrameworkElementFactory(typeof(Grid));

            // Add SvgViewbox as background
            var backgroundFactory = new FrameworkElementFactory(typeof(SvgViewbox));
            backgroundFactory.Name = "BackgroundViewbox";
            backgroundFactory.SetValue(
                SvgViewbox.SourceProperty,
                new Uri("pack://application:,,,./Resources/buttonbg.svg")
            );
            backgroundFactory.SetValue(SvgViewbox.StretchProperty, Stretch.UniformToFill);
            gridFactory.AppendChild(backgroundFactory);

            // Add a TextBlock for the button text
            var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            textBlockFactory.SetValue(
                TextBlock.TextProperty,
                new TemplateBindingExtension(Button.ContentProperty)
            ); // TemplateBinding to Button.Content
            textBlockFactory.SetValue(
                TextBlock.HorizontalAlignmentProperty,
                HorizontalAlignment.Center
            );
            textBlockFactory.SetValue(
                TextBlock.VerticalAlignmentProperty,
                VerticalAlignment.Center
            );
            textBlockFactory.SetValue(TextBlock.ForegroundProperty, Brushes.White);
            textBlockFactory.SetValue(TextBlock.FontSizeProperty, 16.0);
            gridFactory.AppendChild(textBlockFactory);

            buttonTemplate.VisualTree = gridFactory;

            var trigger = new Trigger
            {
                Property = Button.IsEnabledProperty,
                Value = false // Trigger when the button is disabled
            };
            trigger.Setters.Add(
                new Setter
                {
                    Property = SvgViewbox.SourceProperty,
                    TargetName = "BackgroundViewbox", // Target the SvgViewbox
                    Value = new Uri("pack://application:,,,./Resources/buttonbg_disabled.svg") // Set to disabled background
                }
            );
            buttonTemplate.Triggers.Add(trigger);

            var hoverTrigger = new Trigger
            {
                Property = Button.IsMouseOverProperty,
                Value = true // Trigger when the mouse is over the button
            };
            hoverTrigger.Setters.Add(
                new Setter
                {
                    Property = FrameworkElement.CursorProperty, // Change the cursor
                    Value = Cursors.Hand
                }
            );
            buttonTemplate.Triggers.Add(hoverTrigger);

            // Japanese Template
            var japaneseTemplate = new DataTemplate();
            japaneseTemplate.VisualTree = new FrameworkElementFactory(typeof(Button));
            japaneseTemplate.VisualTree.SetValue(Button.TemplateProperty, buttonTemplate);
            japaneseTemplate.VisualTree.SetBinding(Button.ContentProperty, new Binding("Japanese"));
            japaneseTemplate.VisualTree.SetBinding(Button.UidProperty, new Binding("WordId"));
            japaneseTemplate.VisualTree.SetValue(Button.WidthProperty, 150.0);
            japaneseTemplate.VisualTree.SetValue(Button.HeightProperty, 55.0);
            japaneseTemplate.VisualTree.SetValue(Button.MarginProperty, new Thickness(5));
            japaneseTemplate.VisualTree.AddHandler(
                Button.ClickEvent,
                new RoutedEventHandler(JapaneseWordButton_Click)
            );

            // Ukrainian Template
            var ukrainianTemplate = new DataTemplate();
            ukrainianTemplate.VisualTree = new FrameworkElementFactory(typeof(Button));
            ukrainianTemplate.VisualTree.SetValue(Button.TemplateProperty, buttonTemplate);
            ukrainianTemplate.VisualTree.SetBinding(
                Button.ContentProperty,
                new Binding("Ukrainian")
            );
            ukrainianTemplate.VisualTree.SetBinding(Button.UidProperty, new Binding("WordId"));
            ukrainianTemplate.VisualTree.SetValue(Button.WidthProperty, 150.0);
            ukrainianTemplate.VisualTree.SetValue(Button.HeightProperty, 55.0);
            ukrainianTemplate.VisualTree.SetValue(Button.MarginProperty, new Thickness(5));
            ukrainianTemplate.VisualTree.AddHandler(
                Button.ClickEvent,
                new RoutedEventHandler(TranslationButton_Click)
            );

            // Japanese StackPanel
            var japaneseStackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(100, 0, 0, 0)
            };
            Grid.SetRow(japaneseStackPanel, 0);

            var japaneseItemsControl = new ItemsControl { ItemTemplate = japaneseTemplate };
            japaneseItemsControl.SetBinding(
                ItemsControl.ItemsSourceProperty,
                new Binding("CurrentJapanesePairs")
            );
            japaneseStackPanel.Children.Add(japaneseItemsControl);
            mainGrid.Children.Add(japaneseStackPanel);

            // Ukrainian StackPanel
            var ukrainianStackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 100, 0)
            };
            Grid.SetRow(ukrainianStackPanel, 0);

            var ukrainianItemsControl = new ItemsControl { ItemTemplate = ukrainianTemplate };
            ukrainianItemsControl.SetBinding(
                ItemsControl.ItemsSourceProperty,
                new Binding("CurrentUkrainianPairs")
            );
            ukrainianStackPanel.Children.Add(ukrainianItemsControl);
            mainGrid.Children.Add(ukrainianStackPanel);

            // Ensure the DataContext is set
            this.DataContext = ViewModel;

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
            template.Triggers.Add(
                new Trigger
                {
                    Property = Button.IsMouseOverProperty, // Check if mouse is over the button
                    Value = true,
                    Setters =
                    {
                        new Setter(FrameworkElement.CursorProperty, Cursors.Hand) // Set cursor to hand
                    }
                }
            );

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
                    ViewModel.HandleMismatch();
                    mistakes++;
                }

                selectedJapaneseButton = null;
                selectedTranslationButton = null;
            }
        }

        private async void OnGameOver(object? sender, EventArgs e)
        {
            var record = new GameRecord { PlayerId = "Player1", Score = 1000 - 100 * mistakes, };
            if (record.Score < 0)
                record.Score = 0;
            await RecordsViewModel.SaveRecordAsync(record);

            RecordsTable rec = new RecordsTable();
            rec.Show();
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
