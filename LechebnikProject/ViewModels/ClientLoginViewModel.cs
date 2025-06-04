using LechebnikProject.Helpers;
using LechebnikProject.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace LechebnikProject.ViewModels
{
    public class ClientLoginViewModel
    {
        public string Login { get; set; }
        public string Code { get; set; }

        public bool Authenticate()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Code))
                {
                    MessageBox.Show("Логин и код не могут быть пустыми.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                string query = "SELECT * FROM Clients WHERE Login = @Login AND Code = @Code";
                var parameters = new[]
                {
                    new SqlParameter("@Login", Login),
                    new SqlParameter("@Code", Code)
                };
                DataTable dataTable = DatabaseHelper.ExecuteQuery(query, parameters);
                DataRow row = dataTable.Rows[0];
                string status = row["Status"].ToString();

                if (status == "Blocked")
                {
                    MessageBox.Show($"Ваш аккаунт заблокирован. Обратитесь к администратору: andrej_sok@mail.ru", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (dataTable.Rows.Count > 0)
                {
                    AppContext.CurrentClient = new Client
                    {
                        ClientId = (int)row["ClientId"],
                        LastName = row["LastName"].ToString(),
                        FirstName = row["FirstName"].ToString(),
                        MiddleName = row["MiddleName"]?.ToString(),
                        Login = row["Login"].ToString(),
                        Code = row["Code"].ToString(),
                        Discount = (decimal)row["Discount"],
                        Status = row["Status"].ToString()
                    };
                    MessageBox.Show("Клиент успешно аутентифицирован.", "Информирование.", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                else
                {
                    MessageBox.Show("Неверный логин или код.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            catch { MessageBox.Show("Неверный логин или код", "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
        }
    }
}