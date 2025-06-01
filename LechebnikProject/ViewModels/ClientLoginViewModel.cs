using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace LechebnikProject.ViewModels
{
    public class ClientLoginViewModel
    {
        public string Login { get; set; }
        public string Code { get; set; }
        public Client AuthenticatedClient { get; set; }

        public bool Authenticate()
        {
            try
            {
                string query = "SELECT * FROM Clients WHERE Login = @Login AND Code = @Code";
                var parameters = new[]
                {
                    new SqlParameter("@Login", Login),
                    new SqlParameter("@Code", Code)
                };
                DataTable dataTable = DatabaseHelper.ExecuteQuery(query, parameters);
                if (dataTable.Rows.Count > 0)
                {
                    var row = dataTable.Rows[0];
                    AppContext.CurrentClient = new Client
                    {
                        ClientId = (int)row["ClientId"],
                        LastName = row["LastName"].ToString(),
                        FirstName = row["FirstName"].ToString(),
                        MiddleName = row["MiddleName"].ToString(),
                        Login = row["Login"].ToString(),
                        Code = row["Code"].ToString(),
                        Discount = (decimal)row["Discount"],
                        Status = row["Status"].ToString()
                    };

                    MessageBox.Show("Клиент аутентифицирован.", "Информирование.", MessageBoxButton.OK, MessageBoxImage.Information);
                    WindowManager.ShowWindow<QuantityInputWindow>();
                    return true;
                }
                else { MessageBox.Show("Возможно, неверный логин или код.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning); return false; }
            }
            catch { MessageBox.Show("Произошла ошибка при попытке аутентификации.", "Ошибка.", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
        }
    }
}