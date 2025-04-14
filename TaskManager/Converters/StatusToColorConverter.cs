using System.Globalization;
using TaskManager.Models;

namespace TaskManager.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Status status)
            {
                return status switch
                {
                    Status.ToDo => Colors.DarkOrange,
                    Status.InProgress => Colors.DodgerBlue,
                    Status.Done => Colors.ForestGreen,
                    Status.Cancelled => Colors.Gray,
                    _ => Colors.Gray
                };
            }

            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 