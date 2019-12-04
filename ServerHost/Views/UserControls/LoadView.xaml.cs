using System.Windows.Controls;

namespace ServerHost.UserControls
{
    /// <summary>
    /// Логика взаимодействия для LoadView.xaml
    /// </summary>
    public partial class LoadView : UserControl
    {
        string text = string.Empty;
        

        public LoadView(string _text)
        {
            InitializeComponent();

            text = _text;
            message.Text = text;
        }
    }
}
