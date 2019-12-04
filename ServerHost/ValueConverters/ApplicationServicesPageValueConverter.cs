using ServerHost.Views.Pages;
using System;
using System.Diagnostics;
using System.Globalization;

namespace ServerHost
{
    /// <summary>
    /// Converts the ApplicationPage to an actual view/page
    /// </summary>
    public class ApplicationServicesPageValueConverter : BaseValueConverter<ApplicationServicesPageValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch((ApplicationServicePage)value)
            {
                case ApplicationServicePage.General:
                    return new GeneralSettingsPage();

                case ApplicationServicePage.Mail:
                    return new MailSettingsPage();

                case ApplicationServicePage.Services:
                    return new ServicesSettingsPage();

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
