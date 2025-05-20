using System;
using System.Globalization;
using System.Windows.Data;

namespace LechebnikProject.Helpers
{
    public class BooleanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? "Да" : "Нет";
            }
            return "Неизвестно"; // Значение по умолчанию, если входные данные некорректны
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // Обратное преобразование не требуется
        }
    }
}