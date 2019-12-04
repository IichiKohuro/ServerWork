using System.Windows.Controls;

namespace ServerHost.Pages
{
    /// <summary>
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();

            DataContext = new SettingsViewModel();
        }
    }
}
