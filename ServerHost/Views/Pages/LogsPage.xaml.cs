using System.Windows.Controls;

namespace ServerHost.Pages
{
    /// <summary>
    /// Логика взаимодействия для LogsPage.xaml
    /// </summary>
    public partial class LogsPage : Page
    {
        public LogsPage()
        {
            InitializeComponent();

            DataContext = new LogsViewModel();
        }
    }
}
