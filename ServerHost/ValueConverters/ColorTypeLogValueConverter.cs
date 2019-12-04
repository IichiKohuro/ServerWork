using ServerWork;
using System;
using System.Globalization;

namespace ServerHost
{
    public class ColorTypeLogValueConverter : BaseValueConverter<ColorTypeLogValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((LogType)value == LogType.Information)
                return "#65A7E0";

            if ((LogType)value == LogType.Warning)
                return "#FDBD1D";

            if ((LogType)value == LogType.Error)
                return "#EC6A54";

            return "#6EBF9E";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
