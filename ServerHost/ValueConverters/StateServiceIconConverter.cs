using System;
using System.Globalization;
using System.Windows.Data;

namespace ServerHost
{
    public class StateServiceIconConverter : BaseValueConverter<StateServiceIconConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == true)
                return "Cancel";
            else
                return "Run";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
