using System;
using System.Globalization;
using System.Windows.Data;

namespace ScreenTranslator.UI.Converters
{
    public class ValueEqualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            string checkValue = value.ToString();
            string targetValue = parameter.ToString();

            // Handle double comparison with tolerance if needed, but string match is usually enough for simple UI values
            if (double.TryParse(checkValue, out double d1) && double.TryParse(targetValue, out double d2))
            {
                return Math.Abs(d1 - d2) < 0.001;
            }

            return checkValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }
}
