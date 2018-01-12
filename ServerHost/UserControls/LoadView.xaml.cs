using System;
using System.Windows.Controls;


namespace ServerHost.UserControls
{
    /// <summary>
    /// Логика взаимодействия для LoadView.xaml
    /// </summary>
    public partial class LoadView : UserControl
    {
        public LoadView(string _text)
        {
            InitializeComponent();

            message.Text = _text;
        }
    }
}
