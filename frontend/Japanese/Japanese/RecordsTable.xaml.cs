using System.Windows;

namespace Japanese
{
    public partial class RecordsTable : Window
    {
        public RecordsTable()
        {
            InitializeComponent();
            DataContext = new RecordsViewModel(); // Bind the ViewModel
        }
    }
}
