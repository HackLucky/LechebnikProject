using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LechebnikProject.Helpers
{
    public class SenderToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int senderId)
            {
                return senderId == AppContext.CurrentUser.UserId ? Brushes.LightBlue : Brushes.LightGreen;
            }
            return Brushes.Transparent; // Прозрачный фон по умолчанию
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // Обратное преобразование не требуется
        }
    }
}