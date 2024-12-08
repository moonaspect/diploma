using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Japanese.ViewModels;
using SharpVectors.Converters;

namespace Japanese.Views
{
    /// <summary>
    /// Interaction logic for Dictionary.xaml
    /// </summary>
    public partial class Dictionary : Window
    {
        public Dictionary()
        {
            InitializeComponent();
            DataContext = new DictionaryViewModel();

            Grid mainGrid = new Grid();

            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Title
            mainGrid.RowDefinitions.Add(
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
            ); // DataGrid
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Buttons

            SvgViewbox bgViewbox = new SvgViewbox
            {
                Source = new Uri("pack://application:,,,./Resources/bg3.svg"),
                Stretch = Stretch.UniformToFill,
            };
            Grid.SetRowSpan(bgViewbox, 3);
            mainGrid.Children.Add(bgViewbox);

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

            mainGrid.Children.Add(closeButton);
            mainGrid.Children.Add(backButton);

            var titleTextBlock = new TextBlock
            {
                Text = "Dictionary",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10)
            };
            Grid.SetRow(titleTextBlock, 0);
            mainGrid.Children.Add(titleTextBlock);

            var dataGrid = new DataGrid
            {
                AutoGenerateColumns = false,
                CanUserAddRows = false,
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Stretch
            };
            Grid.SetRow(dataGrid, 1);

            Binding itemsSourceBinding = new Binding("WordPairs");
            dataGrid.SetBinding(ItemsControl.ItemsSourceProperty, itemsSourceBinding);

            dataGrid.Columns.Add(
                new DataGridTextColumn
                {
                    Header = "Japanese",
                    Binding = new Binding("Japanese"),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star)
                }
            );
            dataGrid.Columns.Add(
                new DataGridTextColumn
                {
                    Header = "Ukrainian",
                    Binding = new Binding("Ukrainian"),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star)
                }
            );

            mainGrid.Children.Add(dataGrid);

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10)
            };
            Grid.SetRow(stackPanel, 2);

            mainGrid.Children.Add(stackPanel);

            Content = mainGrid;

            Loaded += (sender, args) =>
            {
                if (
                    DataContext is DictionaryViewModel viewModel
                    && viewModel.LoadWordsCommand.CanExecute(null)
                )
                {
                    viewModel.LoadWordsCommand.Execute(null);
                }
            };
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

        private void ButtonBack(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
    }
}
