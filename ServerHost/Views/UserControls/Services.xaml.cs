using System.Windows.Controls;

namespace ServerHost.UserControls
{
    /// <summary>
    /// Логика взаимодействия для Services.xaml
    /// </summary>
    public partial class Services : UserControl
    {
        public Services()
        {
            InitializeComponent();

            DataContext = new ServicesViewModel();
        }
    }
}
