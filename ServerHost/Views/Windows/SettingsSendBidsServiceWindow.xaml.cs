using ServerHost.ViewModel;
using System.Windows;

namespace ServerHost.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для SettingsSendBidsServiceWindow.xaml
    /// </summary>
    public partial class SettingsSendBidsServiceWindow : Window
    {
        public SettingsSendBidsServiceWindow()
        {
            InitializeComponent();

            this.DataContext = new SettingsSendBidsViewModel(this);
        }
    }
}
