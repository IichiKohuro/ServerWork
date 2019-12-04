using ServerHost.Pages;
using System;
using System.Diagnostics;
using System.Globalization;

namespace ServerHost
{
    /// <summary>
    /// Converts the ApplicationPage to an actual view/page
    /// </summary>
    public class ApplicationPageValueConverter : BaseValueConverter<ApplicationPageValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch((ApplicationPage)value)
            {
                case ApplicationPage.Dashboard:
                    return new DashboardPage();

                case ApplicationPage.Settings:
                    return new SettingsPage();

                case ApplicationPage.Logs:
                    return new LogsPage();

                default:
                    Debugger.Break();
                    return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
