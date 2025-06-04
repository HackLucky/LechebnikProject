using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LechebnikProject.Helpers
{
    public class SenderRoleToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int senderId)
            {
                string query = "SELECT Role FROM Users WHERE UserId = @UserId";
                var param = new[] { new SqlParameter("@UserId", senderId) };
                string role = DatabaseHelper.ExecuteScalar(query, param)?.ToString();
                return role == "Admin" ? Brushes.Green : Brushes.Black;
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}