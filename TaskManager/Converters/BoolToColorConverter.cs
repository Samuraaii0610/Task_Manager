using System.Globalization;

namespace TaskManager.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string strParam)
            {
                var colors = strParam.Split('|');
                if (colors.Length >= 2)
                {
                    return boolValue ? 
                        colors[0] : // Si vrai, première couleur
                        colors[1];  // Si faux, deuxième couleur
                }
            }
            
            return Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 