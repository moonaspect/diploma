using System.Windows;
using SharpVectors.Converters;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Data;

namespace Japanese
{
    public partial class RecordsTable : Window
    {
        public RecordsTable()
        {
            InitializeComponent();
            DataContext = new RecordsViewModel(); // Bind the ViewModel

            Grid mainGrid = new Grid();

            SvgViewbox bgViewbox = new SvgViewbox
            {
                Source = new Uri("pack://application:,,,/bg3.svg"),
                Stretch = System.Windows.Media.Stretch.UniformToFill,
            };
            Grid.SetRow(bgViewbox, 0);
            mainGrid.Children.Add(bgViewbox);

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

            mainGrid.Children.Add(closeButton);
            mainGrid.Children.Add(backButton);

            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Title
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // DataGrid
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Buttons

            var titleTextBlock = new TextBlock
            {
                Text = "Game Records Table",
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

            Binding itemsSourceBinding = new Binding("GameRecords");
            dataGrid.SetBinding(DataGrid.ItemsSourceProperty, itemsSourceBinding);

            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Player ID",
                Binding = new Binding("PlayerId"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Score",
                Binding = new Binding("Score"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });

            mainGrid.Children.Add(dataGrid);

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10)
            };
            Grid.SetRow(stackPanel, 2);

            var refreshButton = new Button
            {
                Content = "Refresh",
                Width = 100,
                Height = 30,
                Margin = new Thickness(5)
            };
            Binding refreshCommandBinding = new Binding("LoadRecordsCommand");
            refreshButton.SetBinding(Button.CommandProperty, refreshCommandBinding);

            var addButton = new Button
            {
                Content = "Add Record",
                Width = 100,
                Height = 30,
                Margin = new Thickness(5)
            };
            Binding addCommandBinding = new Binding("SaveRecordCommand");
            addButton.SetBinding(Button.CommandProperty, addCommandBinding);

            stackPanel.Children.Add(refreshButton);
            stackPanel.Children.Add(addButton);
            mainGrid.Children.Add(stackPanel);


            this.Content = mainGrid;
        }
        private Button CreateButton(SvgViewbox content)
        {
            var template = new ControlTemplate(typeof(Button))
            {
                VisualTree = new FrameworkElementFactory(typeof(ContentPresenter))
            };

            template.Triggers.Add(new Trigger
            {
                Property = Button.IsMouseOverProperty,
                Value = true,
                Setters =
        {
            new Setter(FrameworkElement.CursorProperty, Cursors.Hand)
        }
            });

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
