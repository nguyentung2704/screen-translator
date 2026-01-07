using System;
using System.Globalization;
using System.Windows.Data;

namespace ScreenTranslator.UI.Converters
{
    public class MultiValueEqualityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return false;

            object value1 = values[0];
            object value2 = values[1];

            if (value1 == null || value2 == null)
                return false;

            return value1.ToString().Equals(value2.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
